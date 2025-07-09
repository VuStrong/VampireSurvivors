using System;
using ProjectCode1.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu.Character
{
    public class CharacterCard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image displayImage;
        [SerializeField] private GameObject lockedOverlay;

        private CharacterModel characterModel;
        private Action<CharacterModel> onTap;

        public void Setup(CharacterModel characterModel, Action<CharacterModel> onTap)
        {
            this.characterModel = characterModel;
            this.onTap = onTap;
            
            displayImage.sprite = characterModel.Blueprint.sprite;
            lockedOverlay.SetActive(!characterModel.Owned);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onTap?.Invoke(characterModel);
        }
    }
}
