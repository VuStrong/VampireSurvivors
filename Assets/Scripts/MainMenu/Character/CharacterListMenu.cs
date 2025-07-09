using System.Collections;
using System.Collections.Generic;
using ProjectCode1.CrossScene;
using ProjectCode1.Models;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu.Character
{
    public class CharacterListMenu : MonoBehaviour
    {
        [SerializeField] private CharacterCard characterCardPrefab;
        [SerializeField] private Transform commonCharactersHolder;
        [SerializeField] private Transform rareCharactersHolder;
        [SerializeField] private Transform legendaryCharactersHolder;
        [SerializeField] private ContentSizeFitter contentSizeFitter;
        [SerializeField] private CharacterInfoMenu characterInfoMenu;

        public void Populate(IEnumerable<CharacterModel> characterModels)
        {
            foreach (Transform child in commonCharactersHolder.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in rareCharactersHolder.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in legendaryCharactersHolder.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var characterModel in characterModels)
            {
                Transform parent;
                if (characterModel.Blueprint.rarity == CharacterRarity.Common)
                {
                    parent = commonCharactersHolder;
                }
                else if (characterModel.Blueprint.rarity == CharacterRarity.Rare)
                {
                    parent = rareCharactersHolder;
                }
                else
                {
                    parent = legendaryCharactersHolder;
                }

                var card = Instantiate(characterCardPrefab, parent);
                card.Setup(characterModel, OnTapCharacterCard);
            }

            RefreshContentSize();
        }

        void RefreshContentSize()
        {
            IEnumerator Routine()
            {
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                yield return null;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            StartCoroutine(Routine());
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Populate(CharacterManager.Instance.Characters);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void OnTapCharacterCard(CharacterModel characterModel)
        {
            characterInfoMenu.Open(characterModel);
        }
    }
}
