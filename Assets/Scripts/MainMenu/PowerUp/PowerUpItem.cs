using System;
using System.Collections;
using ProjectCode1.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu
{
    public class PowerUpItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text powerUpNameText;
        [SerializeField] private TMP_Text levelText;

        private PowerUpModel powerUpModel;
        private Action<PowerUpModel> onTap;
        public string PowerUpId { get => powerUpModel.PowerUpBlueprint.Id; }

        public void Init(PowerUpModel powerUpModel, Action<PowerUpModel> onTap)
        {
            this.powerUpModel = powerUpModel;
            this.onTap = onTap;
            Refresh();
        }

        public void Refresh()
        {
            if (powerUpModel == null) return;
            image.sprite = powerUpModel.PowerUpBlueprint.Sprite;
            powerUpNameText.text = powerUpModel.PowerUpBlueprint.PowerUpName.GetLocalizedString();
            levelText.text = powerUpModel.CanBeLevelUp ? $"LV.{powerUpModel.Level}" : "LV.MAX";
        }

        public void Flash()
        {
            StartCoroutine(FlashCoroutine());
        }

        IEnumerator FlashCoroutine()
        {
            Color color1 = new(bgImage.color.r, bgImage.color.g, bgImage.color.b, 0);
            Color color2 = new(bgImage.color.r, bgImage.color.g, bgImage.color.b, 1);
            float t = 0;
            while (t < 1)
            {
                bgImage.color = Color.Lerp(color1, color2, Mathf.Repeat(t * 2, 1));
                yield return null;
                t += Time.deltaTime;
            }
            bgImage.color = color1;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onTap?.Invoke(powerUpModel);
        }
    }
}
