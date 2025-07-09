using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1008: Area - Increase area of skills by X%
    /// </summary>
    public class Skill1008 : Skill
    {
        [SerializeField] private UpgradeableFloatStat area;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);
            
            owner.stats.skillAreaBonus += area.Value;
            area.OnUpgraded += OnUpgraded;
        }

        void OnUpgraded()
        {
            owner.stats.skillAreaBonus += area.CurrentUpgrade;
        }
    }
}
