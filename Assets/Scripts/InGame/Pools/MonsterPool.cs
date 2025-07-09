using ProjectCode1.InGame.Monsters;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Pools
{
    public class MonsterPool : MonoBehaviour
    {
        private Monster prefab;
        private ObjectPool<Monster> pool;

        public void Setup(Monster prefab, int defaultCapacity = 30)
        {
            this.prefab = prefab;
            pool = new ObjectPool<Monster>(Create, OnGet, OnRelease, OnDestroyPoolItem, defaultCapacity: defaultCapacity);
        }

        public Monster Get()
        {
            return pool.Get();
        }

        public void Release(Monster monster)
        {
            pool.Release(monster);
        }

        Monster Create()
        {
            return Instantiate(prefab, transform);
        }

        void OnGet(Monster monster)
        {
            monster.gameObject.SetActive(true);
        }

        void OnRelease(Monster monster)
        {
            monster.gameObject.SetActive(false);
        }

        void OnDestroyPoolItem(Monster monster)
        {
            Destroy(monster.gameObject);
        }
    }
}
