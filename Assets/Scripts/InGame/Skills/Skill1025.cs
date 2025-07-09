using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1025: SummonRat - 
    /// Idle: X rats return to player and store the quantity, deal damage to enemies it go through --- 
    /// Moving: Stored rats go to the enemy and explode deal area damage
    /// </summary>
    public class Skill1025 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat cooldown;
        [SerializeField] private float ratSpeed;
        [SerializeField] private Skill1025Rat ratPrefab;
        [SerializeField] private TMP_Text ratCountText;

        private ObjectPool<Skill1025Rat> rats;
        private float counter;
        private int ratCount;

        public float FinalAreaDamage { get => area.Value * (1 + owner.stats.skillAreaBonus / 100); }
        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalCooldown { get => Mathf.Clamp(cooldown.Value * (1 - owner.stats.cooldownReduce / 100), 0.5f, cooldown.Value); }
        public float RatSpeed { get => ratSpeed; }

        protected override void Awake()
        {
            base.Awake();

            rats = new ObjectPool<Skill1025Rat>(
                () =>
                {
                    var rat = Instantiate(ratPrefab);
                    rat.Init(this);
                    return rat;
                },
                (rat) =>
                {
                    rat.gameObject.SetActive(true);
                },
                (rat) =>
                {
                    rat.gameObject.SetActive(false);
                },
                (rat) => Destroy(rat.gameObject),
                false
            );
        }

        void Update()
        {
            if (!Owned) return;

            counter += Time.deltaTime;
            if (counter >= FinalCooldown)
            {
                if (owner.IsMoving)
                {
                    SpawnRatTowardsEnemy();
                }
                else
                {
                    SpawnRatTowardsPlayer();
                }
                counter = 0;
            }
        }

        void SpawnRatTowardsPlayer()
        {
            int count = TotalProjectileCount;
            for (int i = 0; i < count; i++)
            {
                var rat = rats.Get();
                rat.transform.position = GetRandomPositionOffscreen();
                rat.Setup(false);
            }
        }

        void SpawnRatTowardsEnemy()
        {
            if (ratCount <= 0) return;

            int count = TotalProjectileCount;
            var monsters = EntityManager.Instance.GetAllVisibleMonsters(count);
            if (monsters.Count == 0) return;

            for (int i = 0; i < count; i++)
            {
                var rat = rats.Get();
                var offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                rat.transform.position = owner.transform.position + offset;
                rat.Setup(true, i < monsters.Count ? monsters[i] : monsters[^1]);

                ratCount--;
                if (ratCount == 0)
                    break;
            }

            ratCountText.text = ratCount.ToString();
        }

        public void StoreRat(Skill1025Rat rat)
        {
            rats.Release(rat);
            ratCount++;
            ratCountText.text = ratCount.ToString();
        }

        public void DespawnRat(Skill1025Rat rat)
        {
            rats.Release(rat);
        }

        Vector2 GetRandomPositionOffscreen()
        {
            Vector2 randomScreenPosition = new(Random.Range(-0.1f, 1.1f), Random.Range(-0.1f, 1.1f));

            if (Random.Range(0, 2) == 0)
            {
                randomScreenPosition.x = randomScreenPosition.x > 0.5f ? 1.1f : -0.1f;
            }
            else
            {
                randomScreenPosition.y = randomScreenPosition.y > 0.5f ? 1.1f : -0.1f;
            }

            return Camera.main.ViewportToWorldPoint(randomScreenPosition);
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
