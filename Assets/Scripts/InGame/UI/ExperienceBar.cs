using ProjectCode1.InGame.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.InGame.UI
{
    public class ExperienceBar : MonoBehaviour
    {
        [SerializeField] private Slider experienceBar;
        [SerializeField] private TMP_Text levelText;

        private Character character;

        public void Setup(Character character)
        {
            this.character = character;
            character.OnExperienceIncreased += OnPlayerExperienceIncreased;
            character.OnLevelUp += OnPlayerLevelUp;

            OnPlayerLevelUp(1);
        }

        void OnPlayerLevelUp(int level)
        {
            levelText.text = $"LV. {level}";
            experienceBar.maxValue = character.ExperienceToNextLevel;
            experienceBar.value = 0;
        }

        void OnPlayerExperienceIncreased(float value)
        {
            experienceBar.value = value;
        }

        void OnDestroy()
        {
            character.OnLevelUp -= OnPlayerLevelUp;
            character.OnExperienceIncreased -= OnPlayerExperienceIncreased;
        }
    }
}
