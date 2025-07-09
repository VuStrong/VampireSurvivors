using UnityEngine;
using UnityEngine.Localization.Settings;

namespace ProjectCode1.CrossScene
{
    public class SettingsManager
    {
        private static SettingsManager instance;
        public static SettingsManager Instance
        {
            get
            {
                instance ??= new SettingsManager();
                return instance;
            }
        }

        private int locale;
        public int Locale
        {
            get => locale;
            set
            {
                locale = value;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value];
                PlayerPrefs.SetInt("Locale", value);
            }
        }

        private bool damageDisplay;
        public bool DamageDisplay
        {
            get => damageDisplay;
            set
            {
                damageDisplay = value;
                PlayerPrefs.SetInt("DamageDisplay", value ? 1 : 0);
            }
        }

        public SettingsManager()
        {
            Init();
        }

        void Init()
        {
            // Locale
            locale = PlayerPrefs.GetInt("Locale", 0);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[locale];

            // Damage display
            damageDisplay = PlayerPrefs.GetInt("DamageDisplay", 1) == 1;
        }
    }
}
