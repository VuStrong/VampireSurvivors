using ProjectCode1.Blueprints;
using ProjectCode1.CrossScene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectCode1.MainMenu
{
    public class MainMenuSceneManager : MonoBehaviour
    {
        void Awake()
        {
            if (AccountManager.Instance == null)
            {
                SceneManager.LoadScene("Bootstrap");
            }
        }

        public void PlayGame(MapBlueprint mapBlueprint)
        {
            CrossSceneData.mapBlueprint = mapBlueprint;
            SceneManager.LoadScene("Map1");
        }

        ////
        // Tests
        ////
        public void AddExpButtonClicked(int exp)
        {
            AccountManager.Instance.IncrementExperience(exp);
        }

        public void AddCoinClicked(int coin)
        {
            AccountManager.Instance.IncrementCoin(coin);
        }

        public void AddGemClicked(int gem)
        {
            AccountManager.Instance.IncrementGem(gem);
        }
    }
}