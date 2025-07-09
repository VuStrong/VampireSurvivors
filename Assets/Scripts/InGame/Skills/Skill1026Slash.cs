using System.Collections;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1026Slash : MonoBehaviour
    {
        [SerializeField] private float slashTime;
        private Skill1026 skill;

        public void Init(Skill1026 skill) => this.skill = skill;

        public IEnumerator SlashCoroutine()
        {
            Vector3 startScale = new(0.1f, 1, 1);
            Vector3 endScale = transform.localScale;
            transform.localScale = startScale;

            float curTime = 0;
            while (curTime < slashTime)
            {
                float t = curTime / slashTime;
                transform.localScale = Vector2.LerpUnclamped(startScale, endScale, t);
                curTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = endScale;
            gameObject.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if ((skill.Owner.DamageableLayerMask & (1 << collider.gameObject.layer)) != 0)
            {
                if (collider.gameObject.TryGetComponent<IDamageable>(out var damageable))
                {
                    skill.ApplyDamage(damageable);
                }
            }
        }
    }
}
