using ProjectCode1.CrossScene;
using ProjectCode1.InGame.Characters;
using ProjectCode1.InGame.UI;
using ProjectCode1.Blueprints;
using TMPro;
using UnityEngine;

namespace ProjectCode1.InGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MapBlueprint mapBlueprint;
        [SerializeField] private string defaultCharacterId;
        [SerializeField] private SkillManager skillManager;
        [SerializeField] private EntityManager entityManager;
        [SerializeField] private StatsManager statsManager;
        [SerializeField] private TouchJoystick touchJoystick;
        [SerializeField] private KeyboardMovement keyboardMovement;

        [Header("UI")]
        [SerializeField] private ExperienceBar experienceBar;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private GameOverMenu gameOverMenu;

        private Character character;
        private float time = 0;
        private float timeSinceLastMonsterSpawned = 0;
        private bool finalBossSpawned = false;
        private int currentWaveIndex = 0;

        void Awake()
        {
            if (CrossSceneData.mapBlueprint != null)
            {
                mapBlueprint = CrossSceneData.mapBlueprint;
            }

            Setup();
        }

        void Setup()
        {
            var characterId = CharacterManager.Instance?.SelectedCharacter?.Blueprint.id ?? defaultCharacterId;
            character = Instantiate(Resources.Load<Character>($"Prefabs/Characters/{characterId}"), Vector3.zero, Quaternion.identity);
            SetUpCharacter();

            // Setup managers
            skillManager.Setup(character);
            entityManager.Setup(character, mapBlueprint, statsManager);

            // Setup UI
            experienceBar.Setup(character);
            gameOverMenu.Setup(character, statsManager);

            // Setup Camera
            Camera.main.GetComponent<CameraFollowPlayer>().target = character.transform;

            // Setup Controls
            touchJoystick.onJoystickMoved.AddListener(character.SetMoveDirection);
            keyboardMovement.onMoved.AddListener(character.SetMoveDirection);

            // Events
            character.OnDeath += GameOver;
        }

        void SetUpCharacter()
        {
            var powerUps = PowerUpManager.Instance?.PowerUps;
            if (powerUps == null) return;
            foreach (var powerUp in powerUps)
            {
                float value = powerUp.GetValueOfCurrentLevel();

                switch (powerUp.PowerUpBlueprint.Id)
                {
                    case "PowerUpDamage":
                        character.stats.skillDamageBonus += value;
                        break;
                    case "PowerUpHealth":
                        character.stats.health *= 1 + value / 100;
                        break;
                    case "PowerUpRevive":
                        character.stats.reviveCount += (int)value;
                        break;
                }
            }
        }

        void Update()
        {
            // Keep track time
            time += Time.deltaTime;
            timeText.text = System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss");

            // Spawn final boss
            if (time >= mapBlueprint.totalTime && !finalBossSpawned)
            {
                finalBossSpawned = true;
                var boss = Instantiate(mapBlueprint.boss, Vector2.zero, Quaternion.identity);
                boss.Setup(character.transform);
                boss.OnDeath += Pass;
                entityManager.aliveMonsters.Add(boss);
                return;
            }

            if (time >= 3)
            {
                CheckAndSpawnNormalMonster();
            }
        }

        public void LevelUpTest()
        {
            character.GainExperience(10000);
        }

        public void GameOver()
        {
            gameOverMenu.Open(false, time);
        }

        void Pass()
        {
            gameOverMenu.Open(true, time);
        }


        ////////////////////////////////////////////////////////////////////////////
        //// Monsters Spawning
        //// Just like Vampire Survivors: Monsters normally arrive in waves - one wave every fixed second
        //// Each wave specifies a minimum amount and a spawn interval for the monsters
        //// The game will attemp to spawn more monsters if the minimum amount of monsters is not met
        ////////////////////////////////////////////////////////////////////////////
        void CheckAndSpawnNormalMonster()
        {
            if (currentWaveIndex < mapBlueprint.waves.Length - 1 && time >= (currentWaveIndex + 1) * mapBlueprint.timeBetweenWave)
            {
                currentWaveIndex++;
            }

            int totalMonsterCount = entityManager.aliveMonsters.Count;
            if (totalMonsterCount < 300)
            {
                timeSinceLastMonsterSpawned += Time.deltaTime;
                var currentWave = mapBlueprint.waves[currentWaveIndex];
                float spawnInterval;

                if (totalMonsterCount <= currentWave.minimumMonsterAmount)
                {
                    spawnInterval = 0;
                }
                else
                {
                    spawnInterval = currentWave.spawnInterval;
                }

                if (timeSinceLastMonsterSpawned >= spawnInterval)
                {
                    string monsterId = SelectMonsterInWave(currentWave);
                    entityManager.SpawnMonsterAtRandomPosition(monsterId);
                    timeSinceLastMonsterSpawned = 0;
                }
            }
        }

        string SelectMonsterInWave(MapBlueprint.Wave wave)
        {
            if (wave.spawnChances.Length == 0) return "";

            float rand = Random.value * 100;
            float accum = 0f;

            foreach (var spawnChance in wave.spawnChances)
            {
                accum += spawnChance.chance;
                if (rand <= accum)
                {
                    return spawnChance.monster.Id;
                }
            }

            return wave.spawnChances[0].monster.Id;
        }
    }
}
