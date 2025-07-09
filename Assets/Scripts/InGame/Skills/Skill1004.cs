using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1004: Damage - Increase X% skill damage
    /// </summary>
    public class Skill1004 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);
            
            owner.stats.skillDamageBonus += damage.Value;
            damage.OnUpgraded += OnDamageUpgraded;
        }

        void OnDamageUpgraded()
        {
            owner.stats.skillDamageBonus += damage.CurrentUpgrade;
        }
    }
}
