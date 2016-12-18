using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class DirectionalGravity : MonoBehaviour
    {
        public const float Gravity = 9.8f;
        private Rigidbody _rigidbody;
        private readonly List<GravityField> _fields = new List<GravityField>();

        void Start ()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        void FixedUpdate()
        {
            var forceDirection = new Vector3();
            foreach (var gravityField in _fields)
            {
                forceDirection += gravityField.transform.TransformDirection(gravityField.LocalDirection);
            }
            
            _rigidbody.AddForce(forceDirection.normalized * Gravity * _rigidbody.mass - Physics.gravity * _rigidbody.mass);

#if DEBUG
            Debug.DrawRay(transform.position, forceDirection.normalized);
#endif 
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(Tags.GravityField))
            {
                var field = col.GetComponent<GravityField>();
                if(field != null && !_fields.Contains(field))
                    _fields.Add(field);
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.CompareTag(Tags.GravityField))
            {
                var field = col.GetComponent<GravityField>();
                if (field != null && _fields.Contains(field))
                    _fields.Remove(field);
            }
        }

        // From Utilities
        void OnRigidBodyEnabled()
        {
        }

        // From Utilities
        void OnRigidBodyDisabled()
        {
            _fields.Clear();
        }
    }
}
