using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1009: Duration - Increase skill effect duration by X second
    /// </summary>
    public class Skill1009 : Skill
    {
        [SerializeField] private UpgradeableFloatStat duration;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);
            
            owner.stats.skillDurationBonus += duration.Value;
            duration.OnUpgraded += OnUpgraded;
        }

        void OnUpgraded()
        {
            owner.stats.skillDurationBonus += duration.CurrentUpgrade;
        }
    }
}
