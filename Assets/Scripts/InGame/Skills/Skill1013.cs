using System.Collections.Generic;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1013: Swords - Idle: X swords rotate around player
    /// </summary>
    public class Skill1013 : Skill
    {
        [Space(8)]
        [Header("Stats")]
        [SerializeField] private UpgradeableIntStat swordCount;
        [SerializeField] private UpgradeableFloatStat area;
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableFloatStat speed;
        [SerializeField] private float knockback;
        [SerializeField] private Skill1013Sword swordPrefab;

        private readonly List<Skill1013Sword> swords = new();
        public int TotalSwordCount { get => owner.stats.projectileCountBonus + swordCount.Value; }
        public float FinalSpeed { get => speed.Value * (1 + owner.stats.projectileSpeedBonus / 100); }

        void Start()
        {
            RecreateSwords();
        }

        void Update()
        {
            if (!Owned) return;

            if (TotalSwordCount != swords.Count)
                RecreateSwords();

            transform.RotateAround(transform.position, Vector3.back, Time.deltaTime * FinalSpeed);
        }

        void RecreateSwords()
        {
            if (swords.Count > 0)
            {
                foreach (var sword in swords)
                {
                    Destroy(sword.gameObject);
                }
                swords.Clear();
            }

            int count = TotalSwordCount;
            for (int i = 0; i < count; i++)
            {
                var sword = Instantiate(swordPrefab, transform, false);
                sword.Init(this);
                swords.Add(sword);
            }

            ChangeToRotateMode();
        }

        void ChangeToRotateMode()
        {
            transform.localPosition = Vector3.zero;

            int count = swords.Count;
            float finalArea = area.Value * (1 + owner.stats.skillAreaBonus / 100);
            for (int i = 0; i < count; i++)
            {
                float angle = i * (360 / count);

                swords[i].gameObject.SetActive(true);
                swords[i].transform.localRotation = Quaternion.Euler(0, 0, angle);
                swords[i].transform.localScale = new Vector3(finalArea, finalArea, 1);
                swords[i].SetOpacity(1);
            }
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable, knockback);
        }
    }
}
