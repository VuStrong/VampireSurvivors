using System.Collections.Generic;
using ProjectCode1.InGame.Characters;
using ProjectCode1.InGame.Collectables;
using ProjectCode1.InGame.DestructibleUnits;
using ProjectCode1.InGame.Monsters;
using ProjectCode1.InGame.Pools;
using ProjectCode1.Blueprints;
using ProjectCode1.Utils;
using UnityEngine;
using System.Linq;
using ProjectCode1.CrossScene;

namespace ProjectCode1.InGame
{
    public class EntityManager : MonoBehaviour
    {
        public static EntityManager Instance { get; private set; }

        [Header("Pools")]
        [SerializeField] private GameObject monsterPoolHolder;
        [SerializeField] private CollectableExperiencePool expPool;
        [SerializeField] private CollectableCoinPool coinPool;
        [SerializeField] private FloatingTextPool floatingTextPool;

        [Header("Floating Text")]
        [SerializeField] private float floatingTextSize;
        [SerializeField] private Color floatingTextColor;
        [SerializeField] private Color floatingTextCritColor;
        [SerializeField] private Color floatingTextHealingColor;

        [Header("Destructible Units")]
        [SerializeField] private DestructibleUnit destructibleUnitPrefab;
        [SerializeField] private float destructibleUnitsSpawnDelay;
        private float timeSinceLastDestructibleUnitSpawned;
        private const int MaxDestructibleUnitCount = 10;
        private int destructibleUnitCount = 0;

        private readonly Dictionary<string, MonsterPool> monsterPoolDict = new();
        private readonly FastList<Collectable> canFlyCollectables = new();
        public readonly FastList<Monster> aliveMonsters = new();
        private Character character;
        private StatsManager statsManager;
        private float screenWidthWorldSpace, screenHeightWorldSpace;

