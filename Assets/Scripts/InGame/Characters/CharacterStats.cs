using System;
using ProjectCode1.Blueprints;

namespace ProjectCode1.InGame.Characters
{
    [Serializable]
    public class CharacterStats
    {
        /// <summary>
        /// X Max health
        /// </summary>
        public float health = 100;

        /// <summary>
        /// X Amount of HP restored per second
        /// </summary>
        public float recoveryPerSecond;

        /// <summary>
        /// X Move speed.
        /// Modify this stats to speedup forever.
        /// For temporary speedup, should use the IncreaseSpeed method instead
        /// </summary>
        public float speed = 6;

        /// <summary>
        /// X Armor
        /// </summary>
        public int armor;

        /// <summary>
        /// X% skill damage bonus
        /// </summary>
        public float skillDamageBonus;

        /// <summary>
        /// X% experience received bonus
        /// </summary>
        public float experienceBonus;

        /// <summary>
        /// Reduce X% skill cooldown
        /// </summary>
        public float cooldownReduce;

        /// <summary>
        /// X skill projectile count bonus
        /// </summary>
        public int projectileCountBonus;

        /// <summary>
        /// X% skill projectile speed bonus
        /// </summary>
        public float projectileSpeedBonus;

        /// <summary>
        /// X% skill area bonus
        /// </summary>
        public float skillAreaBonus;

        /// <summary>
        /// X pickup range
        /// </summary>
        public float pickupRange;

        /// <summary>
        /// X seconds duration of skill bonus
        /// </summary>
        public float skillDurationBonus;

        /// <summary>
        /// X% coin received bonus
        /// </summary>
        public float greed;

        /// <summary>
        /// X% crit chance
        /// </summary>
        public float critChance;

        /// <summary>
        /// X% crit damage
        /// </summary>
        public float critDamage;

        /// <summary>
        /// X times revive
        /// </summary>
        public int reviveCount;

        public void ImportBlueprint(CharacterBlueprint characterBlueprint)
        {
            health = characterBlueprint.health;
            recoveryPerSecond = characterBlueprint.recoveryPerSecond;
            speed = characterBlueprint.speed;
            armor = characterBlueprint.armor;
            skillDamageBonus = characterBlueprint.skillDamageBonus;
            experienceBonus = characterBlueprint.experienceBonus;
            cooldownReduce = characterBlueprint.cooldownReduce;
            projectileCountBonus = characterBlueprint.projectileCountBonus;
            projectileSpeedBonus = characterBlueprint.projectileSpeedBonus;
            skillAreaBonus = characterBlueprint.skillAreaBonus;
            pickupRange = characterBlueprint.pickupRange;
            skillDurationBonus = characterBlueprint.skillDurationBonus;
            greed = characterBlueprint.greed;
            critChance = characterBlueprint.critChance;
            critDamage = characterBlueprint.critDamage;
        }
    }
}
