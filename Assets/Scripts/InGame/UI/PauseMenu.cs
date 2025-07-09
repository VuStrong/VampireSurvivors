using UnityEngine;

namespace ProjectCode1.InGame.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public void OpenPauseMenu()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        public void ClosePauseMenu()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }
}