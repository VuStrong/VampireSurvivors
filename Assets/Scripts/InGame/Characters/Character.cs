using System;
using System.Collections;
using System.Collections.Generic;
using ProjectCode1.InGame.Skills;
using ProjectCode1.Blueprints;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.InGame.Characters
{
    public class Character : MonoBehaviour, IDamageable
    {
        [SerializeField] protected CharacterBlueprint characterBlueprint;
        [SerializeField] protected Slider healthBar;
        [SerializeField] protected CircleCollider2D collectCollider;
        [SerializeField] protected LayerMask damageableLayerMask;
        protected Collider2D mainCollider;
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;

        [HideInInspector] public CharacterStats stats;
        protected float currentHealth;
        protected int currentLevel = 1;
        protected float currentExperience = 0;
        protected float experienceToNextLevel = 5;
        protected bool alive = true;
        protected Vector2 lookDirection = Vector2.right;
        protected Vector2 moveDirection;
        protected List<Skill> skills = new();
        private Coroutine hitAnimationCoroutine;
        private float speedUpAmount;

        public Action<float> OnExperienceIncreased;
        public Action<int> OnLevelUp;
        public Action OnDeath;
        public readonly List<IDamageable> unitsTakeDamage = new();
        public Vector2 LookDirection { get => lookDirection; }
        public bool IsMoving { get => moveDirection != Vector2.zero; }
        public float ExperienceToNextLevel { get => experienceToNextLevel; }
        public IReadOnlyList<Skill> Skills { get => skills.AsReadOnly(); }
        public LayerMask DamageableLayerMask { get => damageableLayerMask; }
        public Vector3 Position { get => transform.position; }
        public int CurrentLevel { get => currentLevel; }
        public float TotalSpeed { get => stats.speed + speedUpAmount; }

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            mainCollider = GetComponent<Collider2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();

            stats ??= new CharacterStats();
            stats.ImportBlueprint(characterBlueprint);

            if (characterBlueprint.startingSkill != null)
            {
                var skill = Instantiate(characterBlueprint.startingSkill, transform, false);
                AddSkill(skill);
            }
        }

        void Start()
        {
            Init();
        }

        void Init()
        {
            alive = true;
            currentHealth = stats.health;
            if (healthBar != null) healthBar.value = 1;
            StartCoroutine(RecoveryCoroutine());
        }

        protected virtual void Update()
        {
            spriteRenderer.flipX = lookDirection.x < 0;

            if (collectCollider.radius != stats.pickupRange)
                collectCollider.radius = stats.pickupRange;
        }

        protected virtual void FixedUpdate()
        {
            if (IsMoving)
            {
                lookDirection = moveDirection;
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }

            if (alive)
            {
                rb.linearVelocity += (stats.speed + speedUpAmount) * Time.fixedDeltaTime * moveDirection;
            }
        }

        public void SetMoveDirection(Vector2 moveDirection)
        {
            this.moveDirection = moveDirection;
        }

        public virtual void TakeDamage(float amount, Vector2 knockback = default, GameObject source = null, bool isCrit = false)
        {
            if (!alive) return;

            // If have any UnitsTakeDamage, it will take damage for player instead
            if (unitsTakeDamage.Count > 0)
            {
                unitsTakeDamage[0].TakeDamage(amount, knockback, source, isCrit);
                return;
            }

            // Apply armor
            if (stats.armor >= amount)
                amount = amount < 1 ? amount : 1;
            else
                amount -= stats.armor;

            // Decrease health
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, stats.health);
            if (healthBar != null) healthBar.value = currentHealth / stats.health;

            if (currentHealth == 0)
            {
                alive = false;

                if (stats.reviveCount > 0)
                {
                    StartCoroutine(Revive());
                }
                else
                {
                    OnDeath?.Invoke();
                }
            }
            else
            {
                if (hitAnimationCoroutine != null) StopCoroutine(hitAnimationCoroutine);
                hitAnimationCoroutine = StartCoroutine(HitAnimation());
            }
        }

        private IEnumerator Revive()
        {
            if (stats.reviveCount > 0) stats.reviveCount--;
            yield return new WaitForSeconds(1);
            Init();
        }

        private IEnumerator HitAnimation()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = Color.white;
        }

        public void GainHealth(float health)
        {
            if (!alive || health <= 0) return;

            currentHealth += health;
            currentHealth = Math.Clamp(currentHealth, 0, stats.health);
            if (healthBar != null) healthBar.value = currentHealth / stats.health;

            EntityManager.Instance?.SpawnHealingText(
                transform.position + new Vector3(spriteRenderer.bounds.size.x / 2, 0, 0),
                health
            );
        }

        IEnumerator RecoveryCoroutine()
        {
            var delay = new WaitForSeconds(1);
            while (alive)
            {
                yield return delay;
                GainHealth(stats.recoveryPerSecond);
            }
        }

        public void GainExperience(float experience)
        {
            if (!alive) return;

            experience += stats.experienceBonus / 100 * experience;
            currentExperience += experience;

            if (currentExperience >= experienceToNextLevel)
            {
                LevelUp();
            }
            else
            {
                OnExperienceIncreased?.Invoke(currentExperience);
            }
        }

        public void LevelUp()
        {
            if (!alive) return;

            currentLevel++;
            currentExperience = 0;
            experienceToNextLevel += 5;

            OnLevelUp?.Invoke(currentLevel);
        }

        public void AddSkill(Skill skill)
        {
            skill.Init(this);
            skills.Add(skill);
        }

        /// <summary>
        /// Ghost mode: Immune to damage from monsters and passes through them
        /// </summary>
        public void SetGhostMode(bool active = true)
        {
            if (active)
            {
                mainCollider.excludeLayers = 1 << LayerMask.NameToLayer("Monster");
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, .5f);
            }
            else
            {
                mainCollider.excludeLayers = 0;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
        }

        /// <summary>
        /// Increase speed of the character. Can be used for features like speedup over a period of time or an area.
        /// To speedup forever, modify the base speed (stats.speed) instead
        /// </summary>
        public void IncreaseSpeed(float amount, float duration = 0)
        {
            if (amount <= 0) return;

            if (duration > 0)
            {
                StartCoroutine(SpeedUpCoroutine(amount, duration));
            }
            else
            {
                speedUpAmount += amount;
            }
        }

        public void DecreaseSpeed(float amount)
        {
            if (amount <= 0) return;

            speedUpAmount -= amount;
            if (speedUpAmount < 0) speedUpAmount = 0;
        }

        IEnumerator SpeedUpCoroutine(float amount, float duration)
        {
            speedUpAmount += amount;
            yield return new WaitForSeconds(duration);
            speedUpAmount -= amount;
        }
    }
}
