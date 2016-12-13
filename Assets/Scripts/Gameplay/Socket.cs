using System;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class Socket : MonoBehaviour
    {
        public enum SocketBehaviour
        {
            None,
            Charge,
            Consume
        }

        public SocketBehaviour Mode = SocketBehaviour.None;
        public AudioClipWithVolume ArtifactPlacedSound;
        public AudioClipWithVolume ConsumerChargeSound;
        public AudioClipWithVolume ConsumerCharged;
        public AudioClipWithVolume ChargerChargeSound;
        public AudioClipWithVolume ArtifactRemovedSound;

        public bool IsCharged
        {
            get { return _isCharged; }
        }

        public ArtifactSlot Slot
        {
            get { return _slot; }
        }

        public Color ConsumedColor
        {
            get { return _consumedColor; }
        }

        private ArtifactSlot _slot;
        private MeshRenderer _meshRenderer;
        private LightRayInteractor _lightRayInteractor;
        private ParticleSystem _consumeParticleSystem;
        private Color _consumedColor;
        private bool _isCharged;
        private AudioSource _audioSource;

        void Start ()
        {
            _slot = GetComponent<ArtifactSlot>();
            if (_slot == null)
                _slot = GetComponentInChildren<ArtifactSlot>();
            if (_slot == null)
                Debug.LogWarning("Can't find slot");

            _slot.OnArtifactPlaced += SlotOnOnArtifactPlaced;
            _slot.OnArtifactRemoved += SlotOnOnArtifactRemoved;
            _meshRenderer = GetComponent<MeshRenderer>();

            _lightRayInteractor = GetComponentInChildren<LightRayInteractor>();
            if (Mode == SocketBehaviour.Charge && _lightRayInteractor == null)
                Debug.LogWarning("Missing LightRayInteractor in child objects");

            if (Mode == SocketBehaviour.Consume)
            {
                _consumeParticleSystem = GetComponentInChildren<ParticleSystem>();
                if (!_slot.HasObject && _consumeParticleSystem.isPlaying)
                    _consumeParticleSystem.Stop();
            }

            _audioSource = GetComponent<AudioSource>();
        }

        void FixedUpdate()
        {
            if (Mode == SocketBehaviour.Charge)
            {
                if (_slot.HasObject && _lightRayInteractor.HasLight && !_slot.ArtifactInSlot.IsCharged)
                {
                    if (ChargerChargeSound.Clip != null)
                        _audioSource.PlayOneShot(ChargerChargeSound.Clip, ChargerChargeSound.VolumeModifier);
                    
                    _slot.ArtifactInSlot.Charge(_lightRayInteractor.CurrentColor);
                }
            }
        }

        private void SlotOnOnArtifactRemoved(Artifact artifact)
        {
            if (ArtifactRemovedSound.Clip != null)
                _audioSource.PlayOneShot(ArtifactRemovedSound.Clip, ArtifactRemovedSound.VolumeModifier);

            if (_meshRenderer != null)
                _meshRenderer.material.SetColor("_EmissionColor", Color.black);
            if (Mode == SocketBehaviour.Consume)
            {
            }
        }

        private void SlotOnOnArtifactPlaced(Artifact artifact)
        {
            if (ArtifactPlacedSound.Clip != null)
                _audioSource.PlayOneShot(ArtifactPlacedSound.Clip, ArtifactPlacedSound.VolumeModifier);
            Debug.Log("PLACED");

            if (_meshRenderer != null)
                _meshRenderer.material.SetColor("_EmissionColor", artifact.ChargedColor);
            if (Mode == SocketBehaviour.Consume && artifact.IsCharged && !_isCharged)
            {
                _consumedColor = artifact.ChargedColor;

                if (ConsumerCharged.Clip != null)
                {
                    _audioSource.clip = ConsumerCharged.Clip;
                    _audioSource.loop = true;
                    _audioSource.Play();
                }

                if (ConsumerChargeSound.Clip != null)
                    _audioSource.PlayOneShot(ConsumerChargeSound.Clip, ConsumerChargeSound.VolumeModifier);

                _isCharged = true;
                if (_consumeParticleSystem.isStopped)
                    _consumeParticleSystem.Play();
                Debug.Log("ARTIFACT PLACED");

                artifact.UnCharge();
                SendMessage("OnArtifactConsumed", artifact, SendMessageOptions.DontRequireReceiver);
            }
        }

        public void LockSlot()
        {
            _slot.Locked = true;
        }

        public void UnLockSlot()
        {
            _slot.Locked = false;
        }

        public void UnCharge()
        {
            if (_isCharged)
            {
                _isCharged = false;
                if (_consumeParticleSystem.isPlaying)
                    _consumeParticleSystem.Stop();

                if (ConsumerCharged.Clip != null)
                {
                    _audioSource.clip = null;
                }

                _consumedColor = Color.black;
            }
        }
    }
}
