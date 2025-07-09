using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace ProjectCode1.Utils
{
    public class Toast : MonoBehaviour
    {
        public static Toast Instance { get; private set; }

        private Coroutine showUpCoroutine;
        private Image image;
        private TMP_Text text;

        void Awake()
        {
            Instance = this;
            image = GetComponent<Image>();
            text = GetComponentInChildren<TMP_Text>();
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
        }

        IEnumerator ShowUpCoroutine(float duration)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            Vector2 startPos = transform.position - new Vector3(0, 15f);
            Vector2 endPos = transform.position;
            transform.position = startPos;

            float curTime = 0;
            float totalTime = 0.2f;
            while (curTime < totalTime)
            {
                float t = curTime / totalTime;

                transform.position = Vector2.LerpUnclamped(startPos, endPos, t);
                image.color = new Color(image.color.r, image.color.g, image.color.b, t);

                curTime += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Show toast with a string message
        /// </summary>
        public void Show(string message, float duration = 2)
        {
            text.text = message;
            gameObject.SetActive(true);
            if (showUpCoroutine != null) StopCoroutine(showUpCoroutine);
            showUpCoroutine = StartCoroutine(ShowUpCoroutine(duration));
        }

        /// <summary>
        /// Show toast with a string take from LocalizedStringDatabase with key
        /// </summary>
        private void Show(string tableName, string key, float duration = 2)
        {
            var message = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);
            Show(message, duration);
        }


        ///////////////////////////////////////////
        //// Special messages
        ///////////////////////////////////////////
        public void ShowCopiedMessage(float duration = 2)
        {
            Show("Message Text", "Copied", duration);
        }

        public void ShowNotEnoughCoinMessage(float duration = 2)
        {
            Show("Message Text", "NotEnoughCoin", duration);
        }

        public void ShowInvalidUsernameMessage(float duration = 2)
        {
            Show("Message Text", "InvalidUsername", duration);
        }
    }
}
