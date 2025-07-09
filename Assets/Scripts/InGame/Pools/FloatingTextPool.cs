using UnityEngine;
using UnityEngine.Pool;

namespace ProjectCode1.InGame.Pools
{
    public class FloatingTextPool : MonoBehaviour
    {
        [SerializeField] private FloatingText prefab;
        private ObjectPool<FloatingText> pool;

        void Awake()
        {
            pool = new ObjectPool<FloatingText>(Create, OnGet, OnRelease, OnDestroyPoolItem);
        }

        public FloatingText Get()
        {
            return pool.Get();
        }

        public void Release(FloatingText text)
        {
            pool.Release(text);
        }

        FloatingText Create()
        {
            return Instantiate(prefab, transform);
        }

        void OnGet(FloatingText text)
        {
            text.gameObject.SetActive(true);
        }

        void OnRelease(FloatingText text)
        {
            text.gameObject.SetActive(false);
        }

        void OnDestroyPoolItem(FloatingText text)
        {
            Destroy(text.gameObject);
        }
    }
}