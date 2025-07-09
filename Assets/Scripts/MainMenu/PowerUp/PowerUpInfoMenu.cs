using ProjectCode1.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu
{
    public class PowerUpInfoMenu : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text currentLevelText;
        [SerializeField] private TMP_Text nextLevelText;

        public void Open(PowerUpModel powerUpModel)
        {
            image.sprite = powerUpModel.PowerUpBlueprint.Sprite;
            currentLevelText.text = powerUpModel.GetDescriptionOfCurrentLevel();
            nextLevelText.text = powerUpModel.GetDescriptionOfNextLevel();
            gameObject.SetActive(true);
        }
    }
}
