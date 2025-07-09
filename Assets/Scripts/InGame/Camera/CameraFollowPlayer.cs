using UnityEngine;

namespace ProjectCode1.InGame
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        public Transform target;

        void Update()
        {
            if (target == null) return;

            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
