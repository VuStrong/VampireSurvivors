using System;
using ProjectCode1.InGame.Collectables;
using UnityEngine;

namespace ProjectCode1.Blueprints
{
    [CreateAssetMenu(fileName = "DestructibleUnit", menuName = "Blueprints/DestructibleUnit")]
    public class DestructibleUnitBlueprint : ScriptableObject
    {
        public float health = 10;
        public Sprite sprite;
        public Drop[] drops;

        [Serializable]
        public class Drop
        {
            public Collectable item;
            [Range(0f, 100f)]
            public float chance;
        }
    }
}