using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    [RequireComponent(typeof(Rigidbody))]
    public class LastStaticPositionTracker : MonoBehaviour
    {
        public Vector3 LastStaticPos
        {
            get { return _lastStaticPos; }
        }

        public float TimeToStandStill = 0.5f;
        public float VelocityTreshold = 0.1f;
        public bool ResetOnWorldBoundsHit = false;
        public float WaitAfterWorldBoundsHit = 0f;

        private Rigidbody _rigidbody;
        private bool _resetQueried;
        private float _timeStandingStill;
        private Vector3 _lastStaticPos;
        private Quaternion _lastStaticQuaternion;

        void Start ()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _lastStaticPos = transform.position;
        }
	
        void FixedUpdate ()
        {
            if (!_rigidbody.isKinematic && _rigidbody.velocity.magnitude < VelocityTreshold)
            {
                _timeStandingStill += Time.fixedDeltaTime;
                if (_timeStandingStill > TimeToStandStill)
                {
                    _lastStaticPos = transform.position;
                    _lastStaticQuaternion = transform.localRotation;
                }
            }
            else
            {
                _timeStandingStill = 0f;
            }
        }

        public void ResetToStaticPosition()
        {
            DoResetRigidBody();
        }

        public void ResetToStaticPositionDelayed(float delay)
        {
            if (!_resetQueried)
                StartCoroutine(ResetCoroutine(delay));
        }

        private IEnumerator ResetCoroutine(float delay)
        {
            SendMessage("OnPositionResetStarted", SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(delay);
            DoResetRigidBody();
            _resetQueried = false;
        }

        private void DoResetRigidBody()
        {
            _rigidbody.velocity = Vector3.zero;
            transform.position = LastStaticPos;
            transform.localRotation = _lastStaticQuaternion;
            SendMessage("OnPositionReset", SendMessageOptions.DontRequireReceiver);
        }

        void OnTriggerEnter(Collider col)
        {
            if (ResetOnWorldBoundsHit)
            {
                if (col.CompareTag(Tags.WorldBounds))
                    ResetToStaticPositionDelayed(WaitAfterWorldBoundsHit);
            }
        }
    }
}
