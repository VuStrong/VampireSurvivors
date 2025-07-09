using System;
using ProjectCode1.InGame.Augments;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectCode1.InGame.UI
{
    public class AugmentCard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text descText;
        private Augment augment;
        private Action<Augment> onSelected;

        public void Init(Augment augment, Action<Augment> onSelected)
        {
            this.augment = augment;
            this.onSelected = onSelected;

            descText.text = augment.Description;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onSelected?.Invoke(augment);
        }
    }
}
