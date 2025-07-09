using UnityEngine;
using UnityEngine.UI;

namespace ProjectCode1.MainMenu
{
    public class Navigation : MonoBehaviour
    {
        [SerializeField] private int defaultActiveIndex;
        [SerializeField] private GameObject[] screens;
        [SerializeField] private Button[] items;

        void Start()
        {
            SwitchToScreen(defaultActiveIndex);
        }

        void OnEnable()
        {
            for (int i = 0; i < items.Length; i++)
            {
                int index = i;
                items[index].onClick.AddListener(() => SwitchToScreen(index));
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < items.Length; i++)
            {
                int index = i;
                items[index].onClick.RemoveAllListeners();
            }
        }

        public void SwitchToScreen(int index)
        {
            for (int i = 0; i < screens.Length; i++)
            {
                if (i == index)
                {
                    screens[i].SetActive(true);
                    items[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1.1f, 1);
                }
                else
                {
                    screens[i].SetActive(false);
                    items[i].GetComponent<RectTransform>().localScale = Vector3.one;
                }
            }
        }
    }
}