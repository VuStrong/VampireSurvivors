using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1017: Area Damage - Idle: Create area floating around map deal damage --- Moving: Area stick at player position
    /// </summary>
    public class Skill1017 : Skill
    {
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private GameObject areaEffect;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float knockback;

        private bool ownerIsMoving = false;
        private bool isWandering = true;
        private float radius;
        private float timeSinceLastDamage;
        private Vector2 destination;

        public float FinalRadius { get => area.Value * (1 + owner.stats.skillAreaBonus / 100); }

        void Start()
        {
            destination = GetRandomPosition();
        }

        void Update()
        {
            if (!Owned) return;

            // Check owner moving
            if (ownerIsMoving != owner.IsMoving)
            {
                ownerIsMoving = owner.IsMoving;

                if (ownerIsMoving)
                {
                    StickWithPlayer();
                }
                else
                {
                    SwitchToWanderingMode();
                }
            }

            // Moving
            if (isWandering)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
                float distance = (destination - (Vector2)transform.position).sqrMagnitude;
                if (distance <= 0.1f)
                {
                    destination = GetRandomPosition();
                }
            }

            // Check radius
            if (radius != FinalRadius)
            {
                radius = FinalRadius;
                areaEffect.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
            }

            // Damage
            timeSinceLastDamage += Time.deltaTime;
            if (timeSinceLastDamage >= 0.3f)
            {
                Attack();
                timeSinceLastDamage = 0;
            }
        }

        void SwitchToWanderingMode()
        {
            transform.parent = null;
            isWandering = true;
        }

        void StickWithPlayer()
        {
            isWandering = false;
            transform.parent = owner.transform;
            transform.localPosition = Vector3.zero;
        }

        void Attack()
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, radius, owner.DamageableLayerMask);

            foreach (var col in colliders)
            {
                if (!col.gameObject.TryGetComponent<IDamageable>(out var damageable)) continue;
                ApplyDamage(damage.Value, damageable, knockback);
            }
        }

        Vector2 GetRandomPosition()
        {
            if (EntityManager.Instance == null)
            {
                Vector2 randomScreenPosition = new(Random.Range(0f, 1f), Random.Range(0f, 1f));
                return (Vector2)Camera.main.ViewportToWorldPoint(randomScreenPosition);
            }

            var monsters = EntityManager.Instance.GetAllVisibleMonsters(3);
            if (monsters.Count > 0)
            {
                return monsters[Random.Range(0, monsters.Count)].transform.position;
            }
            else
            {
                Vector2 randomScreenPosition = new(Random.Range(0f, 1f), Random.Range(0f, 1f));
                return (Vector2)Camera.main.ViewportToWorldPoint(randomScreenPosition);
            }
        }
    }
}
