using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCode1.CrossScene
{
    public class AccountManager : MonoBehaviour
    {
        public static AccountManager Instance { get; private set; }
        public const string CloudSavePlayerLevelKey = "LEVEL";
        public const string CloudSavePlayerExperienceKey = "EXPERIENCE";

        public string UserId { get; private set; }
        public string Username { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int ExperienceToNextLevel { get => ExperienceOfLevel(Level); }
        public int Coin { get; private set; }
        public int Gem { get; private set; }
        public string UsernameWithoutSuffix
        {
            get
            {
                int index = Username.IndexOf('#');
                if (index >= 0)
                {
                    return Username[..index];
                }
                return Username;
            }
        }

        public Action<string> UsernameChanged;
        public Action<int> LevelChanged;
        public Action<int> ExperienceChanged;
        public Action<int> CoinChanged;
        public Action<int> GemChanged;

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

        public void Init()
        {
            UserId = Guid.NewGuid().ToString();
            Username = PlayerPrefs.GetString("Account.Name", "Username");
            Level = PlayerPrefs.GetInt("Account.Level", 1);
            Experience = PlayerPrefs.GetInt("Account.Experience", 0);
            Coin = PlayerPrefs.GetInt("Account.Coin", 0);
            Gem = PlayerPrefs.GetInt("Account.Gem", 0);

            UsernameChanged?.Invoke(Username);
            LevelChanged?.Invoke(Level);
            ExperienceChanged?.Invoke(Experience);
            CoinChanged?.Invoke(Coin);
            GemChanged?.Invoke(Gem);
        }

        public void UpdateUsername(string username)
        {
            username = username.Trim();
            if (username.Length < 1 || username.Length > 50)
            {
                throw new AppException("Invalid username", AppError.InvalidUsername);
            }

            PlayerPrefs.SetString("Account.Name", username);
            Username = username;
            UsernameChanged?.Invoke(Username);
        }

        public void IncrementCoin(int coin)
        {
            if (coin <= 0) return;

            PlayerPrefs.SetInt("Account.Coin", Coin + coin);
            Coin += coin;
            CoinChanged?.Invoke(Coin);
        }

        public void DecrementCoin(int coin)
        {
            if (coin > Coin) throw new AppException("Not enough currency", AppError.NotEnoughCurrency);

            PlayerPrefs.SetInt("Account.Coin", Coin - coin);
            Coin -= coin;
            CoinChanged?.Invoke(Coin);
        }

        public void IncrementGem(int gem)
        {
            if (gem <= 0) return;

            PlayerPrefs.SetInt("Account.Gem", Gem + gem);
            Gem += gem;
            GemChanged?.Invoke(Gem);
        }

        public void DecrementGem(int gem)
        {
            if (gem > Gem) throw new AppException("Not enough currency", AppError.NotEnoughCurrency);

            PlayerPrefs.SetInt("Account.Gem", Gem - gem);
            Gem -= gem;
            GemChanged?.Invoke(Gem);
        }

        public void IncrementExperience(int experience)
        {
            if (experience <= 0) return;

            Dictionary<string, object> data = new();
            bool isLevelUp = false;
            int finalExp = Experience + experience;
            int currentLevel = Level;
            int expToNextLevel = ExperienceOfLevel(currentLevel);
            int currentPowerUpCount = PowerUpManager.Instance.RemainingPowerUpCount;

            // Set data for next levels
            while (finalExp >= expToNextLevel)
            {
                finalExp -= expToNextLevel;
                currentLevel++;
                expToNextLevel = ExperienceOfLevel(currentLevel);
                currentPowerUpCount += 3;
                isLevelUp = true;
            }

            PlayerPrefs.SetInt("Account.Experience", finalExp);
            if (isLevelUp)
            {
                PlayerPrefs.SetInt("Account.Level", currentLevel);
                PlayerPrefs.SetInt("Account.RemainingPowerUpCount", currentPowerUpCount);

                Level = currentLevel;
                LevelChanged?.Invoke(currentLevel);
                PowerUpManager.Instance.SetRemainingPowerUpCount(currentPowerUpCount);
            }

            Experience = finalExp;
            ExperienceChanged?.Invoke(finalExp);
        }

        public static int ExperienceOfLevel(int level)
        {
            if (level <= 0) return 0;
            return level * 10 + level + 5;
        }
    }
}
