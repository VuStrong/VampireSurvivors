using ProjectCode1.InGame.Skills;
using UnityEngine;
using UnityEngine.Localization;

namespace ProjectCode1.Blueprints
{
    [CreateAssetMenu(fileName = "Character", menuName = "Blueprints/Character")]
    public class CharacterBlueprint : ScriptableObject
    {
        public string id;
        public string characterName;
        public LocalizedString effectDescription;
        public Sprite sprite;
        public CharacterRarity rarity;

        [Space(8)]
        [Header("Stats")]
        [Tooltip("X Max health")]
        public float health = 100;
        [Tooltip("X Amount of HP restored per second")]
        public float recoveryPerSecond;
        [Tooltip("X Move speed")]
        public float speed = 6;
        [Tooltip("X Armor")]
        public int armor;
        [Tooltip("X% skill damage bonus")]
        public float skillDamageBonus;
        [Tooltip("X% experience received bonus")]
        public float experienceBonus;
        [Tooltip("Reduce X% skill cooldown")]
        public float cooldownReduce;
        [Tooltip("X skill projectile count bonus")]
        public int projectileCountBonus;
        [Tooltip("X% skill projectile speed bonus")]
        public float projectileSpeedBonus;
        [Tooltip("X% skill area bonus")]
        public float skillAreaBonus;
        [Tooltip("X pickup range")]
        public float pickupRange;
        [Tooltip("X seconds duration of skill bonus")]
        public float skillDurationBonus;
        [Tooltip("X% coin received bonus")]
        public float greed;
        [Tooltip("X% crit chance")]
        public float critChance;
        [Tooltip("X% crit damage")]
        public float critDamage;

        [Space(10)]
        [Header("Skill")]
        public Skill startingSkill;
    }
}