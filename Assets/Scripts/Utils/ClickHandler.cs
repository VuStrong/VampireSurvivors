using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ProjectCode1.Utils
{
    public class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent onClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (gameObject == eventData?.pointerCurrentRaycast.gameObject)
            {
                onClick?.Invoke();
            }
        }
    }
}
