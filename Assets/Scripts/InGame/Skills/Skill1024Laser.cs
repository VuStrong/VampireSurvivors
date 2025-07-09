using System.Collections.Generic;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1024Laser : MonoBehaviour
    {
        private Skill1024 skill;
        private LineRenderer lineRenderer;
        private EdgeCollider2D col;
        private ParticleSystem laserHitEffect;
        private bool disabled;

        public bool Disabled { get => disabled; }

        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            col = GetComponent<EdgeCollider2D>();
            laserHitEffect = GetComponentInChildren<ParticleSystem>();
        }

        public void Init(Skill1024 skill) => this.skill = skill;

        public void Setup()
        {
            lineRenderer.positionCount = 2;
            col.enabled = true;
            disabled = false;
        }

        public void SetPoints(Vector2 startPoint, Vector2 endPoint)
        {
            if (disabled) return;

            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);
            col.SetPoints(new List<Vector2>() { startPoint, endPoint });

            laserHitEffect.transform.position = endPoint;
            if (!laserHitEffect.isPlaying)
            {
                laserHitEffect.Play();
            }
        }

        public void Disable()
        {
            lineRenderer.positionCount = 0;
            col.enabled = false;
            laserHitEffect.Stop();
            disabled = true;
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
