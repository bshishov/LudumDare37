using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts
{
    public class Interactor : MonoBehaviour
    {
        public float InteractDistance = 1f;

        private Transform _grabbingTransform;
        private Transform _cameraTransform;
        private GameObject _objectToInteract;
        private GameObject _grabbedObject;


        void Start ()
        {
            _cameraTransform = transform.FindChild("MainCamera");
            if (_cameraTransform == null)
            {
                Debug.LogWarning("Couldn't find MainCamera");
            }
            else
            {
                _grabbingTransform = _cameraTransform.FindChild("Grabber");
                if (_grabbingTransform == null)
                    Debug.LogWarning("Couldn't find Grabber");
            }
        }
        
        void Update ()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Interact();    
            }
        }

        void FixedUpdate()
        {
            RaycastHit hit;
            if (Physics.Raycast(
                    new Ray(_cameraTransform.position, _cameraTransform.TransformDirection(Vector3.forward)),
                    out hit, InteractDistance))
            {
                if (hit.collider.CompareTag(Tags.Interactable))
                    _objectToInteract = hit.collider.gameObject;
            }
            else
            {
                _objectToInteract = null;
            }
        }

        void Interact()
        {
            if (_grabbedObject == null)
            {
                if (_objectToInteract != null)
                    TakeObject(_objectToInteract);
            }
            else
            {
                if(_grabbedObject != null)
                    DropObject(_grabbedObject);
            }
        }

        void TakeObject(GameObject obj)
        {
            var body = obj.GetComponent<Rigidbody>();
            if(body != null)
                body.isKinematic = true;
            
            var col = obj.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            obj.transform.SetParent(_grabbingTransform, true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _grabbedObject = obj;
            obj.SendMessage("OnPickedUp", this, SendMessageOptions.DontRequireReceiver);
        }

        void DropObject(GameObject obj)
        {
            var body = obj.GetComponent<Rigidbody>();
            if (body != null)
                body.isKinematic = false;

            var col = obj.GetComponent<Collider>();
            if (col != null)
                col.enabled = true;

            obj.transform.SetParent(null, true);
            _grabbedObject = null;

            obj.SendMessage("OnDropped", this, SendMessageOptions.DontRequireReceiver);
        }
    }
}
