using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts
{
    public class GravityLooker : MonoBehaviour
    {
        private List<GravityField> _fields = new List<GravityField>();

        public Vector3 GravityDirection;
        public Vector3 LocalForward = Vector3.forward;
        public Vector3 LocalDown = Vector3.down;

        public float MinimumVerticalAngle = -90F;
        public float MaximumVerticalAngle = 90F;


        [Range(0.2f, 5f)]
        public float RotationSpeed = 2f;
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        
        private Transform _cameraTransform;

        void Start ()
        {
            _cameraTransform = transform.GetChild(0).transform;
            LockMouse();
        }
        
        void Update ()
        {
            if (Input.GetMouseButtonDown(0))
                LockMouse();

            if (Input.GetKeyDown(KeyCode.Escape))
                UnlockMouse();
            

            var alignment = GetGravityAlignment();

            var horizontalRotation = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            var verticalRotation = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            _cameraTransform.localRotation =
                Utilities.ClampRotationAroundXAxis(
                _cameraTransform.localRotation * 
                Quaternion.Euler(-alignment.x, 0, 0) * 
                Quaternion.Euler(-verticalRotation, 0f, 0f), MinimumVerticalAngle, MaximumVerticalAngle);


            transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * alignment,
                Time.deltaTime*RotationSpeed);
            transform.localRotation *= Quaternion.Euler(0f, horizontalRotation, 0f);
        }

        Quaternion GetGravityAlignment()
        {
            var gravity = ComputeGravity();
            GravityDirection = gravity.normalized;
            Debug.DrawRay(transform.position, transform.TransformDirection(LocalDown).normalized, Color.yellow);
            Debug.DrawRay(transform.position, GravityDirection.normalized, Color.red);

            return Quaternion.FromToRotation(LocalDown, transform.InverseTransformDirection(GravityDirection));
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(Tags.GravityField))
            {
                var field = collider.GetComponent<GravityField>();
                if (field != null && !_fields.Contains(field))
                    _fields.Add(field);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag(Tags.GravityField))
            {
                var field = collider.GetComponent<GravityField>();
                if (field != null && _fields.Contains(field))
                    _fields.Remove(field);
            }
        }

        private Vector3 ComputeGravity()
        {
            var dir = Vector3.zero;
            if (_fields.Count > 0)
            {
                foreach (var gravityField in _fields)
                {
                    dir += gravityField.transform.TransformDirection(gravityField.LocalDirection);
                }

                dir /= _fields.Count;
            }

            return dir;
        }

        private void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void UnlockMouse()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
       
    }
}
