using System;
using System.Collections;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1028Projectile : MonoBehaviour
    {
        private TrailRenderer trailRenderer;
        private Skill1028 skill;

        void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        public void Init(Skill1028 skill) => this.skill = skill;

        public IEnumerator ShootByDirection(Vector2 direction)
        {
            while (true)
            {
                transform.position += skill.FinalProjectileSpeed * Time.deltaTime * (Vector3)direction;
                yield return null;
            }
        }

        public void ClearTrail()
        {
            trailRenderer.Clear();
        }

        public void SetTrailTime(float time)
        {
            trailRenderer.time = time;
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
