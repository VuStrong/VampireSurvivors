using UnityEngine;

namespace ProjectCode1.Utils
{
    public class LoadingOverlay : MonoBehaviour
    {
        public static LoadingOverlay Instance { get; private set; }

        void Awake()
        {
            Instance = this;
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
