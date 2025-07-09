using ProjectCode1.InGame.Characters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1023SawBlade : MonoBehaviour
    {
        private Skill1023 skill;
        private Character owner;
        private float speed;
        private float size = 1;
        private Transform target;
        private Vector3 moveDirection;
        private bool ownerIsMoving;

        public void Init(Skill1023 skill)
        {
            this.skill = skill;
            owner = skill.Owner;
            speed = skill.Speed;

            moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        void Update()
        {
            // If player idle -> moving, set target to player
            // If player moving -> idle, set random moveDirection
            if (ownerIsMoving != owner.IsMoving)
            {
                ownerIsMoving = owner.IsMoving;
                if (ownerIsMoving)
                {
                    target = owner.transform;
                }
                else
                {
                    moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                }
            }

            // Check size of skill
            if (size != skill.FinalSize)
            {
                size = skill.FinalSize;
                transform.localScale = new Vector3(size, size, 1);
            }

            transform.RotateAround(transform.position, Vector3.back, Time.deltaTime * 1000);
            transform.position += speed * Time.deltaTime * moveDirection;

            if (ownerIsMoving)
            {
                CheckTarget();
            }
            else
            {
                CheckPosition();
            }
        }

        void CheckTarget()
        {
            Vector2 v = target.transform.position - transform.position;
            moveDirection = v.normalized;
            if (v.sqrMagnitude <= 0.1f)
            {
                if (target == owner.transform)
                {
                    var monster = EntityManager.Instance.GetClosestMonster(owner.transform.position);
                    target = monster != null ? monster.transform : owner.transform;
                }
                else
                {
                    target = owner.transform;
                }
            }
        }

        /// <summary>
        /// Bounce back if touched the edge of screen
        /// </summary>
        void CheckPosition()
        {
            Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
            Vector2 topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
            bool isOnScreen = (
                transform.position.x > bottomLeft.x &&
                transform.position.x < topRight.x &&
                transform.position.y > bottomLeft.y &&
                transform.position.y < topRight.y
            );

            if (!isOnScreen)
            {
                Vector2 pos = transform.position;
                Vector2 normal = default;

                if (pos.x <= bottomLeft.x)
                {
                    normal = Vector2.right;
                    pos.x = bottomLeft.x;
                }
                if (pos.x >= topRight.x)
                {
                    normal = Vector2.left;
                    pos.x = topRight.x;
                }
                if (pos.y >= topRight.y)
                {
                    normal = Vector2.down;
                    pos.y = topRight.y;
                }
                if (pos.y <= bottomLeft.y)
                {
                    normal = Vector2.up;
                    pos.y = bottomLeft.y;
                }

                moveDirection = Vector2.Reflect(moveDirection, normal);
                transform.position = pos;
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
