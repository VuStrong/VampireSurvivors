using System.Collections.Generic;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1023: SawBlade - 
    /// Idle: X SawBlades move around deal damage, bounces back when hits the edge of screen --- 
    /// Moving: SawBlade move to nearest enemy and return to player
    /// </summary>
    public class Skill1023 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat size;
        [SerializeField] private Skill1023SawBlade sawBladePrefab;
        [SerializeField] private float speed;
        private readonly List<Skill1023SawBlade> sawBlades = new();

        public float Speed { get => speed; }
        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalSize { get => size.Value * (1 + owner.stats.skillAreaBonus / 100); }

        void Update()
        {
            CheckSawBlades();
        }

        void CheckSawBlades()
        {
            int count = TotalProjectileCount;
            if (sawBlades.Count == count) return;

            // add or remove saw blades
            if (sawBlades.Count < count)
            {
                int amountToAdd = count - sawBlades.Count;
                for (int i = 0; i < amountToAdd; i++)
                {
                    var sawBlade = Instantiate(sawBladePrefab, transform.position, Quaternion.identity);
                    sawBlade.Init(this);
                    sawBlades.Add(sawBlade);
                }
            }
            else
            {
                int amountToDestroy = sawBlades.Count - count;
                for (int i = 0; i < amountToDestroy; i++)
                {
                    int index = sawBlades.Count - 1;
                    Destroy(sawBlades[index].gameObject);
                    sawBlades.RemoveAt(index);
                }
            }
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
