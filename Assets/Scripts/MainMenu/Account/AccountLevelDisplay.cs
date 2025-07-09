using ProjectCode1.CrossScene;
using TMPro;
using UnityEngine;

namespace ProjectCode1.MainMenu
{
    public class AccountLevelDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;

        void OnEnable()
        {
            levelText.text = AccountManager.Instance.Level.ToString();
            AccountManager.Instance.LevelChanged += UpdateLevelText;
        }

        void OnDisable()
        {
            AccountManager.Instance.LevelChanged -= UpdateLevelText;
        }

        void UpdateLevelText(int level)
        {
            levelText.text = level.ToString();
        }
    }
}
