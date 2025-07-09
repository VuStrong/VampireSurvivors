using System.Collections;
using ProjectCode1.CrossScene;
using ProjectCode1.InGame.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectCode1.InGame.UI
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text winLoseText;
        [SerializeField] private TMP_Text coinGainedText;
        [SerializeField] private TMP_Text totalTimeText;
        [SerializeField] private TMP_Text accountLevelText;
        [SerializeField] private Slider accountExpBar;
        [SerializeField] private TMP_Text accountExpText;

        private Character character;
        private StatsManager statsManager;

        public void Setup(Character character, StatsManager statsManager)
        {
            this.character = character;
            this.statsManager = statsManager;
        }

        public void Open(bool win, float totalTime)
        {
            gameObject.SetActive(true);
            Time.timeScale = 0;

            int coinGained = statsManager.CoinGained;
            int expGained = 4;

            winLoseText.text = win ? "Win" : "Lose";
            coinGainedText.text = $"+{coinGained}";
            totalTimeText.text = System.TimeSpan.FromSeconds(totalTime).ToString(@"mm\:ss");

            Rewards(coinGained, expGained);
        }

        void Rewards(int coinGained, int expGained)
        {
            if (AccountManager.Instance == null) return;

            StartCoroutine(IncreaseExpAnimation(expGained));

            AccountManager.Instance.IncrementExperience(expGained);
            AccountManager.Instance.IncrementCoin(coinGained);
        }

        IEnumerator IncreaseExpAnimation(int expToGain)
        {
            int currentExp = AccountManager.Instance.Experience;
            int counter = currentExp;
            int finalExp = currentExp + expToGain;
            int currentLevel = AccountManager.Instance.Level;
            int expToNextLevel = AccountManager.Instance.ExperienceToNextLevel;
            var delay = new WaitForSecondsRealtime(0.01f);

            accountLevelText.text = currentLevel.ToString();
            accountExpText.text = $"{currentExp}(+{expToGain})/{expToNextLevel}";
            accountExpBar.value = (float)currentExp / expToNextLevel;

            while (counter < finalExp)
            {
                yield return delay;
                counter++;
                currentExp++;

                if (currentExp >= expToNextLevel)
                {
                    currentExp = 0;
                    currentLevel++;
                    expToNextLevel = AccountManager.ExperienceOfLevel(currentLevel);

                    accountLevelText.text = currentLevel.ToString();
                }

                accountExpText.text = $"{currentExp}(+{expToGain})/{expToNextLevel}";
                accountExpBar.value = (float)currentExp / expToNextLevel;
            }
        }

        public void ReturnToMainMenu()
        {
            StopAllCoroutines();
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
