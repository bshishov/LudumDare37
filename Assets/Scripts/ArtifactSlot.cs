using System;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts
{
    public class ArtifactSlot : MonoBehaviour
    {
        public bool HasObject { get { return _artifactInSlot != null; } }
        public Artifact ArtifactInSlot { get { return _artifactInSlot; } }

        public event Action<Artifact> OnArtifactPlaced;
        public event Action<Artifact> OnArtifactRemoved;

        private Artifact _artifactInSlot;
        
        void Start ()
        {
        }

        void OnInteract(Interactor interactor)
        {
            if (interactor.GrabbedObject != null)
            {
                if (interactor.GrabbedObject.GetComponent<Artifact>() != null)
                {
                    var obj = interactor.GrabbedObject;
                    interactor.DropObject();
                    Utilities.DisableRigidBody(obj, keepCollider:true);

                    obj.transform.SetParent(transform, true);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                    _artifactInSlot = obj.GetComponent<Artifact>();
                    _artifactInSlot.SendMessage("OnPlacedInSlot", this, SendMessageOptions.DontRequireReceiver);

                    if(OnArtifactPlaced != null)
                        OnArtifactPlaced.Invoke(_artifactInSlot);
                }
                else
                {
                    Debug.LogWarning("Only artifacts are accepted");
                }
            }
            else
            {
                if (_artifactInSlot != null)
                {
                    _artifactInSlot.transform.SetParent(null, true);
                    _artifactInSlot.SendMessage("OnRemovedFromSlot", this, SendMessageOptions.DontRequireReceiver);
                    interactor.TakeObject(_artifactInSlot.gameObject);

                    if (OnArtifactRemoved != null)
                        OnArtifactRemoved.Invoke(_artifactInSlot);

                    _artifactInSlot = null;
                }
            }
        }
    }
}
