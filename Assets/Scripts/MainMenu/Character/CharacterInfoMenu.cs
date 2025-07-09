using ProjectCode1.CrossScene;
using ProjectCode1.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu.Character
{
    public class CharacterInfoMenu : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private TMP_Text characterLevelText;
        [SerializeField] private Image image;
        [Header("Stat")]
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text speedText;
        [Header("Skill")]
        [SerializeField] private TMP_Text skillNameText;
        [SerializeField] private Image skillImage;
        [SerializeField] private TMP_Text characterEffectText;
        [Header("Rarity Objects")]
        [SerializeField] private GameObject commonRarity;
        [SerializeField] private GameObject rareRarity;
        [SerializeField] private GameObject legendaryRarity;
        [Header("Buttons")]
        [SerializeField] private GameObject selectButton;

        private CharacterModel characterModel;

        public void Open(CharacterModel characterModel)
        {
            this.characterModel = characterModel;

            characterNameText.text = characterModel.Blueprint.characterName;
            characterLevelText.text = characterModel.Level.ToString();
            image.sprite = characterModel.Blueprint.sprite;
            healthText.text = characterModel.Blueprint.health.ToString();
            speedText.text = characterModel.Blueprint.speed.ToString();
            skillNameText.text = characterModel.Blueprint.startingSkill.Name;
            skillImage.sprite = characterModel.Blueprint.startingSkill.Image;
            characterEffectText.text = characterModel.Blueprint.effectDescription.GetLocalizedString();
            selectButton.SetActive(characterModel.Owned);

            commonRarity.SetActive(characterModel.Blueprint.rarity == CharacterRarity.Common);
            rareRarity.SetActive(characterModel.Blueprint.rarity == CharacterRarity.Rare);
            legendaryRarity.SetActive(characterModel.Blueprint.rarity == CharacterRarity.Legendary);
            
            gameObject.SetActive(true);
        }

        public void Select()
        {
            if (characterModel != null)
            {
                CharacterManager.Instance.SelectCharacter(characterModel.Blueprint.id);
                gameObject.SetActive(false);
            }
        }
    }
}
