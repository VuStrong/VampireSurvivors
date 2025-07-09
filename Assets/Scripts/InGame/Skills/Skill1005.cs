using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1005: Projectile - Projectile +X
    /// </summary>
    public class Skill1005 : Skill
    {
        [SerializeField] private UpgradeableIntStat count;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);
            
            owner.stats.projectileCountBonus += count.Value;
            count.OnUpgraded += OnCountUpgraded;
        }

        void OnCountUpgraded()
        {
            owner.stats.projectileCountBonus += count.CurrentUpgrade;
        }
    }
}
