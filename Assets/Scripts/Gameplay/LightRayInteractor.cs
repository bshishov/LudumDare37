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
        public const float MaxRayLength = 500f;

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
                EnableLight(null, EmitterColor);
            }
        }
	
        void Update ()
        {
            if (HasLight)
            {
                _line.SetPosition(0, transform.position);
                if (Type == LightRayInteractorType.Director)
                {
                    _line.enabled = true;
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, MaxRayLength, Tags.LayerMask.Default))
                    {
                        var lightinteractor = hit.collider.GetComponent<LightRayInteractor>();
                        if (hit.distance > 2f &&  lightinteractor != null && lightinteractor.Type == LightRayInteractorType.Receiver)
                        {
                            HideSparks();
                            ConnectedWith = lightinteractor;
                            ConnectedWith.EnableLight(this, _currentColor);
                            _line.SetPosition(1, ConnectedWith.transform.position);
                        }
                        else
                        {
                            ShowSparksAt(hit.point);
                            if (ConnectedWith != null)
                                ConnectedWith.DisableLight(this);

                            _line.SetPosition(1, hit.point);
                        }
                    }
                    else
                    {
                        if (ConnectedWith != null)
                            ConnectedWith.DisableLight(this);

                        var pos = transform.position + transform.TransformDirection(Vector3.up)*MaxRayLength;
                        _line.SetPosition(1, pos);
                        ShowSparksAt(pos);
                    }
                }
                else
                {
                    if (ConnectedWith != null)
                    {
                        _line.enabled = true;
                        _line.SetPosition(1, ConnectedWith.transform.position);
                    }
                }
            }
        }

        public void EnableLight(LightRayInteractor interactor, Color color)
        {
            var before = HasLight;
            if (interactor != null)
            {
                if (_inputs.Contains(interactor))
                    return;

                _inputs.Add(interactor);
            }

            if (Type == LightRayInteractorType.Emitter || (!before && HasLight))
            {
                if (_audioSource != null)
                {
                    if (LightLoopSound.Clip != null)
                    {
                        _audioSource.clip = LightLoopSound.Clip;
                        _audioSource.loop = true;
                        _audioSource.Play();
                    }
                }

                if (Type == LightRayInteractorType.Receiver && _audioSource != null && OnLight.Clip != null)
                {
                    _audioSource.PlayOneShot(OnLight.Clip, OnLight.VolumeModifier);
                }

                Debug.Log("Enabled light " + gameObject.name);
                _currentColor = color;

                if (_line != null)
                {
                    _line.enabled = true;
                    _line.SetColors(_currentColor, _currentColor);
                }

                if (_particleSystem != null && _particleSystem.isStopped)
                    _particleSystem.Play();
                
                if (_particleSystem != null)
                    _particleSystem.startColor = _currentColor;

                if (ConnectedWith != null)
                    ConnectedWith.EnableLight(this, _currentColor);
            }
        }

        public void DisableLight(LightRayInteractor interactor)
        {
            var before = HasLight;

            if (interactor != null)
            {
                if (_inputs.Contains(interactor))
                    _inputs.Remove(interactor);

                // Hack?
                _inputs.Clear();
            }

            if (Type == LightRayInteractorType.Emitter || (before && !HasLight))
            {
                Debug.Log("Disabled light " + gameObject.name);

                if (_line != null)
                {
                    _line.enabled = false;
                }

                if (_particleSystem != null && _particleSystem.isPlaying)
                    _particleSystem.Stop();

                if (ConnectedWith != null)
                    ConnectedWith.DisableLight(this);

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
    }
}
