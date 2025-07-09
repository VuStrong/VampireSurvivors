using System.Collections;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1018: Shield - 
    /// Idle: Generate shield absorb damage taken, regen when not take damage in X seconds --- 
    /// Moving: Shield deal damage when take damage
    /// </summary>
    public class Skill1018 : Skill, IDamageable
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat cooldown;
        [SerializeField] private float regenAmount;
        [SerializeField] private float timeToRegen;
        [SerializeField] private SpriteRenderer shieldSpriteRenderer;

        private float timeSinceLastShield;
        private int shieldStack;
        private float regenCounter;
        private bool regenActivated;

        public float FinalCooldown { get => Mathf.Clamp(cooldown.Value * (1 - owner.stats.cooldownReduce / 100), 1, cooldown.Value); }
        public Vector3 Position { get => transform.position; }

        void Update()
        {
            if (shieldStack <= 0)
            {
                timeSinceLastShield += Time.deltaTime;
                if (timeSinceLastShield >= FinalCooldown)
                {
                    GenerateShield();
                    timeSinceLastShield = 0;
                }
            }
            else
            {
                regenCounter += Time.deltaTime;
                if (regenCounter >= timeToRegen && !regenActivated)
                {
                    regenActivated = true;
                    StartCoroutine(RegenCoroutine());
                }
            }
        }

        void GenerateShield()
        {
            shieldStack++;
            shieldSpriteRenderer.enabled = true;
            owner.unitsTakeDamage.Add(this);
        }

        public void TakeDamage(float amount, Vector2 knockback = default, GameObject source = null, bool isCrit = false)
        {
            if (shieldStack == 0) return;
            shieldStack--;

            if (shieldStack == 0)
            {
                shieldSpriteRenderer.enabled = false;
                owner.unitsTakeDamage.Remove(this);
            }
            if (owner.IsMoving && source.TryGetComponent<IDamageable>(out var damageable))
            {
                ApplyDamage(damage.Value, damageable);
            }

            regenCounter = 0;
            regenActivated = false;
        }

        IEnumerator RegenCoroutine()
        {
            while (regenActivated)
            {
                owner.GainHealth(regenAmount);
                yield return new WaitForSeconds(1);
            }
        }
    }
}
