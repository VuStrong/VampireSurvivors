using UnityEngine;
using UnityEngine.Localization;

namespace ProjectCode1.Blueprints
{
    [CreateAssetMenu(fileName = "PowerUp", menuName = "Blueprints/PowerUp")]
    public class PowerUpBlueprint : ScriptableObject
    {
        [Header("Power up info")]
        [SerializeField] private string id;
        [SerializeField] private Sprite sprite;
        [SerializeField] private LocalizedString powerUpName;
        [SerializeField]
        [Range(0, 100)]
        private float chance;

        [Header("Power up values")]
        [SerializeField] private float[] values;
        [SerializeField] private LocalizedString powerUpDescription;

        public string Id { get => id; }
        public Sprite Sprite { get => sprite; }
        public LocalizedString PowerUpName { get => powerUpName; }
        public LocalizedString PowerUpDescription { get => powerUpDescription; }
        public float Chance { get => chance; }
        public int TotalLevel { get => values.Length; }

        public float GetValueAtLevel(int level)
        {
            if (level == 0) return 0;
            else if (level > values.Length) return values[^1];

            return values[level - 1];
        }

        public string GetPowerUpDescriptionAtLevel(int level)
        {
            if (level == 0 || level > values.Length) return "...";
            var value = GetValueAtLevel(level);
            return powerUpDescription.GetLocalizedString(new { value });
        }
    }
}
