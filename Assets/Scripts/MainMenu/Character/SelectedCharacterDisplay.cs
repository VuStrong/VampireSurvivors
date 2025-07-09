using ProjectCode1.CrossScene;
using ProjectCode1.Models;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu.Character
{
    public class SelectedCharacterDisplay : MonoBehaviour
    {
        [SerializeField] private CharacterInfoMenu characterInfoMenu;
        [SerializeField] private Image image;

        private CharacterModel selectedCharacter;

        void OnEnable()
        {
            CharacterManager.Instance.SelectedCharacterChanged += UpdateCharacter;
            UpdateCharacter();
        }

        void OnDisable()
        {
            CharacterManager.Instance.SelectedCharacterChanged -= UpdateCharacter;
        }

        void UpdateCharacter()
        {
            var selectedCharacter = CharacterManager.Instance.SelectedCharacter;
            if (selectedCharacter != null)
            {
                this.selectedCharacter = selectedCharacter;
                image.sprite = selectedCharacter.Blueprint.sprite;
            }
        }

        public void OpenCharacterInfoMenu()
        {
            if (selectedCharacter != null)
            {
                characterInfoMenu.Open(selectedCharacter);
            }
        }
    }
}
