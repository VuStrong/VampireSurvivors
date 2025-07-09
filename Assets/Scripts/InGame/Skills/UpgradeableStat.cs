using System;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    public interface IUpgradeableStat
    {
        int UpgradeCount { get; }

        void Upgrade();
    }

    public abstract class UpgradeableStat<T> : IUpgradeableStat
    {
        [SerializeField] protected T value;
        [SerializeField] protected T[] upgrades;

        protected int level = 0;
        public T Value { get => value; }
        public int UpgradeCount { get => upgrades.Length; }
        public T CurrentUpgrade { get => level > 0 ? upgrades[level - 1] : default; }
        public Action OnUpgraded;

        public void Upgrade()
        {
            if (level >= upgrades.Length) return;
            Upgrade(upgrades[level]);
            level++;
            OnUpgraded?.Invoke();
        }

        protected abstract void Upgrade(T valueToUpgrade);
    }

    [Serializable]
    public class UpgradeableFloatStat : UpgradeableStat<float>
    {
        [SerializeField] private UpgradeType type;
        [SerializeField] private UpgradeOperation operation;

        protected override void Upgrade(float valueToUpgrade)
        {
            if (valueToUpgrade <= 0) return;

            if (type == UpgradeType.Percentage)
            {
                valueToUpgrade = valueToUpgrade / 100 * value;
            }

            if (operation == UpgradeOperation.Add)
            {
                value += valueToUpgrade;
            }
            else
            {
                value -= valueToUpgrade;
            }

            if (value < 0) value = 0;
        }

        public enum UpgradeType
        {
            Percentage,
            Absolute,
        }
    }

    [Serializable]
    public class UpgradeableIntStat : UpgradeableStat<int>
    {
        [SerializeField] private UpgradeOperation operation;

        protected override void Upgrade(int valueToUpgrade)
        {
            if (valueToUpgrade <= 0) return;

            if (operation == UpgradeOperation.Add)
            {
                value += valueToUpgrade;
            }
            else
            {
                value -= valueToUpgrade;
            }

            if (value < 0) value = 0;
        }
    }

    public enum UpgradeOperation
    {
        Add,
        Subtract,
    }
}