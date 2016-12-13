using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class Artifact : MonoBehaviour
    {
        public Color InitialColor;
        public bool InitiallyCharged;
        public AudioClipWithVolume PickedUpSound;
        public AudioClipWithVolume ChargedClip;
        public AudioClipWithVolume HitClip;

        public bool IsCharged
        {
            get { return _isCharged; }
        }

        public Color ChargedColor
        {
            get { return _chargedColor; }
        }

        private Color _chargedColor;
        private bool _isCharged;
        private ParticleSystem _particleSystem;
        private MeshRenderer[] _meshRenderers;
        private AudioSource _audioSource;

        void Start ()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _audioSource = GetComponent<AudioSource>();
            StartCoroutine(LateStart());
        }

        IEnumerator LateStart()
        {
            yield return new WaitForFixedUpdate();
            if (InitiallyCharged)
            {
                Charge(InitialColor);
            }
            else
            {
                UnCharge();
            }
        } 

        public void Charge(Color color)
        {
            _chargedColor = color;
            SetColor(color);
            _isCharged = true;

            if(_particleSystem.isStopped)
                _particleSystem.Play();

            if (ChargedClip.Clip != null)
            {
                _audioSource.clip = ChargedClip.Clip;
                _audioSource.loop = true;
                _audioSource.Play();
                _audioSource.volume = ChargedClip.VolumeModifier;
            }
        }

        public void UnCharge()
        {
            _chargedColor = Color.black;
            _isCharged = false;
            SetColor(Color.black);

            if (_particleSystem.isPlaying)
                _particleSystem.Stop();

            _audioSource.Stop();
        }

        private void SetColor(Color color)
        {
            foreach (var meshRenderer in _meshRenderers)
            {
                meshRenderer.material.SetColor("_EmissionColor", color);
            }
          
            _particleSystem.startColor = color;
        }

        void OnPickedUp()
        {
            //Charge(Random.ColorHSV());
            if (PickedUpSound.Clip != null)
            {
                _audioSource.PlayOneShot(PickedUpSound.Clip, PickedUpSound.VolumeModifier);
            }

        }

        void OnInteract(Interactor interactor)
        {
        }

        void OnCollisionEnter(Collision col)
        {
            if(HitClip.Clip != null)
                _audioSource.PlayOneShot(HitClip.Clip, Mathf.Clamp01(HitClip.VolumeModifier * col.impulse.magnitude / 10f));
        }
    }
}
