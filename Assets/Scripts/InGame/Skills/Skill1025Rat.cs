using System.Collections;
using ProjectCode1.InGame.Monsters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1025Rat : MonoBehaviour
    {
        [SerializeField] private ParticleSystem explodeEffect;
        private SpriteRenderer spriteRenderer;
        private Skill1025 skill;
        private Monster targetMonster;
        private bool explodeMode;
        private bool exploded;
        private float scaleXMultiplier = 1;
        private float defaultScaleX;

        void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            defaultScaleX = spriteRenderer.transform.localScale.x;
        }

        public void Init(Skill1025 skill) => this.skill = skill;

        public void Setup(bool explodeMode, Monster targetMonster = null)
        {
            this.explodeMode = explodeMode;
            spriteRenderer.enabled = true;
            exploded = false;
            this.targetMonster = targetMonster;

            if (explodeMode && targetMonster == null)
            {
                FindTarget();
            }
        }

        void Update()
        {
            if (exploded) return;

            Animate();

            Vector3 v;
            if (explodeMode)
            {
                if (targetMonster == null || !targetMonster.Alive)
                {
                    FindTarget();
                }

                if (targetMonster != null)
                {
                    v = targetMonster.transform.position - transform.position;
                }
                else
                {
                    StartCoroutine(Explode());
                    return;
                }
            }
            else
            {
                v = skill.Owner.transform.position - transform.position;
            }

            transform.position += Time.deltaTime * skill.RatSpeed * v.normalized;
            spriteRenderer.flipX = v.x < 0;

            if (v.sqrMagnitude <= 0.1f)
            {
                if (explodeMode)
                {
                    StartCoroutine(Explode());
                }
                else
                {
                    skill.StoreRat(this);
                }
            }
        }

        void Animate()
        {
            spriteRenderer.transform.localScale = new Vector3(
                spriteRenderer.transform.localScale.x + scaleXMultiplier * Time.deltaTime * 2,
                spriteRenderer.transform.localScale.y,
                spriteRenderer.transform.localScale.z
            );

            if (spriteRenderer.transform.localScale.x >= defaultScaleX + 0.1f)
            {
                scaleXMultiplier = -1;
            }
            else if (spriteRenderer.transform.localScale.x <= defaultScaleX - 0.1f)
            {
                scaleXMultiplier = 1;
            }
        }

        void FindTarget()
        {
            var randomMonsters = EntityManager.Instance.GetAllVisibleMonsters(1);
            if (randomMonsters.Count > 0)
            {
                targetMonster = randomMonsters[0];
            }
        }

        IEnumerator Explode()
        {
            exploded = true;
            spriteRenderer.enabled = false;

            var radius = skill.FinalAreaDamage;
            var main = explodeEffect.main;
            main.startSpeed = radius / main.startLifetime.constant;
            explodeEffect.Play();

            var colliders = Physics2D.OverlapCircleAll(transform.position, radius, skill.Owner.DamageableLayerMask);
            foreach (var col in colliders)
            {
                if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) continue;
                skill.ApplyDamage(damageable);
            }

            while (explodeEffect.isPlaying)
            {
                yield return null;
            }

            skill.DespawnRat(this);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if ((skill.Owner.DamageableLayerMask & (1 << collision.gameObject.layer)) != 0 && !exploded)
            {
                if (explodeMode)
                {
                    StartCoroutine(Explode());
                }
                else if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
                {
                    skill.ApplyDamage(damageable);
                }
            }
        }
    }
}
