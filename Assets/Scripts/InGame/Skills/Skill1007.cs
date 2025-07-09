using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1007: Cooldown - Reduce X% skill cooldown
    /// </summary>
    public class Skill1007 : Skill
    {
        [SerializeField] private UpgradeableFloatStat cooldown;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);
            
            owner.stats.cooldownReduce += cooldown.Value;
            cooldown.OnUpgraded += OnUpgraded;
        }

        void OnUpgraded()
        {
            owner.stats.cooldownReduce += cooldown.CurrentUpgrade;
        }
    }
}
