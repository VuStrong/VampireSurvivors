using UnityEngine;

namespace ProjectCode1.InGame
{
    /// <summary>
    /// Interface for all damageable units
    /// </summary>
    public interface IDamageable
    {
        public Vector3 Position { get; }

        public void TakeDamage(float amount, Vector2 knockback = default, GameObject source = null, bool isCrit = false);
    }
}
