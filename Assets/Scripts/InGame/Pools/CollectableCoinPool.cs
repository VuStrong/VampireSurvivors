using ProjectCode1.InGame.Collectables;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Pools
{
    public class CollectableCoinPool : MonoBehaviour
    {
        [SerializeField] private CollectableCoin prefab;
        private ObjectPool<CollectableCoin> pool;

        void Awake()
        {
            pool = new ObjectPool<CollectableCoin>(Create, OnGet, OnRelease, OnDestroyPoolItem);
        }

        public CollectableCoin Get()
        {
            return pool.Get();
        }

        public void Release(CollectableCoin coin)
        {
            pool.Release(coin);
        }

        CollectableCoin Create()
        {
            return Instantiate(prefab, transform);
        }

        void OnGet(CollectableCoin coin)
        {
            coin.gameObject.SetActive(true);
        }

        void OnRelease(CollectableCoin coin)
        {
            coin.gameObject.SetActive(false);
        }

        void OnDestroyPoolItem(CollectableCoin coin)
        {
            Destroy(coin.gameObject);
        }
    }
}
