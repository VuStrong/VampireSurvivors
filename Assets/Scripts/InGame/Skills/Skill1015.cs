using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1015: Grenade - 
    /// Idle: Throw X grenades to random spot deal area damage in X seconds --- 
    /// Moving: Throw X grenades to nearest enemies deal area damage
    /// </summary>
    public class Skill1015 : Skill
    {
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat duration;
        [SerializeField] private UpgradeableFloatStat cooldown;
        [SerializeField] private Skill1015Grenade grenadePrefab;
        [SerializeField] private Skill1015DamageArea damageAreaPrefab;

        private ObjectPool<Skill1015Grenade> grenadePool;
        private ObjectPool<Skill1015DamageArea> damageAreaPool;
        private float timeSinceLastThrow;
        private bool isThrowing;
        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalDuration { get => owner.stats.skillDurationBonus + duration.Value; }
        public float FinalCooldown { get => Mathf.Clamp(cooldown.Value * (1 - owner.stats.cooldownReduce / 100), 1, cooldown.Value); }
        public float FinalArea { get => area.Value * (1 + owner.stats.skillAreaBonus / 100); }

        protected override void Awake()
        {
            base.Awake();

            grenadePool = new(
                () =>
                {
                    var grenade = Instantiate(grenadePrefab, transform, false);
                    grenade.Init(this);
                    return grenade;
                },
                (grenade) =>
                {
                    grenade.gameObject.SetActive(true);
                    grenade.Setup();
                },
                (grenade) =>
                {
                    grenade.gameObject.SetActive(false);
                    grenade.transform.parent = transform;
                },
                (grenade) => Destroy(grenade.gameObject),
                false,
                defaultCapacity: 6
            );

            damageAreaPool = new(
                () =>
                {
                    var area = Instantiate(damageAreaPrefab, transform, false);
                    area.Init(this);
                    return area;
                },
                (area) =>
                {
                    area.gameObject.SetActive(true);
                    area.transform.parent = null;
                    area.Setup();
                },
                (area) =>
                {
                    area.gameObject.SetActive(false);
                    area.transform.parent = transform;
                },
                (area) => Destroy(area.gameObject),
                false,
                defaultCapacity: 6
            );
        }

        void Update()
        {
            if (!isThrowing) timeSinceLastThrow += Time.deltaTime;
            if (timeSinceLastThrow >= FinalCooldown)
            {
                if (owner.IsMoving)
                {
                    StartCoroutine(ThrowNearestEnemies());
                }
                else
                {
                    ThrowRandomSpots();
                }

                timeSinceLastThrow = 0;
            }
        }

        IEnumerator ThrowNearestEnemies()
        {
            int count = TotalProjectileCount;
            var monsters = EntityManager.Instance.GetClosestMonsters(transform.position, count);
            if (monsters.Count == 0) yield break;

            isThrowing = true;
            for (int i = 0; i < count; i++)
            {
                var monsterToThrow = i < monsters.Count ? monsters[i] : monsters[0];
                Vector2 direction = (monsterToThrow.transform.position - transform.position).normalized;
                var grenade = grenadePool.Get();
                grenade.ThrowByDirection(direction);
                yield return new WaitForSeconds(0.16f);
            }
            isThrowing = false;
        }

        void ThrowRandomSpots()
        {
            if (EntityManager.Instance == null) return;

            int count = TotalProjectileCount;
            var monsters = EntityManager.Instance.GetAllVisibleMonsters(count + 5);
            var randomMonsters = monsters.OrderBy(x => Random.value).Take(count).ToList();

            for (int i = 0; i < count; i++)
            {
                if (i >= randomMonsters.Count) break;
                var grenade = grenadePool.Get();
                grenade.ThrowToPosition(randomMonsters[i].transform.position);
            }
        }

        public void DespawnGrenade(Skill1015Grenade grenade)
        {
            grenadePool.Release(grenade);
        }

        public Skill1015DamageArea SpawnDamageArea(Vector2 position)
        {
            var area = damageAreaPool.Get();
            area.transform.position = position;
            return area;
        }

        public void DespawnDamageArea(Skill1015DamageArea area)
        {
            damageAreaPool.Release(area);
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
