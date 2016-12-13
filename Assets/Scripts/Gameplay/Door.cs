using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public enum DoorState
        {
            Closed,
            Closing,
            Opened,
            Opening
        }

        public DoorState State
        {
            get { return _state; }
        }

        public bool Locked = false;
        public float TargetRotation;
        public float Angle1 = -85f;
        public float Angle2 = 85f;
        public float OpenTime = 1f;
        public float CloseTime = 1f;


        public AnimationCurve OpenRotationCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);
        public AnimationCurve CloseRotationCurve = AnimationCurve.EaseInOut(0, 0f, 1f, 1f);
        
        public AudioClipWithVolume DoorMovement;
        public AudioClipWithVolume DoorHandle;
        public AudioClipWithVolume DoorSlam;
        public AudioClipWithVolume DoorIsLocked;

        private DoorState _state;
        private float _transition = 0;
        private float _currentAngle;
        private float _targetAngle;
        private AudioSource _audioSource;

        void Start()
        {
            _state = DoorState.Closed;
            _currentAngle = 0f;
            _audioSource = GetComponent<AudioSource>();
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

                    if (DoorHandle.Clip != null)
                        _audioSource.PlayOneShot(DoorHandle.Clip, DoorHandle.VolumeModifier);
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

                    if(DoorSlam.Clip != null)
                        _audioSource.PlayOneShot(DoorSlam.Clip, DoorSlam.VolumeModifier);
                }
            }
        }

        void OnInteract(Interactor interactor)
        {
            if (_state == DoorState.Opened)
            {
                Close();
            }
            else if(_state == DoorState.Closed && !Locked)
            {
                if (Locked)
                {
                    if(DoorIsLocked.Clip != null)
                        _audioSource.PlayOneShot(DoorIsLocked.Clip, DoorIsLocked.VolumeModifier);
                }
                else
                {
                    _state = DoorState.Opening;
                    _transition = 0;

                    if (DoorMovement.Clip != null)
                        _audioSource.PlayOneShot(DoorMovement.Clip, DoorMovement.VolumeModifier);

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
        }

        public void Close()
        {
            if (_state == DoorState.Opened)
            {
                _state = DoorState.Closing;
                _transition = 0;
                _targetAngle = 0;

                if (DoorMovement.Clip != null)
                    _audioSource.PlayOneShot(DoorMovement.Clip, DoorMovement.VolumeModifier);
            }
        }

        float LastKeyTime(AnimationCurve curve)
        {
            return curve.keys[curve.length - 1].time;
        }
    }
}
	
