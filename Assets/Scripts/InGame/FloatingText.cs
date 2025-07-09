using System.Collections;
using TMPro;
using UnityEngine;

namespace ProjectCode1.InGame
{
    public class FloatingText : MonoBehaviour
    {
        private TMP_Text text;

        void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        public void Setup(Vector2 position, string text, Color? color = null, float size = 2.5f)
        {
            transform.position = position;
            this.text.text = text;
            this.text.color = color != null ? color.Value : Color.white;
            this.text.fontSize = size;

            StopAllCoroutines();
            StartCoroutine(Animate());
        }

        IEnumerator Animate()
        {
            float t = 0;
            while (t < 1)
            {
                transform.position += .5f * Time.deltaTime * Vector3.up;
                // transform.localScale = Vector3.one * EaseOutBack(1 - t);
                yield return null;
                t += Time.deltaTime * 2;
            }
            EntityManager.Instance.DespawnFloatingText(this);
        }

        // float EaseOutBack(float n)
        // {
        //     float c = 1.70158f;
        //     float c3 = c + 1;
        //     return 1 + c3 * Mathf.Pow(n - 1, 3) + c * Mathf.Pow(n - 1, 2);
        // }
    }
}
