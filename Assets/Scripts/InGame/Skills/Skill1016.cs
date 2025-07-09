using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1016: Ice Rock - 
    /// Idle: X Ice rock drop from sky to random spot deal area damage and create slow area in X seconds --- 
    /// Moving: Fire X ice rock to nearest enemies deal area damage and slow enemies
    /// </summary>
    public class Skill1016 : Skill
    {
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat duration;
        [SerializeField] private UpgradeableFloatStat cooldown;
        [SerializeField] private Skill1016IceRock iceRockPrefab;
        [SerializeField] private Skill1016SlowArea slowAreaPrefab;
        [SerializeField] private Skill1016IceBullet iceBulletPrefab;

        private ObjectPool<Skill1016IceRock> iceRockPool;
        private ObjectPool<Skill1016SlowArea> slowAreaPool;
        private ObjectPool<Skill1016IceBullet> iceBulletPool;
        private float timeSinceLastAttack;
        private bool isShooting;
        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalDuration { get => owner.stats.skillDurationBonus + duration.Value; }
        public float FinalCooldown { get => Mathf.Clamp(cooldown.Value * (1 - owner.stats.cooldownReduce / 100), 1, cooldown.Value); }
        public float FinalArea { get => area.Value * (1 + owner.stats.skillAreaBonus / 100); }

        protected override void Awake()
        {
            base.Awake();

            iceRockPool = new(
                () =>
                {
                    var iceRock = Instantiate(iceRockPrefab, transform, false);
                    iceRock.Init(this);
                    return iceRock;
                },
                (iceRock) =>
                {
                    iceRock.gameObject.SetActive(true);
                    iceRock.transform.parent = null;
                },
                (iceRock) =>
                {
                    iceRock.gameObject.SetActive(false);
                    iceRock.transform.parent = transform;
                },
                (iceRock) => Destroy(iceRock.gameObject),
                false,
                defaultCapacity: 6
            );

            slowAreaPool = new(
                () =>
                {
                    var area = Instantiate(slowAreaPrefab, transform, false);
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

            iceBulletPool = new(
                () =>
                {
                    var bullet = Instantiate(iceBulletPrefab, transform, false);
                    bullet.Init(this);
                    return bullet;
                },
                (bullet) =>
                {
                    bullet.gameObject.SetActive(true);
                },
                (bullet) =>
                {
                    bullet.gameObject.SetActive(false);
                    bullet.transform.parent = transform;
                },
                (bullet) => Destroy(bullet.gameObject),
                false,
                defaultCapacity: 6
            );
        }

        void Update()
        {
            if (!isShooting) timeSinceLastAttack += Time.deltaTime;
            if (timeSinceLastAttack >= FinalCooldown)
            {
                if (owner.IsMoving)
                {
                    StartCoroutine(ShootIceBullets());
                }
                else
                {
                    CreateIceRocks();
                }

                timeSinceLastAttack = 0;
            }
        }

        void CreateIceRocks()
        {
            int count = TotalProjectileCount;
            var monsters = EntityManager.Instance.GetAllVisibleMonsters(count + 5);
            var randomMonsters = monsters.OrderBy(x => Random.value).Take(count).ToList();

            for (int i = 0; i < count; i++)
            {
                if (i >= randomMonsters.Count) break;
                var iceRock = iceRockPool.Get();
                iceRock.transform.position = GetRandomPositionOffscreen();
                iceRock.Setup(randomMonsters[i].transform.position);
            }
        }

        IEnumerator ShootIceBullets()
        {
            int count = TotalProjectileCount;
            var monsters = EntityManager.Instance.GetClosestMonsters(transform.position, count);
            if (monsters.Count == 0) yield break;

            isShooting = true;
            for (int i = 0; i < count; i++)
            {
                var monsterToThrow = i < monsters.Count ? monsters[i] : monsters[0];
                Vector2 direction = (monsterToThrow.transform.position - transform.position).normalized;
                var bullet = iceBulletPool.Get();
                bullet.Setup(direction);
                yield return new WaitForSeconds(0.16f);
            }
            isShooting = false;
        }

        Vector2 GetRandomPositionOffscreen()
        {
            Vector2 randomScreenPosition = new(Random.Range(0, 1), 1.1f);

            return Camera.main.ViewportToWorldPoint(randomScreenPosition);
        }

        public void DespawnIceRock(Skill1016IceRock iceRock)
        {
            iceRockPool.Release(iceRock);
        }

        public Skill1016SlowArea SpawnSlowArea(Vector2 position)
        {
            var slowArea = slowAreaPool.Get();
            slowArea.transform.position = position;
            return slowArea;
        }

        public void DespawnSlowArea(Skill1016SlowArea area)
        {
            slowAreaPool.Release(area);
        }

        public void DespawnIceBullet(Skill1016IceBullet bullet)
        {
            iceBulletPool.Release(bullet);
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
