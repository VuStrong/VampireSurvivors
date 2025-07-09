using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1022: Scythe - Idle: Scythe attack on player sides, max 2 --- Moving: Throw scythe up and fall down on screen
    /// </summary>
    public class Skill1022 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableFloatStat cooldown;
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private Skill1022Scythe scythePrefab;
        [SerializeField] private float throwForce;

        private ObjectPool<Skill1022Scythe> scythePool;
        private float timeSinceLastAttack;

        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalCooldown { get => cooldown.Value * (1 - owner.stats.cooldownReduce / 100); }

        protected override void Awake()
        {
            base.Awake();

            scythePool = new ObjectPool<Skill1022Scythe>(
                () =>
                {
                    var scythe = Instantiate(scythePrefab, transform, false);
                    scythe.Init(this);
                    return scythe;
                },
                (scythe) =>
                {
                    scythe.gameObject.SetActive(true);
                    scythe.transform.parent = null;
                    scythe.transform.SetPositionAndRotation(owner.transform.position, Quaternion.Euler(0, 0, 0));
                    
                    float size = area.Value * (1 + owner.stats.skillAreaBonus / 100);
                    scythe.transform.localScale = new Vector3(size, size, 1);
                },
                (scythe) =>
                {
                    scythe.gameObject.SetActive(false);
                    scythe.transform.parent = transform;
                },
                (scythe) => Destroy(scythe.gameObject),
                false,
                defaultCapacity: 4
            );
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (timeSinceLastAttack >= FinalCooldown)
            {
                if (owner.IsMoving)
                {
                    StartCoroutine(ThrowScythes());
                }
                else
                {
                    StartCoroutine(SlashScythes());
                }

                timeSinceLastAttack = 0;
            }
        }

        IEnumerator ThrowScythes()
        {
            int count = TotalProjectileCount;
            var delay = new WaitForSeconds(0.1f);
            int i = 0;

            while (i < count)
            {
                var scythe = SpawnScythe();
                scythe.Setup(true, Vector2.up * throwForce);
                yield return delay;
                i++;
            }
        }

        IEnumerator SlashScythes()
        {
            int count = Mathf.Clamp(TotalProjectileCount, 0, 2);
            if (count == 0) yield break;

            List<Skill1022Scythe> scythes = new();
            {
                for (int i = 0; i < count; i++)
                {
                    var scythe = SpawnScythe();
                    scythe.Setup(false);
                    scythes.Add(scythe);
                }
            }

            bool right = owner.LookDirection.x >= 0;
            float angleZ = 0;
            while (angleZ >= -180)
            {
                if (count == 1)
                {
                    scythes[0].transform.rotation = Quaternion.Euler(0, right ? 0 : 180, angleZ);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        scythes[i].transform.rotation = Quaternion.Euler(0, i % 2 == 0 ? 0 : 180, angleZ);
                    }
                }

                yield return null;
                angleZ -= Time.deltaTime * 500;
            }

            foreach (var scythe in scythes)
            {
                DespawnScythe(scythe);
            }
        }

        public Skill1022Scythe SpawnScythe()
        {
            var scythe = scythePool.Get();
            return scythe;
        }

        public void DespawnScythe(Skill1022Scythe scythe)
        {
            scythePool.Release(scythe);
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
