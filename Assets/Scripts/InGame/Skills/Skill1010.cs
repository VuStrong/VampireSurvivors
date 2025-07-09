using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1010: Pickup Range - Increase pickup radius by X%
    /// </summary>
    public class Skill1010 : Skill
    {
        [SerializeField] private UpgradeableFloatStat radius;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);
            
            owner.stats.pickupRange += radius.Value / 100 * owner.stats.pickupRange;
            radius.OnUpgraded += OnUpgraded;
        }

        void OnUpgraded()
        {
            owner.stats.pickupRange += radius.CurrentUpgrade / 100 * owner.stats.pickupRange;
        }
    }
}
