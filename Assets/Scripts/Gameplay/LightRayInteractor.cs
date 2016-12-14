using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class LightRayInteractor : MonoBehaviour
    {
        public const float LineWidth = 0.4f;
        public const float MinRayLength = 1f;
        public const float MaxRayLength = 200f;

        public bool HasLight
        {
            get { return Type == LightRayInteractorType.Emitter || _inputs.Count > 0; }
        }

        public Color CurrentColor
        {
            get { return _currentColor; }
        }

        public enum LightRayInteractorType
        {
             Emitter,
             Receiver,
             Director,
        }

        public bool ShouldUseRaycastTest
        {
            get { return Type == LightRayInteractorType.Director || 
                    (Type == LightRayInteractorType.Emitter && ConnectedWith != null); }
        }

        public LightRayInteractorType Type;
        public LightRayInteractor ConnectedWith;
        public Color EmitterColor = Color.cyan;
        public GameObject LightSparks;
        public AudioClipWithVolume OnLight;
        public AudioClipWithVolume LightLoopSound;

        private GameObject _lightSparks;
        private LineRenderer _line;
        private ParticleSystem _particleSystem;
        private Color _currentColor;
        private readonly List<LightRayInteractor> _inputs = new List<LightRayInteractor>();
        private AudioSource _audioSource;

        void Start()
        {
            _line = GetComponent<LineRenderer>();
            if (_line != null)
            {
                _line.SetWidth(LineWidth, LineWidth);
                _line.SetPosition(0, transform.position);
                _line.SetPosition(1, transform.position);
                _line.enabled = false;
            }

            _particleSystem = GetComponentInChildren<ParticleSystem>();
            if (_particleSystem != null && _particleSystem.isPlaying)
            {
                _particleSystem.Stop();
            }

            StartCoroutine(LateStart());

            _audioSource = GetComponent<AudioSource>();
        }

        IEnumerator LateStart()
        {
            yield return new WaitForFixedUpdate();
            if (Type == LightRayInteractorType.Emitter && ConnectedWith != null)
            {
                OnLightEnabled(EmitterColor);
            }
        }
	
        void Update ()
        {
            if (HasLight)
            {
                // In case interactor is moving
                _line.SetPosition(0, transform.position);

                if (ShouldUseRaycastTest)
                {
                    var direction = transform.TransformDirection(Vector3.up);
                    if (Type == LightRayInteractorType.Emitter)
                        direction = (ConnectedWith.transform.position - transform.position).normalized;

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, direction, out hit, MaxRayLength, Tags.LayerMask.Default))
                    {
                        var lightinteractor = hit.collider.GetComponent<LightRayInteractor>();
                        if (hit.distance > MinRayLength &&  lightinteractor != null && lightinteractor.Type == LightRayInteractorType.Receiver)
                        {
                            if (lightinteractor.OnLigtRayIn(this, _currentColor))
                            {
                                HideSparks();
                                ConnectedWith = lightinteractor;
                                _line.SetPosition(1, ConnectedWith.transform.position);
                            }
                            else
                            {
                                // Wrong color
                                OnRayCastFailed(hit);
                            }
                        }
                        else
                        {
                            // Not a receiver
                            OnRayCastFailed(hit);
                        }
                    }
                    else
                    {
                        // Not hit
                        hit.point = transform.position + transform.TransformDirection(Vector3.up)*MaxRayLength;
                        OnRayCastFailed(hit);
                    }
                }
                else
                {
                    // Without raycast (direct connect)
                    if (ConnectedWith != null)
                    {
                        _line.enabled = true;
                        _line.SetPosition(1, ConnectedWith.transform.position);
                    }
                }
            }
        }

        public bool OnLigtRayIn(LightRayInteractor interactor, Color color)
        {
            // If current color is already set (lightning is on)
            if (HasLight && !Utilities.IsIdenticalColor(color, _currentColor))
                return false;

            var before = HasLight;
            if (interactor != null)
            {
                // return true beacuse ligth input succeeded
                if (_inputs.Contains(interactor))
                    return true;

                _inputs.Add(interactor);
            }

            // if HasLight changed then call OnLightEnabled
            if (!before && HasLight)
                OnLightEnabled(color);
            return true;
        }

        public void OnLightRayOut(LightRayInteractor interactor)
        {
            var before = HasLight;

            
            if (interactor != null)
            {
                if (_inputs.Contains(interactor))
                    _inputs.Remove(interactor);
            }

            // if HasLight changed then call OnLightDisabled
            if (before && !HasLight)
                OnLightDisabled();
        }

        public void OnLightEnabled(Color color)
        {
            Debug.LogFormat("Light enabled [{0}] {1}", gameObject.name, color);
            _currentColor = color;

            if (_audioSource != null)
            {
                if (LightLoopSound.Clip != null)
                {
                    _audioSource.clip = LightLoopSound.Clip;
                    _audioSource.loop = true;
                    _audioSource.Play();
                }

                if (Type == LightRayInteractorType.Receiver && OnLight.Clip != null)
                {
                    _audioSource.PlayOneShot(OnLight.Clip, OnLight.VolumeModifier);
                }
            }

            if (_line != null)
            {
                _line.enabled = true;
                _line.SetColors(_currentColor, _currentColor);
            }

            if (_particleSystem != null)
            {
                _particleSystem.startColor = _currentColor;

                //if(_particleSystem.isStopped)
                _particleSystem.Play();
            }

            if (!ShouldUseRaycastTest && ConnectedWith != null)
                ConnectedWith.OnLigtRayIn(this, _currentColor);
        }

        public void OnLightDisabled()
        {
            Debug.LogFormat("Light disabled [{0}]", gameObject.name);

            if (ConnectedWith != null)
                ConnectedWith.OnLightRayOut(this);

            if (_line != null)
            {
                _line.enabled = false;
            }

            if (_particleSystem != null && _particleSystem.isPlaying)
                _particleSystem.Stop();

            HideSparks();

            if (_audioSource != null)
            {
                if (LightLoopSound.Clip != null)
                {
                    _audioSource.clip = null;
                    _audioSource.loop = false;
                }
            }
        }

        private void ShowSparksAt(Vector3 pos)
        {
            if (_lightSparks != null)
            {
                _lightSparks.transform.position = pos;
                var particles = _lightSparks.GetComponent<ParticleSystem>();
                if(particles.isStopped)
                    particles.Play();
            }
            else
            {
                if (LightSparks != null)
                    _lightSparks = (GameObject) Instantiate(LightSparks, pos, Quaternion.identity);
            }
        }

        private void HideSparks()
        {
            if (_lightSparks != null)
            {
                var particles = _lightSparks.GetComponent<ParticleSystem>();
                if(particles.isPlaying)
                    particles.Stop();
            }
        }

        private void OnRayCastFailed(RaycastHit hit)
        {
            // disconnect
            if (ConnectedWith != null)
                ConnectedWith.OnLightRayOut(this);

            // emitter can't lose it connection, but director does
            if (Type != LightRayInteractorType.Emitter)
                ConnectedWith = null;

            ShowSparksAt(hit.point);
            _line.SetPosition(1, hit.point);
        }
    }
}
