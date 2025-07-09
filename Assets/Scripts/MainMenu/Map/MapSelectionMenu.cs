using ProjectCode1.Blueprints;
using UnityEngine;

namespace ProjectCode1.MainMenu
{
    public class MapSelectionMenu : MonoBehaviour
    {
        [SerializeField] private MapBlueprint[] maps;
        [SerializeField] private MapItem mapItemPrefab;
        [SerializeField] private Transform mapItemsHolder;
        [SerializeField] private MainMenuSceneManager mainMenuSceneManager;

        void Start()
        {
            Init();
        }

        void Init()
        {
            foreach (var map in maps)
            {
                var mapItem = Instantiate(mapItemPrefab, mapItemsHolder, false);
                mapItem.Init(map, OnMapSelected);
            }
        }

        void OnMapSelected(MapBlueprint mapBlueprint)
        {
            mainMenuSceneManager.PlayGame(mapBlueprint);
        }
    }
}
