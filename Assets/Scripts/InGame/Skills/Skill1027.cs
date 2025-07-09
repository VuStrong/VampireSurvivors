using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectCode1.InGame.Monsters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1027: Bat - Idle: X bats auto attack anemies --- Moving: Bat prioritizes attacking enemies in front of player
    /// </summary>
    public class Skill1027 : Skill
    {
        [SerializeField] private UpgradeableIntStat batCount;
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat duration;
        [SerializeField] private UpgradeableFloatStat cooldown;
        [SerializeField] private Skill1027Bat batPrefab;
        [SerializeField] private float batSpeed;

        private readonly List<Skill1027Bat> bats = new();
        private readonly List<Skill1027Bat> spawnedBats = new();
        private float counter;
        private bool isActivated;
        private bool isReturning;
        private float detectTimer;

        public int TotalBatCount { get => owner.stats.projectileCountBonus + batCount.Value; }
        public float FinalDuration { get => owner.stats.skillDurationBonus + duration.Value; }
        public float FinalCooldown { get => Mathf.Clamp(cooldown.Value * (1 - owner.stats.cooldownReduce / 100), 0.5f, cooldown.Value); }
        public float BatSpeed { get => batSpeed; }

        void Start()
        {
            counter = FinalCooldown;
        }

        void Update()
        {
            CheckBats();

            if (isActivated)
            {
                if (!isReturning) counter += Time.deltaTime;
                if (counter >= FinalDuration && !isReturning)
                {
                    StartCoroutine(ReturnAllBats());
                }
            }
            else
            {
                counter -= Time.deltaTime;
                if (counter <= 0)
                {
                    SpawnBats();
                }
            }

            if (isActivated && owner.IsMoving)
            {
                detectTimer += Time.deltaTime;
                if (detectTimer >= 0.5f)
                {
                    detectTimer = 0f;
                    DetectMonsterInFront();
                }
            }
        }

        void CheckBats()
        {
            int count = TotalBatCount;
            if (count == bats.Count) return;

            if (bats.Count < count)
            {
                int amountToAdd = count - bats.Count;
                for (int i = 0; i < amountToAdd; i++)
                {
                    var bat = Instantiate(batPrefab);
                    bat.Init(this);
                    bat.gameObject.SetActive(false);
                    bats.Add(bat);
                }
            }
            else
            {
                int amountToDestroy = bats.Count - count;
                for (int i = 0; i < amountToDestroy; i++)
                {
                    int index = bats.Count - 1;
                    Destroy(bats[index].gameObject);
                    bats.RemoveAt(index);
                }
            }
        }

        void SpawnBats()
        {
            int count = bats.Count;
            var monsters = EntityManager.Instance.GetAllVisibleMonsters(count);
            if (monsters.Count == 0) return;

            for (int i = 0; i < count; i++)
            {
                var offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                bats[i].transform.position = owner.transform.position + offset;
                bats[i].gameObject.SetActive(true);
                bats[i].SetTarget(i < monsters.Count ? monsters[i] : monsters[^1]);
                spawnedBats.Add(bats[i]);
            }

            isActivated = true;
            counter = 0;
        }

        IEnumerator ReturnAllBats()
        {
            isReturning = true;

            foreach (var bat in spawnedBats)
            {
                bat.Return();
            }

            while (!spawnedBats.All(b => b.Returned))
            {
                yield return null;
            }

            spawnedBats.Clear();
            isActivated = false;
            isReturning = false;
            counter = FinalCooldown;
        }

        public void DespawnBat(Skill1027Bat bat)
        {
            bat.gameObject.SetActive(false);
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }

        void DetectMonsterInFront()
        {
            Vector2 direction = owner.LookDirection;
            Vector3 originPos = owner.transform.position;
            var monster = DetectMonsterInCone2D(direction, originPos, 60, 3);
            if (monster != null)
            {
                foreach (var bat in spawnedBats)
                {
                    bat.SetTarget(monster);
                }
            }
        }

        Monster DetectMonsterInCone2D(Vector2 forward, Vector2 origin, float angle, float distance)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, distance, owner.DamageableLayerMask);
            foreach (var hit in hits)
            {
                Vector2 dirToTarget = (hit.transform.position - (Vector3)origin).normalized;
                float angleToTarget = Vector2.Angle(forward, dirToTarget);

                if (angleToTarget <= angle / 2f && hit.TryGetComponent<Monster>(out var monster))
                {
                    return monster;
                }
            }

            return null;
        }
    }
}
