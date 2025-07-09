using System;
using System.Collections;
using ProjectCode1.Blueprints;
using UnityEngine;

namespace ProjectCode1.InGame.DestructibleUnits
{
    /// <summary>
    /// Destructible units will drop collectibles when destroyed.
    /// </summary>
    public class DestructibleUnit : MonoBehaviour, IDamageable
    {
        private static WaitForSeconds hitAnimationDelay;

        [SerializeField] protected DestructibleUnitBlueprint blueprint;

        protected float currentHealth;
        protected SpriteRenderer spriteRenderer;
        protected Coroutine hitAnimationCoroutine;

        public Action OnDestroyed;
        public Vector3 Position { get => transform.position; }

        void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            hitAnimationDelay ??= new WaitForSeconds(0.15f);
        }

        public void Setup(DestructibleUnitBlueprint blueprint = null)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (blueprint != null)
                this.blueprint = blueprint;

            currentHealth = this.blueprint.health;
            spriteRenderer.sprite = this.blueprint.sprite;
        }

        public void TakeDamage(float amount, Vector2 knockback = default, GameObject source = null, bool isCrit = false)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                DropCollectable();
                Destroy(gameObject);
                OnDestroyed?.Invoke();
            }
            else
            {
                if (hitAnimationCoroutine != null) StopCoroutine(hitAnimationCoroutine);
                hitAnimationCoroutine = StartCoroutine(HitAnimation());
            }
        }

        private IEnumerator HitAnimation()
        {
            spriteRenderer.color = Color.red;
            yield return hitAnimationDelay;
            spriteRenderer.color = Color.white;
        }

        protected virtual void DropCollectable()
        {
            float rand = UnityEngine.Random.value * 100;
            float accum = 0f;

            for (int i = 0; i < blueprint.drops.Length; i++)
            {
                accum += blueprint.drops[i].chance;
                if (rand <= accum)
                {
                    EntityManager.Instance.SpawnCollectable(blueprint.drops[i].item, transform.position);
                    break;
                }
            }
        }
    }
}
