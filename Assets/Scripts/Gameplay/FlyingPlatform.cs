using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class FlyingPlatform : MonoBehaviour
    {
        public enum FlyingPlatformState
        {
            AtInitialPosition,   
            AtTargetPosition,
            MovingToTarget,
            MovingToInitial,
            WaitingAtTarget
        }

        public Vector3 TargetPosition;
        public float TravelTime = 10f;
        public float WaitAtTarget = 2f;
        public bool RequireChargeForMovingBack;

        private Vector3 _initialPosition;
        private FlyingPlatformState _state;
        private float _transition;
        private Socket _socket;
        private AudioSource _audioSource;

        void Start ()
        {
            _initialPosition = transform.position;
            _state = FlyingPlatformState.AtInitialPosition;
            _socket = GetComponent<Socket>();
            _audioSource = GetComponent<AudioSource>();
        }
	
        
        void Update ()
        {
            if (_state == FlyingPlatformState.WaitingAtTarget)
            {
                _transition += Time.deltaTime;
                if (_transition > TravelTime)
                {
                    if (!RequireChargeForMovingBack)
                    {
                        _state = FlyingPlatformState.MovingToInitial;
                        _transition = 0;
                    }
                }
            }

            if (_state == FlyingPlatformState.MovingToTarget)
            {
                _transition += Time.deltaTime;
                transform.position = Vector3.Lerp(_initialPosition, TargetPosition, _transition/TravelTime);
                if (_transition > TravelTime)
                {
                    _state = FlyingPlatformState.WaitingAtTarget;
                    _transition = 0;

                    if (RequireChargeForMovingBack)
                        _socket.UnCharge();
                }
            }

            if (_state == FlyingPlatformState.MovingToInitial)
            {
                _transition += Time.deltaTime;
                transform.position = Vector3.Lerp(TargetPosition, _initialPosition, _transition / TravelTime);
                if (_transition > TravelTime)
                {
                    _state = FlyingPlatformState.AtInitialPosition;
                    _transition = 0;

                    _socket.UnCharge();
                }
            }
        }

        void OnArtifactConsumed(Artifact artifact)
        {
            if (_state == FlyingPlatformState.AtInitialPosition)
            {
                _state = FlyingPlatformState.MovingToTarget;
                _transition = 0;
            }

            if ((_state == FlyingPlatformState.AtTargetPosition || _state == FlyingPlatformState.WaitingAtTarget) && RequireChargeForMovingBack)
            {
                _state = FlyingPlatformState.MovingToInitial;
                _transition = 0;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, TargetPosition);
            Gizmos.DrawWireCube(TargetPosition, Vector3.one);
        }


        void OnTriggerEnter(Collider col)
        {
            if (col.transform.parent == null && col.attachedRigidbody != null)
            {
                col.transform.SetParent(transform);
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.transform.parent == transform && col.attachedRigidbody != null)
            {
                col.transform.SetParent(null);
            }
        }
    }
}
