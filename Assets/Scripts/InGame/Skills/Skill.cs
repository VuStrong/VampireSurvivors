using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectCode1.InGame.Characters;
using UnityEngine;
using UnityEngine.Localization;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Base class for all Skill in game
    /// </summary>
    public abstract class Skill : MonoBehaviour
    {
        [Header("Skill Info")]
        [SerializeField] private string id;
        [SerializeField] protected Sprite image;
        [SerializeField] protected LocalizedString localizedName;
        [SerializeField] protected LocalizedString localizedDescription;
        [SerializeField] protected LocalizedString[] localizedUpgradeDescriptions;
        [SerializeField] protected SkillRequirement[] requiredSkills;

        protected Character owner;
        protected int level = 1;
        protected int maxLevel;
        protected List<IUpgradeableStat> upgradeableStats;

        public string Id { get => id; }
        public int Level { get => level; }
        public Sprite Image { get => image; }
        public string Name { get => localizedName.GetLocalizedString(); }
        public string Description { get => localizedDescription.GetLocalizedString(); }
        public bool CanBeUpgrade { get => owner != null && level < maxLevel; }
        public Character Owner { get => owner; }
        public bool Owned { get => owner != null; }
        public bool IsCombinedSkill { get => requiredSkills.Length > 0; }
        public IReadOnlyList<SkillRequirement> RequiredSkills { get => Array.AsReadOnly(requiredSkills); }

        protected virtual void Awake()
        {
            upgradeableStats = GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(f => typeof(IUpgradeableStat).IsAssignableFrom(f.FieldType))
                .Select(f => f.GetValue(this) as IUpgradeableStat)
                .ToList();

            maxLevel = upgradeableStats.Any() ? upgradeableStats.Max(s => s.UpgradeCount) + 1 : 1;
        }

        public virtual void Init(Character owner)
        {
            if (Owned) return;
            this.owner = owner;

            transform.parent = owner.transform;
            transform.localPosition = Vector3.zero;
        }

        public virtual void Upgrade()
        {
            if (!CanBeUpgrade) return;
            level++;

            foreach (var stat in upgradeableStats)
            {
                stat.Upgrade();
            }
        }

        public string GetUpgradeDescription()
        {
            if (!CanBeUpgrade) return "";

            return localizedUpgradeDescriptions[level - 1].GetLocalizedString();
        }

        /// <summary>
        /// Calculate final damage, apply it to target with effects and return final damage
        /// </summary>
        public float ApplyDamage(float damage, IDamageable damageable, float knockback = 0)
        {
            damage += owner.stats.skillDamageBonus / 100 * damage;

            bool crit = UnityEngine.Random.value <= owner.stats.critChance / 100;
            if (crit) damage += owner.stats.critDamage / 100 * damage;

            damage = Mathf.Ceil(damage);

            if (knockback > 0)
            {
                Vector2 knockbackDirection = (damageable.Position - owner.transform.position).normalized;
                damageable.TakeDamage(damage, knockbackDirection * knockback, isCrit: crit);
            }
            else
            {
                damageable.TakeDamage(damage, isCrit: crit);
            }

            return damage;
        }

        [Serializable]
        public class SkillRequirement
        {
            public Skill skill;
            public int requiredLevel;
        }
    }
}
