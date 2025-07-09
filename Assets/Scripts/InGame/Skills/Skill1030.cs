using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1030: BreathFire - Idle: Peachone VS but it is dragon
    /// </summary>
    public class Skill1030 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat duration;
        [SerializeField] private UpgradeableFloatStat cooldownBonus;
        [SerializeField] private Skill1030Projectile projectilePrefab;
        [SerializeField] private float circleSpeed;
        [SerializeField] private float cooldown;
        [SerializeField] private float timeBetweenSet;
        [SerializeField] private GameObject circleIndicator;

        private SpriteRenderer spriteRenderer;
        private ObjectPool<Skill1030Projectile> projectilePool;
        private float timeSinceLastCast;
        private bool isCasting;
        private Vector3 moveDirection;

        public float MoveSpeed { get => Mathf.Clamp(owner.stats.speed / 2, 1, owner.stats.speed); }
        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalDuration { get => owner.stats.skillDurationBonus + duration.Value; }
        public float FinalCooldown { get => Mathf.Clamp(cooldown * (1 - (owner.stats.cooldownReduce + cooldownBonus.Value) / 100), 0.5f, cooldown); }
        public float CircleSpeed { get => circleSpeed * (1 + owner.stats.projectileSpeedBonus / 100); }

        void Start()
        {
            transform.parent = null;

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            circleIndicator.SetActive(false);
            projectilePool = new ObjectPool<Skill1030Projectile>(
                () =>
                {
                    var projectile = Instantiate(projectilePrefab);
                    projectile.Init(this);
                    return projectile;
                },
                (projectile) =>
                {
                    projectile.gameObject.SetActive(true);
                    projectile.transform.position = transform.position;
                },
                (projectile) =>
                {
                    projectile.gameObject.SetActive(false);
                },
                (projectile) => Destroy(projectile.gameObject),
                false
            );
        }

        void Update()
        {
            if (!Owned) return;

            Move();

            if (!isCasting) timeSinceLastCast += Time.deltaTime;
            if (timeSinceLastCast >= FinalCooldown && !isCasting)
            {
                StartCoroutine(Cast());
                timeSinceLastCast = 0;
            }
        }

        void Move()
        {
            Vector3 v = owner.transform.position + new Vector3(0.3f, 0f) - transform.position;
            float speed;
            if (v.sqrMagnitude > 0.1f)
            {
                moveDirection = v.normalized;
                speed = MoveSpeed;
            }
            else
            {
                speed = 0.4f;
            }

            transform.position += speed * Time.deltaTime * moveDirection;
            spriteRenderer.flipX = v.x < 0;
        }

        IEnumerator Cast()
        {
            int totalProjectileCount = TotalProjectileCount;
            if (totalProjectileCount <= 0) yield break;

            isCasting = true;
            float delay = Mathf.Clamp(timeBetweenSet * (1 - cooldownBonus.Value / 100), 0.1f, timeBetweenSet); // delay between sets
            float finalDuration = FinalDuration;
            float angle = 0;
            float interval = 0;
            float time = 0;
            circleIndicator.SetActive(true);

            while (time < finalDuration)
            {
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4;
                Vector2 location = owner.transform.position + offset;

                if (interval >= delay)
                {
                    for (int i = 0; i < totalProjectileCount; i++)
                    {
                        Vector2 randomPoint = location + Random.insideUnitCircle;
                        var projectile = projectilePool.Get();
                        projectile.SetDestination(randomPoint, (i + 1) * 0.1f, i < totalProjectileCount / 2);
                    }

                    interval = 0;
                }

                circleIndicator.transform.position = location;

                yield return null;
                time += Time.deltaTime;
                interval += Time.deltaTime;
                angle -= CircleSpeed * Time.deltaTime;
            }

            isCasting = false;
            timeSinceLastCast = 0;
            circleIndicator.SetActive(false);
        }

        public void DespawnProjectile(Skill1030Projectile projectile)
        {
            projectilePool.Release(projectile);
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
