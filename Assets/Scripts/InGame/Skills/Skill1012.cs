using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1012: Greed - Increase coin received by X%
    /// </summary>
    public class Skill1012 : Skill
    {
        [SerializeField] private UpgradeableFloatStat greed;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);

            owner.stats.greed += greed.Value;
            greed.OnUpgraded += OnUpgraded;
        }

        void OnUpgraded()
        {
            owner.stats.greed += greed.CurrentUpgrade;
        }
    }
}