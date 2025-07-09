using System.Collections;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1015Grenade : MonoBehaviour
    {
        [SerializeField] private Collider2D col;
        [SerializeField] private ParticleSystem explodeEffect;
        private Skill1015 skill;
        private bool exploded;
        private float explodeRadius;
        public static readonly float speed = 12f;

        public void Init(Skill1015 skill) => this.skill = skill;

        public void Setup()
        {
            transform.localPosition = Vector2.zero;
            exploded = false;
            explodeRadius = skill.FinalArea;
            var shape = explodeEffect.shape;
            shape.radius = explodeRadius;
        }

        public void ThrowToPosition(Vector2 position)
        {
            transform.parent = null;
            col.enabled = false;
            StartCoroutine(ThrowToPositionCoroutine(position));
        }

        public void ThrowByDirection(Vector2 direction)
        {
            transform.parent = null;
            col.enabled = true;
            StartCoroutine(ThrowByDirectionCoroutine(direction));
        }

        IEnumerator ThrowToPositionCoroutine(Vector2 position)
        {
            while (Vector3.Distance(transform.position, position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
                yield return null;
            }

            Damage();
            skill.SpawnDamageArea(transform.position);
            yield return new WaitForSeconds(0.2f);
            skill.DespawnGrenade(this);
        }

        IEnumerator ThrowByDirectionCoroutine(Vector2 direction)
        {
            float t = 0;
            while (t < 2 && !exploded)
            {
                transform.position += speed * Time.deltaTime * (Vector3)direction;
                yield return null;
                t += Time.deltaTime;
            }

            if (exploded)
            {
                Damage();
                yield return new WaitForSeconds(0.2f);
                skill.DespawnGrenade(this);
            }
            else
            {
                skill.DespawnGrenade(this);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if ((skill.Owner.DamageableLayerMask & (1 << collision.gameObject.layer)) != 0)
            {
                exploded = true;
                col.enabled = false;
            }
        }

        void Damage()
        {
            explodeEffect.Play();
            var colliders = Physics2D.OverlapCircleAll(transform.position, explodeRadius, skill.Owner.DamageableLayerMask);

            foreach (var col in colliders)
            {
                if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) continue;
                skill.ApplyDamage(damageable);
            }
        }
    }
}
