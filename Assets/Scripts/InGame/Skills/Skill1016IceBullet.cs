using ProjectCode1.InGame.Monsters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1016IceBullet : MonoBehaviour
    {
        public static readonly float speed = 12f;
        [SerializeField] private ParticleSystem explodeEffect;
        [SerializeField] private Collider2D col;
        [Range(0, 100)] public float slow;
        private Skill1016 skill;
        private float explodeRadius;
        private Vector2 direction;
        private float time;
        private bool exploded;

        public void Init(Skill1016 skill) => this.skill = skill;

        public void Setup(Vector2 direction)
        {
            this.direction = direction;
            col.enabled = true;
            time = 0;
            exploded = false;
            transform.localPosition = Vector2.zero;
            transform.parent = null;
            explodeRadius = skill.FinalArea;
            var shape = explodeEffect.shape;
            shape.radius = explodeRadius;
        }

        void Update()
        {
            time += Time.deltaTime;
            if (time < 2 && !exploded)
            {
                transform.position += speed * Time.deltaTime * (Vector3)direction;
            }
            else if (!exploded)
            {
                Release();
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if ((skill.Owner.DamageableLayerMask & (1 << collision.gameObject.layer)) != 0)
            {
                exploded = true;
                col.enabled = false;
                explodeEffect.Play();
                var colliders = Physics2D.OverlapCircleAll(transform.position, explodeRadius, skill.Owner.DamageableLayerMask);

                foreach (var col in colliders)
                {
                    if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) continue;
                    skill.ApplyDamage(damageable);
                    if (damageable is Monster monster)
                    {
                        monster.Slow(slow / 100, 2);
                    }
                }

                Invoke(nameof(Release), 0.2f);
            }
        }

        void Release()
        {
            skill.DespawnIceBullet(this);
        }
    }
}