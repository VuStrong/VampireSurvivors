using System.Collections.Generic;
using ProjectCode1.InGame.Monsters;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// Skill1029: BouncingBullet - 
    /// Idle: X bullets bouncing continuously between enemies, small damage, slow speed --- 
    /// Moving: bullets bouncing between certain enemies and return to player. More damage, speed.
    /// </summary>
    public class Skill1029 : Skill
    {
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private Skill1029Bullet bulletPrefab;
        [SerializeField] private float bulletSpeed;

        private readonly List<Skill1029Bullet> bullets = new();
        private readonly List<Monster> visibleMonsters = new();
        private bool ownerIsMoving;
        private bool bulletDisabled;

        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public IReadOnlyList<Monster> VisibleMonsters { get => visibleMonsters; }

        void Start()
        {
            FindNewMonsters();
        }

        void OnDisable()
        {
            foreach (var bullet in bullets)
            {
                if (bullet != null)
                {
                    Destroy(bullet.gameObject);
                }
            }
            bullets.Clear();
        }

        void Update()
        {
            CheckBullets();
            CheckMonsters();

            if (ownerIsMoving != owner.IsMoving)
            {
                ownerIsMoving = owner.IsMoving;
                ChangeBulletsState();
            }
        }

        void CheckBullets()
        {
            int count = TotalProjectileCount;
            if (count == bullets.Count) return;

            if (bullets.Count < count)
            {
                int amountToAdd = count - bullets.Count;
                for (int i = 0; i < amountToAdd; i++)
                {
                    var position = owner.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                    var bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
                    bullet.Init(this);
                    bullet.speed = bulletSpeed;
                    bullets.Add(bullet);
                }
            }
            else
            {
                int amountToDestroy = bullets.Count - count;
                for (int i = 0; i < amountToDestroy; i++)
                {
                    int index = bullets.Count - 1;
                    Destroy(bullets[index].gameObject);
                    bullets.RemoveAt(index);
                }
            }
        }

        void FindNewMonsters(int max = 20)
        {
            var monsters = EntityManager.Instance.GetClosestMonsters(owner.transform.position, max, true);
            visibleMonsters.Clear();
            visibleMonsters.AddRange(monsters);
        }

        /// <summary>
        /// Check each monster in the list, if it outside of the screen or dead, remove it from the list.
        /// If there are few monsters left, find new
        /// If cannot find any monsters on screen, disable all bullets
        /// </summary>
        void CheckMonsters()
        {
            for (int i = visibleMonsters.Count - 1; i >= 0; i--)
            {
                var monster = visibleMonsters[i];
                if (!monster.Alive || !EntityManager.Instance.PositionOnScreen(monster.transform.position))
                {
                    visibleMonsters.RemoveAt(i);
                }
            }

            if (visibleMonsters.Count <= Mathf.Clamp(bullets.Count / 2, 0, 5))
            {
                FindNewMonsters();
            }

            if (visibleMonsters.Count == 0 && !bulletDisabled)
            {
                foreach (var bullet in bullets)
                {
                    if (bullet.gameObject.activeSelf)
                        bullet.gameObject.SetActive(false);
                }
                bulletDisabled = true;
            }
            else if (visibleMonsters.Count > 0 && bulletDisabled)
            {
                foreach (var bullet in bullets)
                {
                    if (!bullet.gameObject.activeSelf)
                        bullet.gameObject.SetActive(true);
                }
                bulletDisabled = false;
            }
        }

        void ChangeBulletsState()
        {
            if (ownerIsMoving)
            {
                foreach (var bullet in bullets)
                {
                    bullet.speed = bulletSpeed * 2;
                    bullet.maxBounce = 5;
                }
            }
            else
            {
                foreach (var bullet in bullets)
                {
                    bullet.speed = bulletSpeed;
                    bullet.maxBounce = 0;
                }
            }
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(ownerIsMoving ? damage.Value * 2 : damage.Value, damageable);
        }
    }
}
