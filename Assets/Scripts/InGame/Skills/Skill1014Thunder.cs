using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1014Thunder : MonoBehaviour
    {
        [SerializeField] private ParticleSystem thunderEffect;
        [SerializeField] private ParticleSystem areaEffect;
        private Skill1014 skill;

        public void Init(Skill1014 skill) => this.skill = skill;

        public void Strike(Vector2 position, float radius, float knockback = 0)
        {
            transform.position = position;
            thunderEffect.Play();

            var areaEffectMain = areaEffect.main;
            areaEffectMain.startSpeed = radius / areaEffectMain.startLifetime.constant;

            var colliders = Physics2D.OverlapCircleAll(position, radius, skill.Owner.DamageableLayerMask);
            foreach (var col in colliders)
            {
                if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) continue;

                skill.ApplyDamage(damageable, knockback);
            }
        }
    }
}
