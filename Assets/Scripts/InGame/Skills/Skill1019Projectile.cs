using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1019Projectile : MonoBehaviour
    {
        [SerializeField] private float speed;
        private TrailRenderer trailRenderer;

        private Skill1019 skill;
        private Vector2 direction = Vector2.right;
        private float time;
        private bool collided;

        void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        public void Init(Skill1019 skill) => this.skill = skill;

        public void Setup(Vector2 direction)
        {
            this.direction = direction;
            time = 0;
            collided = false;
        }

        void Update()
        {
            time += Time.deltaTime;
            if (time < 2 && !collided)
            {
                transform.position += speed * Time.deltaTime * (Vector3)direction;
            }
            else if (!collided)
            {
                Release();
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if ((skill.Owner.DamageableLayerMask & (1 << collision.gameObject.layer)) != 0)
            {
                if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
                {
                    collided = true;
                    skill.ApplyDamage(damageable);
                    Release();
                }
            }
        }

        void Release()
        {
            skill.DespawnProjectile(this);
            trailRenderer.Clear();
        }
    }
}
