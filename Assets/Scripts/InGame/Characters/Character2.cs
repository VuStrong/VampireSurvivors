using UnityEngine;

namespace ProjectCode1.InGame.Characters
{
    /// <summary>
    /// Character Effect: Gain 45% skill cooldown the first time health drop below 40%
    /// </summary>
    public class Character2 : Character
    {
        private bool effectActivated = false;

        public override void TakeDamage(float amount, Vector2 knockback = default, GameObject source = null, bool isCrit = false)
        {
            base.TakeDamage(amount, knockback, source, isCrit);

            if (!effectActivated && currentHealth <= stats.health * 0.4)
            {
                effectActivated = true;
                stats.cooldownReduce += 40;
            }
        }
    }
}