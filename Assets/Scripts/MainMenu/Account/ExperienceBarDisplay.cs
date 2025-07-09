using ProjectCode1.CrossScene;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu
{
    public class ExperienceBarDisplay : MonoBehaviour
    {
        [SerializeField] private Slider experienceBar;
        [SerializeField] private TMP_Text experienceText;

        void OnEnable()
        {
            UpdateExpBar(AccountManager.Instance.Experience);
            AccountManager.Instance.ExperienceChanged += UpdateExpBar;
        }

        void OnDisable()
        {
            AccountManager.Instance.ExperienceChanged -= UpdateExpBar;
        }

        void UpdateExpBar(int exp)
        {
            int expToNextLevel = AccountManager.Instance.ExperienceToNextLevel;

            experienceBar.maxValue = expToNextLevel;
            experienceBar.value = exp;
            if (experienceText != null)
            {
                experienceText.text = $"{exp}/{expToNextLevel}";
            }
        }
    }
}