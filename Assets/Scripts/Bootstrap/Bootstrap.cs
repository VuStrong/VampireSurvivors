using ProjectCode1.CrossScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectCode1.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;
        [SerializeField] private Button playButton;

        void Start()
        {
            playButton.gameObject.SetActive(false);
            InitManagers();
            playButton.gameObject.SetActive(true);
        }

        void OnEnable()
        {
            playButton.onClick.AddListener(PlayGame);
        }

        void OnDisable()
        {
            playButton.onClick.RemoveAllListeners();
        }

        void InitManagers()
        {
            _ = SettingsManager.Instance;
            AccountManager.Instance.Init();
            CharacterManager.Instance.InitializeCharacters();
            PowerUpManager.Instance.Init();
        }

        void PlayGame()
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
