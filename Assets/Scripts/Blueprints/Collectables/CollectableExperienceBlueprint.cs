using UnityEngine;

namespace ProjectCode1.Blueprints
{
    [CreateAssetMenu(fileName = "CollectableExp", menuName = "Blueprints/Collectable/Exp", order = 1)]
    public class CollectableExperienceBlueprint : ScriptableObject
    {
        [Tooltip("Amount of experience to gain")]
        public float exp;
    }
}
