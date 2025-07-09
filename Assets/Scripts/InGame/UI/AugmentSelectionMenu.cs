using System;
using System.Collections.Generic;
using ProjectCode1.InGame.Augments;
using UnityEngine;

namespace ProjectCode1.InGame.UI
{
    public class AugmentSelectionMenu : MonoBehaviour
    {
        [SerializeField] private AugmentCard augmentCardPrefab;
        [SerializeField] private GameObject augmentCardsHolder;

        private Action<Augment> onAugmentSelected;

        public void Setup(Action<Augment> onAugmentSelected)
        {
            this.onAugmentSelected = onAugmentSelected;
        }

        public void Open(IEnumerable<Augment> augmentsToDisplay)
        {
            foreach (Transform child in augmentCardsHolder.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var augment in augmentsToDisplay)
            {
                var augmentCard = Instantiate(augmentCardPrefab, augmentCardsHolder.transform, false);
                augmentCard.Init(augment, onAugmentSelected);
            }

            gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        public void Close()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
