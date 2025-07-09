using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1016IceRock : MonoBehaviour
    {
        public static readonly float speed = 20f;
        private Skill1016 skill;
        private Vector2 destination;

        public void Init(Skill1016 skill)
        {
            this.skill = skill;
        }

        public void Setup(Vector2 destination)
        {
            this.destination = destination;
        }

        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, destination) <= 0.1f)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, 1, skill.Owner.DamageableLayerMask);
                foreach (var col in colliders)
                {
                    if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) continue;
                    skill.ApplyDamage(damageable);
                }

                skill.SpawnSlowArea(destination);
                skill.DespawnIceRock(this);
            }
        }
    }
}
