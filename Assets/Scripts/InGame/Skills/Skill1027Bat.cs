using ProjectCode1.InGame.Monsters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1027Bat : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Skill1027 skill;
        private Monster targetMonster;
        private bool isReturning;
        private bool returned = true;
        private float timeSinceLastAttack;
        private float scaleXMultiplier = 1;
        private float defaultScaleX;
        private int waveFrequency;
        private float waveAmplitude;

        public bool Returned { get => returned; }

        void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            defaultScaleX = spriteRenderer.transform.localScale.x;
        }

        void OnEnable()
        {
            isReturning = false;
            returned = false;
            waveFrequency = Random.Range(1, 5);
            waveAmplitude = Random.Range(0.5f, 1.5f);
        }

        public void Init(Skill1027 skill) => this.skill = skill;

        void Update()
        {
            Animate();

            Vector3 v;
            if (!isReturning)
            {
                if (targetMonster == null || !targetMonster.Alive)
                {
                    FindTarget();
                }

                if (targetMonster != null)
                {
                    v = targetMonster.transform.position - transform.position;
                }
                else
                {
                    return;
                }
            }
            else
            {
                v = skill.Owner.transform.position - transform.position;
                if (v.sqrMagnitude < 0.1f)
                {
                    returned = true;
                    skill.DespawnBat(this);
                    return;
                }
            }

            v.Normalize();
            Vector3 perpendicular = new(-v.y, v.x);
            float waveOffset = Mathf.Sin(Time.time * Mathf.PI * waveFrequency) * waveAmplitude;
            transform.position += Time.deltaTime * skill.BatSpeed * (v + perpendicular * waveOffset);
            timeSinceLastAttack += Time.deltaTime;
        }

        public void SetTarget(Monster targetMonster)
        {
            this.targetMonster = targetMonster;
        }

        public void Return()
        {
            isReturning = true;
        }

        void Animate()
        {
            spriteRenderer.transform.localScale = new Vector3(
                spriteRenderer.transform.localScale.x + scaleXMultiplier * Time.deltaTime,
                spriteRenderer.transform.localScale.y,
                spriteRenderer.transform.localScale.z
            );

            if (spriteRenderer.transform.localScale.x >= defaultScaleX + 0.1f)
            {
                scaleXMultiplier = -1;
            }
            else if (spriteRenderer.transform.localScale.x <= defaultScaleX)
            {
                scaleXMultiplier = 1;
            }
        }

        void FindTarget()
        {
            var randomMonsters = EntityManager.Instance.GetAllVisibleMonsters(1);
            if (randomMonsters.Count > 0)
            {
                targetMonster = randomMonsters[0];
            }
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (
                (skill.Owner.DamageableLayerMask & (1 << collision.gameObject.layer)) != 0 &&
                timeSinceLastAttack >= 0.5f
            )
            {
                if (collision.gameObject.TryGetComponent<Monster>(out var monster))
                {
                    skill.ApplyDamage(monster);
                    targetMonster = monster;
                }
                timeSinceLastAttack = 0;
            }
        }
    }
}
