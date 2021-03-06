﻿using System.Collections;
using Assets.Scripts.Gameplay;
using Assets.Scripts.UI;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts
{
    public class Interactor : MonoBehaviour
    {
        public float InteractDistance = 1f;

        public GameObject GrabbedObject
        {
            get { return _grabbedObject; }
        }

        private Transform _grabbingTransform;
        private Transform _cameraTransform;
        private GameObject _objectToInteract;
        private GameObject _grabbedObject;
        private Helper _helper;

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

            _helper = GameObject.FindObjectOfType<Helper>();
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
                {
                    _objectToInteract = hit.collider.gameObject;

                    if (_helper != null)
                    {
                        if (_objectToInteract.GetComponent<Artifact>() != null)
                        {
                            _helper.Show("Take");
                        }
                        else if (_objectToInteract.GetComponent<ArtifactSlot>() != null)
                        {
                            if(_grabbedObject != null)
                                _helper.Show("Place");
                            else if(_objectToInteract.GetComponent<ArtifactSlot>().HasObject)
                                _helper.Show("Take");
                        }
                        else if(_objectToInteract.GetComponent<MirrorMover>() != null)
                        {
                            _helper.Show("Move mirror");
                        }
                        else if(_objectToInteract.GetComponent<Door>() != null)
                        {
                            var door = _objectToInteract.GetComponent<Door>();
                            if (door.State == Door.DoorState.Closed)
                                _helper.Show("Open");
                            else if(door.State == Door.DoorState.Opened)
                                _helper.Show("Close");
                        }
                        else
                        {
                            _helper.Show("Interact");
                        }
                    }
                }
            }
            else
            {
                if (_grabbedObject != null)
                {
                    _helper.Show("Drop");
                }
                _objectToInteract = null;

                if(_helper != null)
                    _helper.Hide();
            }
        }

        void Interact()
        {
            if (_objectToInteract != null)
            {
                if (_objectToInteract.GetComponent<Artifact>() != null && _grabbedObject == null)
                {
                    TakeObject(_objectToInteract);
                }
                else
                {
                    _objectToInteract.SendMessage("OnInteract", this, SendMessageOptions.DontRequireReceiver);
                }
            }
            else if (_grabbedObject != null)
            {
                DropObject();
            }
        }

        public void TakeObject(GameObject obj)
        {
            Utilities.DisableRigidBody(obj);
            
            obj.transform.SetParent(_grabbingTransform, true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            _grabbedObject = obj;
            obj.SendMessage("OnPickedUp", this, SendMessageOptions.DontRequireReceiver);
        }

        public void DropObject()
        {
            if (_grabbedObject == null)
            {
                Debug.LogWarning("Trying to drop a null object");
                return;
            }

            
            Utilities.EnableRigidBody(_grabbedObject);
            _grabbedObject.transform.SetParent(null, true);
            _grabbedObject.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;

            _grabbedObject.SendMessage("OnDropped", this, SendMessageOptions.DontRequireReceiver);
            _grabbedObject = null;
        }
    }
}
