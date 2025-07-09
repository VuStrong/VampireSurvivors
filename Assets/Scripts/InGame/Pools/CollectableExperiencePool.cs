using ProjectCode1.InGame.Collectables;
using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Pools
{
    public class CollectableExperiencePool : MonoBehaviour
    {
        [SerializeField] private CollectableExperience prefab;
        private ObjectPool<CollectableExperience> pool;

        void Awake()
        {
            pool = new ObjectPool<CollectableExperience>(Create, OnGet, OnRelease, OnDestroyPoolItem, defaultCapacity: 50);
        }

        public CollectableExperience Get()
        {
            return pool.Get();
        }

        public void Release(CollectableExperience exp)
        {
            pool.Release(exp);
        }

        CollectableExperience Create()
        {
            return Instantiate(prefab, transform);
        }

        void OnGet(CollectableExperience exp)
        {
            exp.gameObject.SetActive(true);
        }

        void OnRelease(CollectableExperience exp)
        {
            exp.gameObject.SetActive(false);
        }

        void OnDestroyPoolItem(CollectableExperience exp)
        {
            Destroy(exp.gameObject);
        }
    }
}
