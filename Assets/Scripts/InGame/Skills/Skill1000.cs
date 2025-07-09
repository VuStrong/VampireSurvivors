using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1000: Health - Increase X% max health
    /// </summary>
    public class Skill1000 : Skill
    {
        [SerializeField] private UpgradeableFloatStat health;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);

            owner.stats.health += health.Value / 100 * owner.stats.health;
            health.OnUpgraded += OnHealthUpgraded;
        }

        void OnHealthUpgraded()
        {
            owner.stats.health += health.CurrentUpgrade / 100 * owner.stats.health;
        }
    }
}
