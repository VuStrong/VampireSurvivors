using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1026: Slash - Idle: Slash clockwise around player --- Moving: Slash in player look direction
    /// </summary>
    public class Skill1026 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private float knockback;

        private Skill1026Slash defaultSlash;
        private readonly List<Skill1026Slash> slashes = new();
        private float timeSinceLastAttack;
        private float currentAngle;
        private bool isSlashing;

        public float FinalAreaDamage { get => area.Value * (1 + owner.stats.skillAreaBonus / 100); }
        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }

        protected override void Awake()
        {
            base.Awake();

            defaultSlash = GetComponentInChildren<Skill1026Slash>();
            defaultSlash.Init(this);
            defaultSlash.gameObject.SetActive(false);
            slashes.Add(defaultSlash);
        }

        void Update()
        {
            if (!Owned) return;

            CheckSlashes();

            if (!isSlashing) timeSinceLastAttack += Time.deltaTime;
            if (timeSinceLastAttack >= 0.5f && !isSlashing)
            {
                timeSinceLastAttack = 0;

                if (owner.IsMoving)
                {
                    StartCoroutine(SlashInFront());
                }
                else
                {
                    StartCoroutine(SlashAround());
                }
            }
        }

        void CheckSlashes()
        {
            int count = TotalProjectileCount;
            if (count <= 0) count = 1;
            if (count == slashes.Count) return;

            if (slashes.Count < count)
            {
                int amountToAdd = count - slashes.Count;
                for (int i = 0; i < amountToAdd; i++)
                {
                    var slash = Instantiate(defaultSlash, transform, false);
                    slash.Init(this);
                    slash.gameObject.SetActive(false);
                    slashes.Add(slash);
                }
            }
            else
            {
                int amountToDestroy = slashes.Count - count;
                for (int i = 0; i < amountToDestroy; i++)
                {
                    int index = slashes.Count - 1;
                    Destroy(slashes[index].gameObject);
                    slashes.RemoveAt(index);
                }
            }
        }

        IEnumerator SlashInFront()
        {
            isSlashing = true;
            var littleDelay = new WaitForSeconds(0.2f);
            float finalArea = FinalAreaDamage;
            int count = slashes.Count;
            int i = 0;

            while (i < count)
            {
                Vector2 direction = owner.LookDirection;
                float angle = Vector2.Angle(Vector2.right, direction);
                if (direction.y < 0) angle = 360 - angle;

                slashes[i].gameObject.SetActive(true);
                slashes[i].transform.localRotation = Quaternion.Euler(0, 0, angle);
                slashes[i].transform.localScale = new Vector3(finalArea, 1, 1);
                yield return StartCoroutine(slashes[i].SlashCoroutine());
                yield return littleDelay;
                i++;
            }

            isSlashing = false;
        }

        IEnumerator SlashAround()
        {
            isSlashing = true;
            int count = slashes.Count;
            float finalArea = FinalAreaDamage;
            List<Coroutine> coroutines = new();

            for (int i = 0; i < count; i++)
            {
                float angle = i * (360 / count) + currentAngle;

                slashes[i].gameObject.SetActive(true);
                slashes[i].transform.localRotation = Quaternion.Euler(0, 0, angle);
                slashes[i].transform.localScale = new Vector3(finalArea, 1, 1);
                coroutines.Add(StartCoroutine(slashes[i].SlashCoroutine()));
            }

            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }

            currentAngle -= 20;
            isSlashing = false;
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable, knockback);
        }
    }
}
