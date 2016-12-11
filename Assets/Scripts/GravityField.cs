using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(BoxCollider))]
    public class GravityField : MonoBehaviour
    {
        public const string TriggerEnterMessage = "OnGravityFieldEnter";
        public const string TriggerExitMessage = "OnGravityFieldExit";
        public const float Gravity = 9.8f;

        public Vector3 LocalDirection = Vector3.down;

        private BoxCollider _collider;
        

        void Start ()
        {
            _collider = GetComponent<BoxCollider>();

            if (!_collider.isTrigger)
                Debug.LogWarning("Collider should be a trigger");
            
        }
        
        void Update ()
        {
	        
        }

        void OnTriggerEnter(Collider collider)
        {
            collider.SendMessage(TriggerEnterMessage, this, SendMessageOptions.DontRequireReceiver);
        }

        void OnTriggerStay(Collider collider)
        {
            var body = collider.attachedRigidbody;
            if (body != null)
            {
                var direction = transform.TransformDirection(LocalDirection);
                Debug.DrawRay(body.transform.position, direction * Gravity, Color.blue);

                // Negate the unity gravity and apply custom
                body.AddForce(direction * Gravity * body.mass - Physics.gravity * body.mass);
            }
        }


        void OnTriggerExit(Collider collider)
        {
            collider.SendMessage(TriggerExitMessage, this, SendMessageOptions.DontRequireReceiver);
        }

        void OnDrawGizmos()
        {
            var collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
                Gizmos.DrawRay(collider.bounds.center, transform.TransformDirection(LocalDirection));
            }
        }
    }
}
