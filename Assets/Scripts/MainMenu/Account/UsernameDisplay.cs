using ProjectCode1.CrossScene;
using TMPro;
using UnityEngine;

namespace ProjectCode1.MainMenu
{
    public class UsernameDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text usernameText;

        void OnEnable()
        {
            usernameText.text = AccountManager.Instance.Username;
            AccountManager.Instance.UsernameChanged += UpdateUsernameText;
        }

        void OnDisable()
        {
            AccountManager.Instance.UsernameChanged -= UpdateUsernameText;
        }

        void UpdateUsernameText(string username)
        {
            usernameText.text = username;
        }
    }
}
