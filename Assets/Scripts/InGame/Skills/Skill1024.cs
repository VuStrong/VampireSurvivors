using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1024: LaserBeam - 
    /// Idle: Cast lasers rotate around player --- 
    /// Moving: Cast lasers to nearest enemy and go accross the screen
    /// </summary>
    public class Skill1024 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat cooldown;
        [SerializeField] private UpgradeableFloatStat duration;

        private readonly List<Skill1024Laser> lasers = new();
        private Skill1024Laser defaultLaser;
        private float timeSinceLastCast;
        private bool isCasting;

        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalCooldown { get => Mathf.Clamp(cooldown.Value * (1 - owner.stats.cooldownReduce / 100), 1, cooldown.Value); }
        public float FinalDuration { get => owner.stats.skillDurationBonus + duration.Value; }

        protected override void Awake()
        {
            base.Awake();

            defaultLaser = GetComponentInChildren<Skill1024Laser>();
            defaultLaser.Init(this);
            lasers.Add(defaultLaser);
        }

        void Start()
        {
            transform.parent = null;
            transform.position = Vector3.zero;
        }

        void Update()
        {
            CheckLasers();

            if (!isCasting) timeSinceLastCast += Time.deltaTime;
            if (timeSinceLastCast >= FinalCooldown && !isCasting)
            {
                isCasting = true;
                timeSinceLastCast = 0;

                if (owner.IsMoving)
                {
                    StartCoroutine(CastLasersToClosestEnemy());
                }
                else
                {
                    StartCoroutine(CastLasersAroundPlayer());
                }
            }
        }

        void CheckLasers()
        {
            int count = TotalProjectileCount;
            if (count <= 0) count = 1;
            if (count == lasers.Count) return;

            // add or remove lines
            if (lasers.Count < count)
            {
                int amountToAdd = count - lasers.Count;
                for (int i = 0; i < amountToAdd; i++)
                {
                    var laser = Instantiate(defaultLaser, transform, false);
                    laser.Init(this);
                    laser.Disable();
                    lasers.Add(laser);
                }
            }
            else
            {
                int amountToDestroy = lasers.Count - count;
                for (int i = 0; i < amountToDestroy; i++)
                {
                    int index = lasers.Count - 1;
                    Destroy(lasers[index].gameObject);
                    lasers.RemoveAt(index);
                }
            }
        }

        IEnumerator CastLasersAroundPlayer()
        {
            isCasting = true;
            int count = lasers.Count;
            float[] angles = new float[count];

            // Setup
            for (int i = 0; i < count; i++)
            {
                lasers[i].Setup();
                angles[i] = i * (2 * Mathf.PI / count);
            }

            float finalDuration = FinalDuration;
            float size = 0.5f;
            float t = 0;
            while (t <= finalDuration)
            {
                Vector3 startPos = owner.transform.position;
                // Draw lasers
                for (int i = 0; i < count; i++)
                {
                    Vector3 offset = new Vector2(Mathf.Cos(angles[i]), Mathf.Sin(angles[i])) * size;
                    Vector3 endPos = startPos + offset;

                    lasers[i].SetPoints(startPos, endPos);
                    angles[i] += Time.deltaTime * 2.5f;
                }

                yield return null;
                t += Time.deltaTime;
                size = Mathf.Clamp(size + Time.deltaTime * 2, 0, 10);
            }

            isCasting = false;
            foreach (var laser in lasers)
            {
                laser.Disable();
            }
        }

        IEnumerator CastLasersToClosestEnemy()
        {
            isCasting = true;
            int count = lasers.Count;
            var monsters = EntityManager.Instance.GetClosestMonsters(owner.transform.position, count);
            if (monsters.Count == 0)
            {
                isCasting = false;
                yield break;
            }

            // Setup lasers
            Vector2[] endPoints = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                lasers[i].Setup();
                endPoints[i] = i < monsters.Count ?
                    monsters[i].transform.position :
                    monsters[^1].transform.position;
            }

            Vector2 direction = owner.LookDirection;
            int endedLaserCount = 0; // Keep track of ended lasers, end the casting if all the lasers is ended
            while (endedLaserCount < count)
            {
                Vector2 startPoint = owner.transform.position;
                for (int i = 0; i < count; i++)
                {
                    if (lasers[i].Disabled) continue;

                    lasers[i].SetPoints(startPoint, endPoints[i]);
                    endPoints[i] += 8f * Time.deltaTime * direction;

                    if (!EntityManager.Instance.PositionOnScreen(endPoints[i]))
                    {
                        lasers[i].Disable();
                        endedLaserCount++;
                    }
                }

                yield return null;
            }

            isCasting = false;
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
