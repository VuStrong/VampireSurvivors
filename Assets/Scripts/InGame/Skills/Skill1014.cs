using System.Collections.Generic;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1014: Thunder - Idle: Thunder strike to random spot deal area damage
    /// </summary>
    public class Skill1014 : Skill
    {
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat cooldown;
        [SerializeField] private Skill1014Thunder thunderPrefab;

        private float timeSinceLastStrike;
        private readonly List<Skill1014Thunder> thunders = new();
        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalCooldown { get => Mathf.Clamp(cooldown.Value * (1 - owner.stats.cooldownReduce / 100), .3f, cooldown.Value); }

        void Start()
        {
            CheckThunders();
        }

        void Update()
        {
            CheckThunders();

            timeSinceLastStrike += Time.deltaTime;
            if (timeSinceLastStrike >= FinalCooldown)
            {
                StrikeRandomSpot();
                timeSinceLastStrike = 0;
            }
        }

        void CheckThunders()
        {
            int count = TotalProjectileCount;
            if (thunders.Count == count) return;

            if (thunders.Count < count)
            {
                int amountToAdd = count - thunders.Count;
                for (int i = 0; i < amountToAdd; i++)
                {
                    var thunder = Instantiate(thunderPrefab, transform, false);
                    thunder.Init(this);
                    thunders.Add(thunder);
                }
            }
            else
            {
                int amountToDestroy = thunders.Count - count;
                for (int i = 0; i < amountToDestroy; i++)
                {
                    int index = thunders.Count - 1;
                    Destroy(thunders[index].gameObject);
                    thunders.RemoveAt(index);
                }
            }
        }

        void StrikeRandomSpot()
        {
            if (EntityManager.Instance == null) return;

            int count = thunders.Count;
            var monsters = EntityManager.Instance.GetAllVisibleMonsters(count);
            float finalRadius = area.Value * (1 + owner.stats.skillAreaBonus / 100);

            for (int i = 0; i < count; i++)
            {
                if (i >= monsters.Count) break;
                thunders[i].Strike(monsters[i].transform.position, finalRadius);
            }
        }

        public void ApplyDamage(IDamageable damageable, float knockback = 0)
        {
            ApplyDamage(damage.Value, damageable, knockback);
        }
    }
}
