using System.Collections.Generic;
using ProjectCode1.CrossScene;
using ProjectCode1.Models;
using ProjectCode1.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu
{
    public class PowerUpMenu : MonoBehaviour
    {
        [SerializeField] private PowerUpItem powerUpItemPrefab;
        [SerializeField] private GameObject powerUpItemHolder;
        [SerializeField] private TMP_Text coinToPowerUpText;
        [SerializeField] private Button powerUpButton;
        [SerializeField] private PowerUpInfoMenu powerUpInfoMenu;

        private readonly List<PowerUpItem> powerUpItems = new();

        void Start()
        {
            // Listen event to disable or enable powerup button
            PowerUpManager.Instance.CanPowerUpStatusChanged += SetPowerUpButtonInteractable;
            
            Populate(PowerUpManager.Instance.PowerUps);
            coinToPowerUpText.text = PowerUpManager.Instance.RequiredCoinToPowerUp.ToString();
            SetPowerUpButtonInteractable(PowerUpManager.Instance.CanPowerUp);
        }

        void Populate(IEnumerable<PowerUpModel> powerUpModels)
        {
            foreach (Transform child in powerUpItemHolder.transform)
            {
                Destroy(child.gameObject);
            }
            powerUpItems.Clear();

            foreach (var powerUpModel in powerUpModels)
            {
                var powerUpItem = Instantiate(powerUpItemPrefab, powerUpItemHolder.transform, false);
                powerUpItem.Init(powerUpModel, OnTapPowerUpItem);
                powerUpItems.Add(powerUpItem);
            }
        }

        void OnTapPowerUpItem(PowerUpModel powerUpModel)
        {
            powerUpInfoMenu.Open(powerUpModel);
        }

        void SetPowerUpButtonInteractable(bool interactable)
        {
            powerUpButton.interactable = interactable;
        }

        public void PowerUpButtonClicked()
        {
            LoadingOverlay.Instance?.Show();
            var powerUp = PowerUpManager.Instance.LevelUpRandomPowerUp();
            LoadingOverlay.Instance?.Hide();
            if (powerUp == null) return;

            RefreshAndFlashPowerUp(powerUp.PowerUpBlueprint.Id);
        }

        void RefreshAndFlashPowerUp(string powerUpId)
        {
            foreach (var powerUpItem in powerUpItems)
            {
                if (powerUpItem.PowerUpId == powerUpId)
                {
                    powerUpItem.Refresh();
                    powerUpItem.Flash();
                    return;
                }
            }
        }

        void OnDestroy()
        {
            PowerUpManager.Instance.CanPowerUpStatusChanged -= SetPowerUpButtonInteractable;
        }
    }
}
