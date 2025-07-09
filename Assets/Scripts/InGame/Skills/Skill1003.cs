using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1003: Armor - Reduce X damage taken
    /// </summary>
    public class Skill1003 : Skill
    {
        [SerializeField] private UpgradeableIntStat armor;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);

            owner.stats.armor += armor.Value;
            armor.OnUpgraded += OnArmorUpgraded;
        }

        void OnArmorUpgraded()
        {
            owner.stats.armor += armor.CurrentUpgrade;
        }
    }
}
