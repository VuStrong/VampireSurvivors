using TMPro;
using UnityEngine;

namespace ProjectCode1.Utils
{
    public class FPSDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private float interval;
        private float timer;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                float fps = 1 / Time.unscaledDeltaTime;
                fpsText.text = Mathf.Ceil(fps).ToString();
                timer = 0f;
            }
        }
    }
}
