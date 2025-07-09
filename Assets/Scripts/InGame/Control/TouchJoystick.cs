using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ProjectCode1.InGame
{
    public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        [Tooltip("If true, the joystick always stick in the first position")]
        private bool permanent = false;
        [SerializeField] private float joystickRadius;
        [SerializeField] private RectTransform joystick, joystickBounds;
        public UnityEvent<Vector2> onJoystickMoved;
        public UnityEvent onStartTouch, onEndTouch;

        private RectTransform controlRect;
        private bool beingTouched = false;
        private Vector2 initialTouchPosition;

        public bool BeingTouched { get => beingTouched; }

        void Awake()
        {
            controlRect = GetComponent<RectTransform>();

            joystick.gameObject.SetActive(permanent);
            joystickBounds.gameObject.SetActive(permanent);
        }

        void Update()
        {
            if (beingTouched)
            {
                if (Time.timeScale > 0)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(controlRect, Input.mousePosition, null, out Vector2 touchPosition);
                    UpdateTouch(touchPosition);
                }
                else
                {
                    EndTouch();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(controlRect, eventData.position, null, out Vector2 touchPosition);
            StartTouch(permanent ? joystick.localPosition : touchPosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EndTouch();
        }

        public void StartTouch(Vector2 touchPosition)
        {
            if (Time.timeScale > 0)
            {
                beingTouched = true;

                // Save the initial touch position
                initialTouchPosition = touchPosition;

                // Set the joystick position
                joystick.localPosition = initialTouchPosition;
                joystickBounds.localPosition = initialTouchPosition;

                // Size the joystick bounds
                joystickBounds.sizeDelta = 2 * joystickRadius * Vector2.one;

                // Display the joystick
                joystick.gameObject.SetActive(true);
                joystickBounds.gameObject.SetActive(true);

                onStartTouch.Invoke();
            }
        }

        public void UpdateTouch(Vector2 touchPosition)
        {
            Vector2 joystickDelta = touchPosition - initialTouchPosition;
            Vector2 moveDirection = joystickDelta.normalized;

            // Update the joystick position, locking it within the joystick bounds
            joystick.localPosition = joystickDelta.magnitude > joystickRadius ? initialTouchPosition + moveDirection * joystickRadius : touchPosition;

            onJoystickMoved.Invoke(moveDirection);
        }

        public void EndTouch()
        {
            joystick.localPosition = joystickBounds.localPosition;

            // Hide the joystick
            joystick.gameObject.SetActive(permanent);
            joystickBounds.gameObject.SetActive(permanent);

            onJoystickMoved.Invoke(Vector2.zero);
            onEndTouch.Invoke();
            beingTouched = false;
        }
    }
}
