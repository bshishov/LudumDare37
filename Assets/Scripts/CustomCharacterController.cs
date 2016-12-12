﻿using Assets.Scripts.UI;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class CustomCharacterController : MonoBehaviour
    {
        public float ForwardSpeed = 8.0f;
        public float BackwardSpeed = 4.0f;
        public float StrafeSpeed = 4.0f;
        public float RunMultiplier = 2.0f;   // Speed when sprinting
        public KeyCode RunKey = KeyCode.LeftShift;
        public float JumpForce = 30f;
        public float AirControlModifier = 0.7f;
        public float GroundDrag = 5f;

        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
        public float ShellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        public float GroundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float StickToGroundHelperDistance = 0.5f; // stops the character

        [HideInInspector]
        public float CurrentTargetSpeed = 8f;

        public Vector3 Velocity
        {
            get { return _rigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return _isGrounded; }
        }

        public bool Running
        {
            get { return _running; }
        }

        public bool Jumping
        {
            get { return _jumping; }
        }

        private Rigidbody _rigidBody;
        private CapsuleCollider _capsule;
        private bool _running;
        private bool _previouslyGrounded;
        private bool _isGrounded;
        private bool _jumping;
        private bool _jump;
        private Vector3 _groundContactNormal;

        void Start ()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
        }

        void Update()
        {
            if (CrossPlatformInputManager.GetButtonDown("Jump") && !_jump)
            {
                _jump = true;
            }

            if (CrossPlatformInputManager.GetButtonDown("Fire2"))
            {
                GetComponent<LastStaticPositionTracker>().ResetToStaticPositionDelayed(0.5f);
            }
        }

        void FixedUpdate()
        {
            var input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            GroundCheck();

            UpdateDesiredTargetSpeed(input);


            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                var desiredMove = transform.forward * input.y + transform.right * input.x;
                
                desiredMove = Vector3.ProjectOnPlane(desiredMove, _groundContactNormal).normalized;

                if (!_isGrounded)
                    desiredMove *= AirControlModifier;

                desiredMove.x = desiredMove.x * CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * CurrentTargetSpeed;
                if (_rigidBody.velocity.sqrMagnitude <
                    (CurrentTargetSpeed * CurrentTargetSpeed))
                {
                    _rigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (_isGrounded)
            {
                _rigidBody.drag = GroundDrag;

                if (_jump)
                {
                    _rigidBody.drag = 0f;
                    _rigidBody.velocity = Vector3.ProjectOnPlane(_rigidBody.velocity, transform.up);
                        //new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);
                    _rigidBody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
                    _jumping = true;
                }

                if (!_jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && _rigidBody.velocity.magnitude < 1f)
                {
                    _rigidBody.Sleep();
                }
            }
            else
            {
                _rigidBody.drag = 0f;
                if (_previouslyGrounded && !_jumping)
                {
                    StickToGroundHelper();
                }
            }
            _jump = false;
        }

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (input == Vector2.zero) return;
            if (input.x > 0 || input.x < 0)
            {
                //strafe
                CurrentTargetSpeed = StrafeSpeed;
            }
            if (input.y < 0)
            {
                //backwards
                CurrentTargetSpeed = BackwardSpeed;
            }
            if (input.y > 0)
            {
                //forwards
                //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                CurrentTargetSpeed = ForwardSpeed;
            }
#if !MOBILE_INPUT
            if (Input.GetKey(RunKey))
            {
                CurrentTargetSpeed *= RunMultiplier;
                _running = true;
            }
            else
            {
                _running = false;
            }
#endif
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            _previouslyGrounded = _isGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, _capsule.radius * (1.0f - ShellOffset), -transform.up, out hitInfo,
                                   ((_capsule.height / 2f) - _capsule.radius) + GroundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                _isGrounded = true;
                _groundContactNormal = hitInfo.normal;
            }
            else
            {
                _isGrounded = false;
                _groundContactNormal = transform.up;
            }
            if (!_previouslyGrounded && _isGrounded && _jumping)
            {
                _jumping = false;
            }
        }

        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, _capsule.radius * (1.0f - ShellOffset), -transform.up, out hitInfo,
                                   ((_capsule.height / 2f) - _capsule.radius) +
                                   StickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, transform.up)) < 85f)
                {
                    _rigidBody.velocity = Vector3.ProjectOnPlane(_rigidBody.velocity, hitInfo.normal);
                }
            }
        }

        private float SlopeMultiplier()
        {
            var angle = Vector3.Angle(_groundContactNormal, transform.up);
            return SlopeCurveModifier.Evaluate(angle);
        }

        void OnPositionResetStarted()
        {
            FindObjectOfType<Fader>().FadeInOut(0.5f);
        }
    }
}