        public float MonsterHpMultiplier
        {
            get => character.CurrentLevel;
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void Setup(Character character, MapBlueprint mapBlueprint, StatsManager statsManager)
        {
            this.character = character;
            this.statsManager = statsManager;

            Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
            Vector2 topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
            screenWidthWorldSpace = topRight.x - bottomLeft.x;
            screenHeightWorldSpace = topRight.y - bottomLeft.y;

            var normalMonsters = mapBlueprint.GetAllNormalMonsters();
            foreach (var normalMonster in normalMonsters)
            {
                var monsterPool = monsterPoolHolder.AddComponent<MonsterPool>();
                monsterPool.Setup(normalMonster);
                monsterPoolDict[normalMonster.Id] = monsterPool;
            }
        }

        void Update()
        {
            timeSinceLastDestructibleUnitSpawned += Time.deltaTime;
            if (timeSinceLastDestructibleUnitSpawned >= destructibleUnitsSpawnDelay && destructibleUnitCount < MaxDestructibleUnitCount)
            {
                SpawnDestructibleUnit();
                timeSinceLastDestructibleUnitSpawned = 0f;
            }
        }

        public Vector2 GetRandomPositionOffscreen()
        {
            Vector2 randomScreenPosition = new(Random.Range(-0.1f, 1.1f), Random.Range(-0.1f, 1.1f));

            if (Random.Range(0, 2) == 0)
            {
                randomScreenPosition.x = randomScreenPosition.x > 0.5f ? 1.1f : -0.1f;
            }
            else
            {
                randomScreenPosition.y = randomScreenPosition.y > 0.5f ? 1.1f : -0.1f;
            }

            return Camera.main.ViewportToWorldPoint(randomScreenPosition);
        }

        public bool PositionOnScreen(Vector2 position)
        {
            var camera = Camera.main;
            return (
                position.x > camera.transform.position.x - screenWidthWorldSpace / 2 &&
                position.x < camera.transform.position.x + screenWidthWorldSpace / 2 &&
                position.y > camera.transform.position.y - screenHeightWorldSpace / 2 &&
                position.y < camera.transform.position.y + screenHeightWorldSpace / 2
            );
        }


        ////////////////////////////////////////////////////////////////////////////
        //// Monsters
        ////////////////////////////////////////////////////////////////////////////
        public Monster SpawnMonsterAtRandomPosition(string monsterId)
        {
            return SpawnMonster(monsterId, GetRandomPositionOffscreen());
        }

        public Monster SpawnMonster(string monsterId, Vector2 position)
        {
            if (!monsterPoolDict.TryGetValue(monsterId, out var monsterPool))
            {
                return null;
            }
            var monster = monsterPool.Get();
            monster.transform.position = position;
            monster.Setup(character.transform);

            aliveMonsters.Add(monster);
            return monster;
        }

        public void DespawnMonster(Monster monster)
        {
            if (!monsterPoolDict.TryGetValue(monster.Id, out var monsterPool))
            {
                return;
            }
            monsterPool.Release(monster);
            aliveMonsters.Remove(monster);
            statsManager.MonstersKilled++;
        }

        public List<Monster> GetAllVisibleMonsters(int max = 0)
        {
            List<Monster> list = new();

            foreach (var monster in aliveMonsters)
            {
                if (PositionOnScreen(monster.transform.position))
                {
                    list.Add(monster);
                }
                if (max > 0 && list.Count >= max)
                    break;
            }

            return list;
        }

        public Monster GetClosestMonster(Vector2 position, bool mustOnScreen = false)
        {
            Monster monster = null;
            float closest = 1000;

            foreach (var aliveMonster in aliveMonsters)
            {
                if (mustOnScreen && !PositionOnScreen(aliveMonster.transform.position))
                {
                    continue;
                }

                float distance = ((Vector2)aliveMonster.transform.position - position).sqrMagnitude;
                if (distance < closest)
                {
                    closest = distance;
                    monster = aliveMonster;
                }
            }

            return monster;
        }

        public List<Monster> GetClosestMonsters(Vector2 position, int max = 1, bool mustOnScreen = false)
        {
            IEnumerable<Monster> list = mustOnScreen ? GetAllVisibleMonsters() : aliveMonsters;
            return list
                .OrderBy(m => (m.transform.position - (Vector3)position).sqrMagnitude)
                .Take(max)
                .ToList();
        }


        ////////////////////////////////////////////////////////////////////////////
        //// Collectable Experience
        ////////////////////////////////////////////////////////////////////////////
        public CollectableExperience SpawnExp(Vector2 position)
        {
            var exp = expPool.Get();
            exp.Setup(position);
            canFlyCollectables.Add(exp);
            return exp;
        }

        public void DespawnExp(CollectableExperience exp)
        {
            expPool.Release(exp);
            canFlyCollectables.Remove(exp);
        }


        ////////////////////////////////////////////////////////////////////////////
        //// Collectable Coin
        ////////////////////////////////////////////////////////////////////////////
        public CollectableCoin SpawnCoin(Vector2 position)
        {
            var coin = coinPool.Get();
            coin.Setup(position);
            canFlyCollectables.Add(coin);
            return coin;
        }

        public void DespawnCoin(CollectableCoin coin)
        {
            coinPool.Release(coin);
            canFlyCollectables.Remove(coin);

            int coinToGain = coin.CoinAmount + Mathf.FloorToInt(character.stats.greed / 100 * coin.CoinAmount);
            statsManager.CoinGained += coinToGain;
        }


        ////////////////////////////////////////////////////////////////////////////
        //// Collectables
        ////////////////////////////////////////////////////////////////////////////
        public Collectable SpawnCollectable(Collectable prefab, Vector2 position)
        {
            if (prefab is CollectableCoin)
            {
                return SpawnCoin(position);
            }
            else if (prefab is CollectableExperience)
            {
                return SpawnExp(position);
            }

            var collectable = Instantiate(prefab);
            collectable.Setup(position);
            return collectable;
        }

        public void CollectAllCollectableItems()
        {
            foreach (var c in canFlyCollectables)
            {
                if (c != null) c.FlyToCollector(character);
            }
        }


        ////////////////////////////////////////////////////////////////////////////
        //// Destructible Unit
        ////////////////////////////////////////////////////////////////////////////
        public DestructibleUnit SpawnDestructibleUnit()
        {
            var unit = Instantiate(destructibleUnitPrefab, GetRandomPositionOffscreen(), Quaternion.identity);
            destructibleUnitCount++;
            unit.OnDestroyed = () => destructibleUnitCount--;
            return unit;
        }


        ////////////////////////////////////////////////////////////////////////////
        //// Floating Text
        ////////////////////////////////////////////////////////////////////////////
        public FloatingText SpawnDamageText(Vector2 position, float damage, bool isCrit = false)
        {
            if (!SettingsManager.Instance.DamageDisplay) return null;

            var text = floatingTextPool.Get();
            if (isCrit)
                text.Setup(position, damage.ToString(), floatingTextCritColor, floatingTextSize);
            else
                text.Setup(position, damage.ToString(), floatingTextColor, floatingTextSize);

            return text;
        }

        public FloatingText SpawnHealingText(Vector2 position, float amount)
        {
            var text = floatingTextPool.Get();
            text.Setup(position, $"+{amount}", floatingTextHealingColor, floatingTextSize);
            return text;
        }

        public void DespawnFloatingText(FloatingText text)
        {
            floatingTextPool.Release(text);
        }


        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
