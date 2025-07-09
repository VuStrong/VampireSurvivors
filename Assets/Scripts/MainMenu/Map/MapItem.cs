using System;
using ProjectCode1.Blueprints;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectCode1.MainMenu
{
    public class MapItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descText;

        private MapBlueprint mapBlueprint;
        private Action<MapBlueprint> onSelected;

        public void Init(MapBlueprint mapBlueprint, Action<MapBlueprint> onSelected)
        {
            this.mapBlueprint = mapBlueprint;
            this.onSelected = onSelected;
            nameText.text = mapBlueprint.mapName;
            descText.text = mapBlueprint.description;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onSelected?.Invoke(mapBlueprint);
        }
    }
}
