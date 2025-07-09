using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1006: Projectile Speed - Increase projectile speed by X%
    /// </summary>
    public class Skill1006 : Skill
    {
        [SerializeField] private UpgradeableFloatStat projectileSpeed;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);
            
            owner.stats.projectileSpeedBonus += projectileSpeed.Value;
            projectileSpeed.OnUpgraded += OnUpgraded;
        }

        void OnUpgraded()
        {
            owner.stats.projectileSpeedBonus += projectileSpeed.CurrentUpgrade;
        }
    }
}
