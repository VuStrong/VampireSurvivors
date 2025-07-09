using System.Collections;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1020: Drill - 
    /// Idle: Draw circle and deal damage inside when finish drawing --- 
    /// Moving: Leave a line behind deal damage to enemies walk in
    /// </summary>
    public class Skill1020 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat speed;
        [SerializeField] private float radius;
        [SerializeField] private LineRenderer idleLineRenderer;

        private bool isDrawing;
        public float FinalSpeed { get => speed.Value * (1 + owner.stats.projectileSpeedBonus / 100); }

        void Update()
        {
            if (owner.IsMoving)
            {

            }
            else if (!isDrawing)
            {
                isDrawing = true;
                StartCoroutine(DrawAndDamage());
            }
        }

        IEnumerator DrawAndDamage()
        {
            float finalSpeed = FinalSpeed;
            Vector2 center = owner.transform.position;
            Vector2 currentPoint = center + Vector2.right * radius;
            Vector2 lastPoint = currentPoint;
            float currentAngle = 0;
            float targetAngle = Mathf.Deg2Rad * 360;

            idleLineRenderer.positionCount = 1;
            idleLineRenderer.SetPosition(0, currentPoint);

            // Draw
            while (currentAngle <= targetAngle)
            {
                Vector2 offset = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * radius;
                currentPoint = center + offset;

                if (Vector2.Distance(lastPoint, currentPoint) >= 0.1f)
                {
                    idleLineRenderer.positionCount++;
                    idleLineRenderer.SetPosition(idleLineRenderer.positionCount - 1, currentPoint);
                    lastPoint = currentPoint;
                }

                yield return null;
                currentAngle += finalSpeed * Time.deltaTime;
            }

            // Deal Damage
            var colliders = Physics2D.OverlapCircleAll(center, radius, owner.DamageableLayerMask);
            foreach (var col in colliders)
            {
                if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) continue;
                ApplyDamage(damageable);
            }

            idleLineRenderer.positionCount = 0;
            isDrawing = false;
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
