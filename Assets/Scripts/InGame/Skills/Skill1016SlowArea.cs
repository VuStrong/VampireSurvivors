using ProjectCode1.InGame.Monsters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public class Skill1016SlowArea : MonoBehaviour
    {
        [Range(0, 100)] public float slow;

        private Skill1016 skill;
        private float duration;
        private float time;

        public void Init(Skill1016 skill)
        {
            this.skill = skill;
        }

        public void Setup()
        {
            time = 0;
            duration = skill.FinalDuration;

            float radius = skill.FinalArea;
            transform.localScale = new Vector3(radius, radius, 1);
        }

        void Update()
        {
            time += Time.deltaTime;
            if (time >= duration)
            {
                skill.DespawnSlowArea(this);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<Monster>(out var monster))
            {
                monster.Slow(slow / 100);
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<Monster>(out var monster))
            {
                monster.Slow(0);
            }
        }
    }
}
