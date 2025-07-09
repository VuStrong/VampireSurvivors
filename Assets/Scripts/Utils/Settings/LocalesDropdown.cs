using System.Collections;
using System.Collections.Generic;
using ProjectCode1.CrossScene;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace ProjectCode1.Utils.Settings
{
    public class LocalesDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;

        IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;

            var options = new List<TMP_Dropdown.OptionData>();
            int selected = 0;
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[i];
                if (LocalizationSettings.SelectedLocale == locale)
                    selected = i;

                options.Add(new TMP_Dropdown.OptionData(locale.name));
            }

            dropdown.options = options;
            dropdown.value = selected;
            dropdown.onValueChanged.AddListener(OnLocaleSelected);
        }

        void OnLocaleSelected(int index)
        {
            SettingsManager.Instance.Locale = index;
        }
    }
}
