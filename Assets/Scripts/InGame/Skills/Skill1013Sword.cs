using System.Collections;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1013Sword : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Skill1013 skill;

        public void Init(Skill1013 skill) => this.skill = skill;

        public IEnumerator StrikeCoroutine(float time)
        {
            Vector3 startScale = new(0.1f, 0.1f, 1);
            Vector3 endScale = transform.localScale;
            transform.localScale = startScale;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            float curTime = 0;
            while (curTime < time)
            {
                float t = curTime / time;
                transform.localScale = Vector2.LerpUnclamped(startScale, endScale, t);
                spriteRenderer.color = new Color(1f, 1f, 1f, t);

                curTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = endScale;
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }

        public void SetOpacity(float opacity)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, opacity);
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
