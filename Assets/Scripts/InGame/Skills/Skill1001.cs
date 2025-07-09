using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1001: Regen, Recovery X health per second
    /// </summary>
    public class Skill1001 : Skill
    {
        [SerializeField] private UpgradeableFloatStat regen;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);

            owner.stats.recoveryPerSecond += regen.Value;
            regen.OnUpgraded += OnRegenUpgraded;
        }

        void OnRegenUpgraded()
        {
            owner.stats.recoveryPerSecond += regen.CurrentUpgrade;
        }
    }
}