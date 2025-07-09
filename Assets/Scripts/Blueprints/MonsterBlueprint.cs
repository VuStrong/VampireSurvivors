using UnityEngine;

namespace ProjectCode1.Blueprints
{
    [CreateAssetMenu(fileName = "Monster", menuName = "Blueprints/Monster")]
    public class MonsterBlueprint : ScriptableObject
    {
        public string id;
        public string monsterName;

        [Space(8)]
        [Header("Stats")]
        public float health = 10;
        public float speed = 6;
        public float damage;
        public float attackSpeed = 1;
        public LayerMask damageableLayerMask;

        [Space(8)]
        [Header("Sprite")]
        public Sprite sprite;
    }
}
