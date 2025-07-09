using UnityEngine;
using UnityEngine.Events;

namespace ProjectCode1.InGame
{
    public class KeyboardMovement : MonoBehaviour
    {
        public UnityEvent<Vector2> onMoved;
        private Vector2 lastDirection;

        void Update()
        {
            Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (lastDirection != direction)
            {
                lastDirection = direction;
                onMoved.Invoke(direction);
            }
        }
    }
}
