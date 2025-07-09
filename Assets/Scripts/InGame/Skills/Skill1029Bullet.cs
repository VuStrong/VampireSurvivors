using System.Collections.Generic;
using ProjectCode1.InGame.Monsters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1029Bullet : MonoBehaviour
    {
        [HideInInspector] public float speed;
        [HideInInspector] public int maxBounce;
        private TrailRenderer trailRenderer;
        private Skill1029 skill;
        private int currentTargetIndex;
        private IReadOnlyList<Monster> targets;
        private bool isReturning;
        private int bounceCount;

        public void Init(Skill1029 skill)
        {
            this.skill = skill;
            targets = skill.VisibleMonsters;
        }

        void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        void OnEnable()
        {
            if (skill != null && skill.Owned)
            {
                var position = skill.Owner.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                transform.position = position;
                trailRenderer.Clear();
            }
        }

        void Update()
        {
            MoveAndCheckTarget();
        }

        void MoveAndCheckTarget()
        {
            if (targets.Count == 0)
                return;
            else if (currentTargetIndex >= targets.Count)
                currentTargetIndex = 0;

            Monster target = targets[currentTargetIndex];
            Vector3 v;

            if (isReturning)
            {
                v = skill.Owner.transform.position - transform.position;
            }
            else
            {
                v = target.transform.position - transform.position;
            }

            transform.position += speed * Time.deltaTime * v.normalized;

            float distance = v.sqrMagnitude;
            if (distance <= 0.01f)
            {
                if (isReturning)
                {
                    isReturning = false;
                }
                else if (targets.Count == 1)
                {
                    isReturning = true;
                }
                else
                {
                    bounceCount++;
                    if (maxBounce > 0 && bounceCount >= maxBounce)
                    {
                        isReturning = true;
                        bounceCount = 0;
                    }
                    ChangeTargetIndex();
                }
            }
            else if (!target.Alive)
            {
                ChangeTargetIndex();
            }
        }

        void ChangeTargetIndex()
        {
            currentTargetIndex = Random.Range(0, targets.Count);
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
