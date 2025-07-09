using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCode1.InGame.Skills
{
    /// <summary>
    /// ProjectileAround - 
    /// Idle: Projectiles circle around player deal damage --- 
    /// Moving: Projectiles shot from character in circle repeatedly
    /// </summary>
    public class Skill1028 : Skill
    {
        [SerializeField] private UpgradeableIntStat projectileCount;
        [SerializeField] private UpgradeableFloatStat projectileSpeed;
        [SerializeField] private UpgradeableFloatStat damage;
        [SerializeField] private Skill1028Projectile projectilePrefab;
        [SerializeField] private float radius;

        private readonly List<Skill1028Projectile> projectiles = new();
        private float angleZ;
        private bool isShooting;
        private bool justShooted;

        public int TotalProjectileCount { get => owner.stats.projectileCountBonus + projectileCount.Value; }
        public float FinalProjectileSpeed { get => projectileSpeed.Value * (1 + owner.stats.projectileSpeedBonus / 100); }

        void OnDisable()
        {
            foreach (var projectile in projectiles)
            {
                if (projectile != null)
                {
                    Destroy(projectile.gameObject);
                }
            }
            projectiles.Clear();
        }

        void Update()
        {
            if (!Owned) return;

            CheckProjectiles();

            angleZ -= FinalProjectileSpeed / radius * Time.deltaTime;

            if (owner.IsMoving && !isShooting)
            {
                StartCoroutine(Shoot());
            }
            else if (!isShooting)
            {
                MoveProjectilesAround();
            }
        }

        void CheckProjectiles()
        {
            int count = TotalProjectileCount;
            if (count == projectiles.Count) return;

            if (projectiles.Count < count)
            {
                int amountToAdd = count - projectiles.Count;
                for (int i = 0; i < amountToAdd; i++)
                {
                    var projectile = Instantiate(projectilePrefab);
                    projectile.Init(this);
                    projectiles.Add(projectile);
                }
            }
            else
            {
                int amountToDestroy = projectiles.Count - count;
                for (int i = 0; i < amountToDestroy; i++)
                {
                    int index = projectiles.Count - 1;
                    Destroy(projectiles[index].gameObject);
                    projectiles.RemoveAt(index);
                }
            }
        }

        void MoveProjectilesAround()
        {
            int count = projectiles.Count;
            for (int i = 0; i < count; i++)
            {
                float angle = i * (2 * Mathf.PI / count) + angleZ;
                Vector3 offset = new(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                projectiles[i].transform.position = owner.transform.position + offset;

                if (justShooted)
                {
                    projectiles[i].ClearTrail();
                    projectiles[i].SetTrailTime(0.2f);
                }
            }

            justShooted = false;
        }

        IEnumerator Shoot()
        {
            isShooting = true;
            int count = projectiles.Count;
            List<Coroutine> coroutines = new();

            for (int i = 0; i < count; i++)
            {
                float angle = i * (2 * Mathf.PI / count) + angleZ;
                Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));

                projectiles[i].transform.position = owner.transform.position;
                projectiles[i].ClearTrail();
                projectiles[i].SetTrailTime(0.7f);
                coroutines.Add(StartCoroutine(projectiles[i].ShootByDirection(direction)));
            }

            yield return new WaitForSeconds(1);

            foreach (var coroutine in coroutines)
            {
                StopCoroutine(coroutine);
            }

            isShooting = false;
            justShooted = true;
        }

        public void ApplyDamage(IDamageable damageable)
        {
            ApplyDamage(damage.Value, damageable);
        }
    }
}
