using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1002: Speed - Move X% faster
    /// </summary>
    public class Skill1002 : Skill
    {
        [SerializeField] private UpgradeableFloatStat speed;

        public override void Init(Character character)
        {
            if (Owned) return;
            base.Init(character);

            owner.stats.speed += speed.Value / 100 * owner.stats.speed;
            speed.OnUpgraded += OnSpeedUpgraded;
        }

        void OnSpeedUpgraded()
        {
            owner.stats.speed += speed.CurrentUpgrade / 100 * owner.stats.speed;
        }
    }
}
