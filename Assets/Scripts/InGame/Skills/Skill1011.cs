using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1011: Experience - Increase experience received by X%
    /// </summary>
    public class Skill1011 : Skill
    {
        [SerializeField] private UpgradeableFloatStat expBonus;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);

            owner.stats.experienceBonus += expBonus.Value;
            expBonus.OnUpgraded += OnUpgraded;
        }

        void OnUpgraded()
        {
            owner.stats.experienceBonus += expBonus.CurrentUpgrade;
        }
    }
}
