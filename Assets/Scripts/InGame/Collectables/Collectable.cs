using System.Collections;
using ProjectCode1.InGame.Characters;
using ProjectCode1.Utils;
using UnityEngine;

namespace ProjectCode1.InGame.Collectables
{
    /// <summary>
    /// Base class for all collectable items in game
    /// </summary>
    public abstract class Collectable : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If false, it will not fly to player. Only be collected if player touch it")]
        protected bool canFly;
        
        protected Collider2D col;
        protected bool isFlying;

        public bool CanFly { get => canFly; }

        protected virtual void Awake()
        {
            if (col == null) col = GetComponent<Collider2D>();
        }

        public void Setup(Vector2 position)
        {
            if (col == null) col = GetComponent<Collider2D>();
            col.enabled = true;
            isFlying = false;
            transform.position = position;
        }

        protected abstract void OnCollected(Character collector);

        public void FlyToCollector(Character collector)
        {
            if (canFly && !isFlying)
            {
                StartCoroutine(FlyToCollectorCoroutine(collector));
            }
        }

        private IEnumerator FlyToCollectorCoroutine(Character collector)
        {
            isFlying = true;
            col.enabled = false;

            float speed = 5f;
            Vector3 v = collector.transform.position - transform.position;
            while (v.sqrMagnitude > 0.1f)
            {
                transform.position += speed * Time.deltaTime * v.normalized;
                yield return null;
                v = collector.transform.position - transform.position;
                if (speed < 9f) speed += 2f * Time.deltaTime;
            }

            OnCollected(collector);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            var collector = collision.gameObject;
            if (canFly && collector.CompareTag(GameObjectTags.PlayerCollectable))
            {
                FlyToCollector(collector.GetComponentInParent<Character>());
            }
            else if (!canFly && collector.CompareTag(GameObjectTags.Player))
            {
                OnCollected(collector.GetComponent<Character>());
            }
        }
    }
}
