using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1030Projectile : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        private float speed;
        private Skill1030 skill;

        private Vector3 startPosition;
        private Vector3 destination;
        private float duration;
        private float ellipseRadiusY = 1f;
        private bool lowerArc;
        private float timer = 0f;

        public void Init(Skill1030 skill) => this.skill = skill;

        void OnEnable()
        {
            timer = 0;
        }

        void Update()
        {
            // ellipse arc moving
            if (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);

                Vector3 center = (startPosition + destination) * 0.5f;
                float radiusX = Vector3.Distance(startPosition, destination) * 0.5f;
                float angle = Mathf.Lerp(Mathf.PI, 0f, t);
                float x = Mathf.Cos(angle) * radiusX;
                float y = Mathf.Sin(angle) * ellipseRadiusY * (lowerArc ? -1 : 1);
                Vector3 localPos = new(x, y, 0f);
                Vector3 dir = destination - startPosition;
                float rotationAngleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAngleDeg);
                Vector3 rotated = rotation * localPos;

                transform.position = center + rotated;
            }
            else
            {
                skill.DespawnProjectile(this);
            }
        }

        public void SetDestination(Vector3 destination, float ellipseRadiusY = 1, bool lowerArc = false)
        {
            this.destination = destination;
            startPosition = transform.position;
            timer = 0;
            duration = Vector3.Distance(startPosition, destination) / speed;
            duration = Mathf.Clamp(duration, 0.4f, duration);
            this.ellipseRadiusY = ellipseRadiusY;
            this.lowerArc = lowerArc;
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
