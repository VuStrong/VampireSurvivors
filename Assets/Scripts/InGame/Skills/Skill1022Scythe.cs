using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1022Scythe : MonoBehaviour
    {
        private Skill1022 skill;
        private Rigidbody2D rb;
        private bool throwMode;

        public void Init(Skill1022 skill) => this.skill = skill;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Setup(bool throwMode, Vector2 force = default)
        {
            this.throwMode = throwMode;

            if (throwMode)
            {
                rb.gravityScale = 5;
                rb.AddForce(force);
            }
            else
            {
                rb.gravityScale = 0;
            }
        }

        void Update()
        {
            if (!throwMode) return;

            transform.RotateAround(transform.position, Vector3.back, Time.deltaTime * 200);

            if (!EntityManager.Instance.PositionOnScreen(transform.position))
            {
                skill.DespawnScythe(this);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if ((skill.Owner.DamageableLayerMask & (1 << collision.gameObject.layer)) != 0)
            {
                if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
                {
                    skill.ApplyDamage(damageable);
                }
            }
        }
    }
}
