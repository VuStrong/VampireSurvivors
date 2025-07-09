using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1015DamageArea : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effect;
        private Skill1015 skill;
        private float duration;
        private float radius;
        private float time;
        private float timeSinceLastDamage;

        public void Init(Skill1015 skill)
        {
            this.skill = skill;
        }

        public void Setup()
        {
            time = 0;
            timeSinceLastDamage = 0;
            duration = skill.FinalDuration;
            radius = skill.FinalArea;

            var shape = effect.shape;
            shape.radius = radius;
        }

        void Update()
        {
            time += Time.deltaTime;
            if (time >= duration)
            {
                skill.DespawnDamageArea(this);
                return;
            }

            timeSinceLastDamage += Time.deltaTime;
            if (timeSinceLastDamage >= 1)
            {
                Damage();
                timeSinceLastDamage = 0;
            }
        }

        void Damage()
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, radius, skill.Owner.DamageableLayerMask);

            foreach (var col in colliders)
            {
                if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) continue;
                skill.ApplyDamage(damageable);
            }
        }
    }
}
