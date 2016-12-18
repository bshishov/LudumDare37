using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(BoxCollider))]
    public class GravityField : MonoBehaviour
    {
        public const string TriggerEnterMessage = "OnGravityFieldEnter";
        public const string TriggerExitMessage = "OnGravityFieldExit";
        public Vector3 LocalDirection = Vector3.down;

        private BoxCollider _collider;

        void Start ()
        {
            _collider = GetComponent<BoxCollider>();

            if (!_collider.isTrigger)
                Debug.LogWarning("Collider should be a trigger");
        }

        void OnTriggerEnter(Collider collider)
        {
            collider.SendMessage(TriggerEnterMessage, this, SendMessageOptions.DontRequireReceiver);
        }

        void OnTriggerExit(Collider collider)
        {
            collider.SendMessage(TriggerExitMessage, this, SendMessageOptions.DontRequireReceiver);
        }

        void OnDrawGizmos()
        {
            var col = GetComponent<BoxCollider>();
            if (col != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
                Gizmos.DrawRay(col.bounds.center, transform.TransformDirection(LocalDirection));
            }
        }
    }
}
