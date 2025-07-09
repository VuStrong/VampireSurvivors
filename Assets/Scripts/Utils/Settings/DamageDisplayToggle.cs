using ProjectCode1.CrossScene;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.Utils.Settings
{
    public class DamageDisplayToggle : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;

        void Start()
        {
            bool active = SettingsManager.Instance.DamageDisplay;
            toggle.isOn = active;
            toggle.onValueChanged.AddListener(OnToggle);
        }

        void OnToggle(bool isOn)
        {
            SettingsManager.Instance.DamageDisplay = isOn;
        }
    }
}
