using System;
using System.Collections.Generic;
using System.Linq;
using ProjectCode1.Models;
using ProjectCode1.Blueprints;
using UnityEngine;
using Newtonsoft.Json;

namespace ProjectCode1.CrossScene
{
    public class PowerUpManager : MonoBehaviour
    {
        public static PowerUpManager Instance { get; private set; }

        [SerializeField] private int requiredCoinToPowerUp;

        /// <summary>
        /// Key is ID of the PowerUp and value is level of the PowerUp
        /// </summary>
        private Dictionary<string, int> playerPowerUp;
        private readonly List<PowerUpModel> powerUps = new();

        public int RemainingPowerUpCount { get; private set; }
        public int RequiredCoinToPowerUp { get => requiredCoinToPowerUp; }
        public bool CanPowerUp { get; private set; }
        public Action<bool> CanPowerUpStatusChanged;
        public IReadOnlyList<PowerUpModel> PowerUps { get => powerUps.AsReadOnly(); }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            AccountManager.Instance.CoinChanged += OnAccountCoinChanged;
        }

        public void Init()
        {
            string powerUpJson = PlayerPrefs.GetString("PowerUps", "");
            if (!string.IsNullOrEmpty(powerUpJson))
            {
                playerPowerUp = JsonConvert.DeserializeObject<Dictionary<string, int>>(powerUpJson);
            }
            else
            {
                playerPowerUp = new Dictionary<string, int>();
            }
            RemainingPowerUpCount = PlayerPrefs.GetInt("Account.RemainingPowerUpCount", 3);

            var powerUpBlueprints = Resources.LoadAll<PowerUpBlueprint>("Blueprints/PowerUps").ToList();
            if (powerUpBlueprints.Count == 0) return;

            foreach (var powerUpBlueprint in powerUpBlueprints)
            {
                if (!playerPowerUp.TryGetValue(powerUpBlueprint.Id, out int level))
                {
                    level = 0;
                }

                var model = new PowerUpModel(powerUpBlueprint, level);
                powerUps.Add(model);
            }

            CheckCanPowerUp();
        }

        void OnAccountCoinChanged(int coin)
        {
            CheckCanPowerUp();
        }

        void CheckCanPowerUp()
        {
            bool canPowerUp = requiredCoinToPowerUp <= AccountManager.Instance.Coin && RemainingPowerUpCount > 0;
            if (CanPowerUp != canPowerUp)
            {
                CanPowerUp = canPowerUp;
                CanPowerUpStatusChanged?.Invoke(CanPowerUp);
            }
        }

        /// <summary>
        /// Level up random available PowerUps
        /// </summary>
        /// <returns>Updated PowerUp, null if no PoweUp updated</returns>
        public PowerUpModel LevelUpRandomPowerUp()
        {
            if (!CanPowerUp)
                return null;

            PowerUpModel selectedPowerUp = SelectRandomPowerUp();
            if (selectedPowerUp == null)
                return null;

            playerPowerUp[selectedPowerUp.PowerUpBlueprint.Id] = ++selectedPowerUp.Level;
            RemainingPowerUpCount--;

            AccountManager.Instance.DecrementCoin(requiredCoinToPowerUp);
            SavePowerUp();

            CheckCanPowerUp();

            return selectedPowerUp;
        }

        PowerUpModel SelectRandomPowerUp()
        {
            float chance = 100;
            List<PowerUpModel> powerUpsCanBeLevelUp = new();

            // Remove chance of power ups that cannot be level up
            foreach (var powerUp in powerUps)
            {
                if (powerUp.CanBeLevelUp)
                {
                    powerUpsCanBeLevelUp.Add(powerUp);
                }
                else
                {
                    chance -= powerUp.PowerUpBlueprint.Chance;
                }
            }

            if (chance <= 0) return null;

            float rand = UnityEngine.Random.Range(0, chance);
            float accum = 0;
            foreach (var powerUp in powerUpsCanBeLevelUp)
            {
                accum += powerUp.PowerUpBlueprint.Chance;
                if (rand <= accum)
                {
                    return powerUp;
                }
            }

            return null;
        }

        void SavePowerUp()
        {
            PlayerPrefs.SetInt("Account.RemainingPowerUpCount", RemainingPowerUpCount);
            PlayerPrefs.SetString("PowerUps", JsonConvert.SerializeObject(playerPowerUp));
        }

        public void SetRemainingPowerUpCount(int count)
        {
            if (count < 0) return;

            RemainingPowerUpCount = count;
            CheckCanPowerUp();
        }
    }
}
