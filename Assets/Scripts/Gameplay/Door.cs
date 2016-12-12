using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Door : MonoBehaviour
    {
        public enum DoorState
        {
            Closed,
            Closing,
            Opened,
            Opening
        }

        public bool Locked = false;
        public float TargetRotation;
        public float Angle1 = -85f;
        public float Angle2 = 85f;
        public float OpenTime = 1f;
        public float CloseTime = 1f;


        public AnimationCurve OpenRotationCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);
        public AnimationCurve CloseRotationCurve = AnimationCurve.EaseInOut(0, 0f, 1f, 1f);

        private DoorState _state;
        private float _transition = 0;
        private float _currentAngle;
        private float _targetAngle;

        void Start()
        {
            _state = DoorState.Closed;
            _currentAngle = 0f;
        }

        void Update()
        {
            if (_state == DoorState.Closing || _state == DoorState.Opening)
                _transition += Time.deltaTime;

            if (_state == DoorState.Opening)
            {
                var a = Mathf.Lerp(_currentAngle, _targetAngle, OpenRotationCurve.Evaluate(_transition / OpenTime));
                transform.localRotation = Quaternion.Euler(0f, a, 0f);
                if (_transition > OpenTime)
                {
                    _state = DoorState.Opened;
                    _transition = 0f;
                    _currentAngle = _targetAngle;
                }
            }

            if (_state == DoorState.Closing)
            {
                var a = Mathf.Lerp(_currentAngle, _targetAngle, CloseRotationCurve.Evaluate(_transition / CloseTime));
                transform.localRotation = Quaternion.Euler(0f, a, 0f);
                if (_transition > CloseTime)
                {
                    _state = DoorState.Closed;
                    _transition = 0f;
                    _currentAngle = _targetAngle;
                }
            }
        }

        void OnInteract(Interactor interactor)
        {
            if (_state == DoorState.Opened)
            {
                _state = DoorState.Closing;
                _transition = 0;
                _targetAngle = 0;
            }
            else if(_state == DoorState.Closed && !Locked)
            {
                _state = DoorState.Opening;
                _transition = 0;

                var localDelta = transform.InverseTransformPoint(interactor.transform.position) - transform.InverseTransformPoint(transform.position);
                if (localDelta.x < 0)
                {
                    _targetAngle = Angle1;
                }
                else
                {
                    _targetAngle = Angle2;
                }
            }
        }

        float LastKeyTime(AnimationCurve curve)
        {
            return curve.keys[curve.length - 1].time;
        }
    }
}
	
