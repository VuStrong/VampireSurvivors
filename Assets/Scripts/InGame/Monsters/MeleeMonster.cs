using UnityEngine;

namespace ProjectCode1.InGame.Monsters
{
    public class MeleeMonster : Monster
    {
        protected float timeSinceLastAttack;

        protected override void Update()
        {
            base.Update();
            if (!alive) return;
            timeSinceLastAttack += Time.deltaTime;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!alive) return;

            Vector2 moveDirection = (target.position - transform.position).normalized;
            rb.linearVelocity += monsterBlueprint.speed * (1 - slowEffect) * Time.fixedDeltaTime * moveDirection;
        }

        void OnCollisionStay2D(Collision2D col)
        {
            if (
                alive &&
                (monsterBlueprint.damageableLayerMask & (1 << col.collider.gameObject.layer)) != 0 &&
                timeSinceLastAttack >= 1 / monsterBlueprint.attackSpeed)
            {
                if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) return;
                damageable.TakeDamage(monsterBlueprint.damage, source: gameObject);
                timeSinceLastAttack = 0;
            }
        }
    }
}
