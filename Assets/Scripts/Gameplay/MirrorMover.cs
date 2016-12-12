using Assets.Scripts.Utility;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts.Gameplay
{
    public class MirrorMover : MonoBehaviour
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public float MinimumVerticalAngle = -90F;
        public float MaximumVerticalAngle = 90F;

        private bool _active;
        private Camera _camera;
        private Camera _playerCamera;
        private Transform _innerWheel;
        private GameObject _player;
        
        void Start ()
        {
            _playerCamera = Camera.main;
            _player = GameObject.FindWithTag(Tags.Player);

            _camera = GetComponentInChildren<Camera>();
            _camera.gameObject.SetActive(false);
            _innerWheel = transform.GetChild(0);
            _active = false;
        }
        
        void Update ()
        {
            if (_active)
            {
                var horizontalRotation = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
                var verticalRotation = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

                transform.localRotation *= Quaternion.Euler(0f, 0f, -horizontalRotation);

                // Inner wheel rotates by vertical mouse
                _innerWheel.localRotation =
                    Utilities.ClampRotationAroundXAxis(_innerWheel.localRotation*Quaternion.Euler(-verticalRotation, 0f, 0f), 
                    MinimumVerticalAngle,
                    MaximumVerticalAngle);

                if (CrossPlatformInputManager.GetButtonDown("Fire1"))
                    Release();
            }
        }

        void OnInteract(Interactor interactor)
        {
            if(interactor.GrabbedObject != null)
                return;

            Debug.Log("Mirror activating");
            _active = true;
            _camera.gameObject.SetActive(true);
            _player.SetActive(false);
        }

        void Release()
        {
            _active = false;
            _player.SetActive(true);
            _camera.gameObject.SetActive(false);
        }
    }
}
