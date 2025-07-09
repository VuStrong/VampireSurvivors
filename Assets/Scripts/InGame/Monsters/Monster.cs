using System;
using System.Collections;
using ProjectCode1.Blueprints;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.InGame.Monsters
{
    public abstract class Monster : MonoBehaviour, IDamageable
    {
        private static WaitForSeconds hitAnimationDelay;

        [SerializeField] protected MonsterBlueprint monsterBlueprint;
        [SerializeField] protected Slider healthBar;
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Collider2D col;

        protected float currentHealth;
        protected bool alive = true;
        protected Transform target;
        private Coroutine hitAnimationCoroutine;
        protected float slowEffect;
        private Coroutine disableSlowCoroutine;

        public string Id { get => monsterBlueprint.id; }
        public Action OnDeath;
        public Vector3 Position { get => transform.position; }
        public Collider2D Col { get => col; }
        public bool Alive { get => alive; }

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            col = GetComponent<Collider2D>();

            currentHealth = monsterBlueprint.health;
            if (healthBar != null)
            {
                healthBar.maxValue = currentHealth;
                healthBar.value = currentHealth;
            }

            hitAnimationDelay ??= new WaitForSeconds(0.15f);
        }

        public void Setup(Transform target)
        {
            this.target = target;
            alive = true;
            slowEffect = 0;

            currentHealth = monsterBlueprint.health * EntityManager.Instance.MonsterHpMultiplier;
            if (healthBar != null)
            {
                healthBar.maxValue = currentHealth;
                healthBar.value = currentHealth;
            }
        }

        protected virtual void Update()
        {
            if (target == null) return;

            spriteRenderer.flipX = target.position.x - transform.position.x < 0;
        }

        protected virtual void FixedUpdate()
        {
        }

        public void TakeDamage(float amount, Vector2 knockback = default, GameObject source = null, bool isCrit = false)
        {
            if (!alive) return;

            currentHealth -= amount;
            if (healthBar != null)
                healthBar.value = currentHealth;

            if (knockback != default)
                rb.AddForce(knockback);

            if (hitAnimationCoroutine != null)
                StopCoroutine(hitAnimationCoroutine);

            EntityManager.Instance.SpawnDamageText(transform.position, amount, isCrit);

            if (currentHealth <= 0)
            {
                StartCoroutine(KilledCoroutine());
            }
            else
            {
                hitAnimationCoroutine = StartCoroutine(HitAnimation());
            }
        }

        private IEnumerator HitAnimation()
        {
            spriteRenderer.color = Color.red;
            yield return hitAnimationDelay;
            spriteRenderer.color = Color.white;
        }

        private IEnumerator KilledCoroutine()
        {
            alive = false;

            yield return HitAnimation();

            EntityManager.Instance.DespawnMonster(this);
            DropExp();

            OnDeath?.Invoke();
        }

        public void Kill()
        {
            TakeDamage(currentHealth);
        }

        protected virtual void DropExp()
        {
            EntityManager.Instance.SpawnExp(transform.position);
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        /// <summary>
        /// Slow down monster
        /// </summary>
        /// <param name="amount">Slow amount</param>
        /// <param name="duration">How long in seconds?, 0 to forever</param>
        public void Slow(float amount, float duration = 0)
        {
            if (!alive) return;
            if (slowEffect <= amount || amount == 0)
            {
                if (disableSlowCoroutine != null) StopCoroutine(disableSlowCoroutine);
                slowEffect = amount;
                if (duration > 0)
                {
                    disableSlowCoroutine = StartCoroutine(DisableSlowCoroutine(duration));
                }
            }
        }

        IEnumerator DisableSlowCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            slowEffect = 0;
        }

        public void Push(Vector2 force)
        {
            rb.AddForce(force);
        }
    }
}
