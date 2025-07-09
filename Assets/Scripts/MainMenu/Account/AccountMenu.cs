using System;
using ProjectCode1.CrossScene;
using ProjectCode1.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu
{
    public class AccountMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text accountLevelText;
        [SerializeField] private TMP_Text userIdText;
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private Slider experienceBar;
        [SerializeField] private TMP_Text experienceText;

        void OnEnable()
        {
            var accountManager = AccountManager.Instance;
            accountLevelText.text = accountManager.Level.ToString();
            userIdText.text = $"User ID: {accountManager.UserId}";
            usernameInputField.text = accountManager.UsernameWithoutSuffix;
            experienceBar.maxValue = accountManager.ExperienceToNextLevel;
            experienceBar.value = accountManager.Experience;
            experienceText.text = $"{accountManager.Experience}/{accountManager.ExperienceToNextLevel}";

            accountManager.ExperienceChanged += UpdateExpBar;
            accountManager.LevelChanged += UpdateAccountLevel;
            accountManager.UsernameChanged += UpdateUsername;
        }

        void OnDisable()
        {
            AccountManager.Instance.ExperienceChanged -= UpdateExpBar;
            AccountManager.Instance.LevelChanged -= UpdateAccountLevel;
            AccountManager.Instance.UsernameChanged -= UpdateUsername;
        }

        void UpdateExpBar(int exp)
        {
            int expToNextLevel = AccountManager.Instance.ExperienceToNextLevel;

            experienceBar.maxValue = expToNextLevel;
            experienceBar.value = exp;
            experienceText.text = $"{exp}/{expToNextLevel}";
        }

        void UpdateAccountLevel(int level)
        {
            accountLevelText.text = level.ToString();
        }

        void UpdateUsername(string _)
        {
            usernameInputField.text = AccountManager.Instance.UsernameWithoutSuffix;
        }

        public void CopyUserId()
        {
            GUIUtility.systemCopyBuffer = AccountManager.Instance.UserId;
            Toast.Instance.ShowCopiedMessage();
        }

        public void ChangeUsername()
        {
            try
            {
                LoadingOverlay.Instance?.Show();
                AccountManager.Instance.UpdateUsername(usernameInputField.text);
            }
            catch (AppException ex)
            {
                if (ex.Error == AppError.InvalidUsername)
                    Toast.Instance?.ShowInvalidUsernameMessage();
                else
                {
                    Toast.Instance?.Show("Error");
                    Debug.LogException(ex);
                }
            }
            catch (Exception ex)
            {
                Toast.Instance?.Show("Error");
                Debug.LogException(ex);
            }
            LoadingOverlay.Instance?.Hide();
        }
    }
}
