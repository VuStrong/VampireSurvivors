using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1019: Orb - Idle: X orb fly circle around player shot 1 projectile forward --- Moving: X orb froze and shot nearest enemy
    /// </summary>
    public class Skill1019 : Skill
    {
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat speed;
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private float shootDelay;
        [SerializeField] private GameObject orbPrefab;
        [SerializeField] private Skill1019Projectile projectilePrefab;

        private readonly List<GameObject> orbs = new();
        private ObjectPool<Skill1019Projectile> projectilePool;
        private float timeSinceLastShoot;
        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalSpeed { get => speed.Value * (1 + owner.stats.projectileSpeedBonus / 100); }

        protected override void Awake()
        {
            base.Awake();

            projectilePool = new ObjectPool<Skill1019Projectile>(
                () =>
                {
                    var projectile = Instantiate(projectilePrefab, transform, false);
                    projectile.Init(this);
                    return projectile;
                },
                (projectile) =>
                {
                    projectile.gameObject.SetActive(true);
                    projectile.transform.parent = null;
                },
                (projectile) =>
                {
                    projectile.gameObject.SetActive(false);
                    projectile.transform.parent = transform;
                },
                (projectile) => Destroy(projectile.gameObject),
                false
            );
        }

        void Update()
        {
            CheckOrbs();

            if (!owner.IsMoving)
            {
                transform.RotateAround(transform.position, Vector3.back, Time.deltaTime * FinalSpeed);
            }

            timeSinceLastShoot += Time.deltaTime;
            if (timeSinceLastShoot >= shootDelay)
            {
                Shoot();
                timeSinceLastShoot = 0;
            }
        }

        void CheckOrbs()
        {
            int count = TotalProjectileCount;
            if (orbs.Count == count) return;

            // add or remove orbs
            if (orbs.Count < count)
            {
                int amountToAdd = count - orbs.Count;
                for (int i = 0; i < amountToAdd; i++)
                {
                    var orb = Instantiate(orbPrefab, transform, false);
                    orbs.Add(orb);
                }
            }
            else
            {
                int amountToDestroy = orbs.Count - count;
                for (int i = 0; i < amountToDestroy; i++)
                {
                    int index = orbs.Count - 1;
                    Destroy(orbs[index]);
                    orbs.RemoveAt(index);
                }
            }

            // RePosition
            count = orbs.Count;
            float radius = 1;
            for (int i = 0; i < count; i++)
            {
                float angle = i * (2 * Mathf.PI / count);
                Vector2 position = new(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

                orbs[i].transform.localPosition = position;
            }
        }

        void Shoot()
        {
            bool ownerIsMoving = owner.IsMoving;
            foreach (var orb in orbs)
            {
                var projectile = SpawnProjectile();
                Vector2 direction = Vector2.right;

                if (ownerIsMoving)
                {
                    var monster = EntityManager.Instance.GetClosestMonster(orb.transform.position);
                    if (monster != null)
                    {
                        direction = (monster.transform.position - orb.transform.position).normalized;
                    }
                }
                else
                {
                    direction = (orb.transform.position - owner.transform.position).normalized;
                }

                projectile.transform.position = orb.transform.position;
                projectile.Setup(direction);
            }
        }

        public Skill1019Projectile SpawnProjectile()
        {
            var projectile = projectilePool.Get();
            return projectile;
        }

        public void DespawnProjectile(Skill1019Projectile projectile)
        {
            projectilePool.Release(projectile);
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}