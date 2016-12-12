using System;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Socket : MonoBehaviour
    {
        public enum SocketBehaviour
        {
            None,
            Charge,
            Consume
        }

        public SocketBehaviour Mode = SocketBehaviour.None;

        private ArtifactSlot _slot;
        private MeshRenderer _meshRenderer;
        private LightRayInteractor _lightRayInteractor;
        private ParticleSystem _consumeParticleSystem;

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
        }

        void FixedUpdate()
        {
            if (Mode == SocketBehaviour.Charge)
            {
                if (_slot.HasObject && _lightRayInteractor.HasLight)
                {
                    _slot.ArtifactInSlot.Charge(_lightRayInteractor.CurrentColor);
                }
            }
        }

        private void SlotOnOnArtifactRemoved(Artifact artifact)
        {
            _meshRenderer.material.SetColor("_EmissionColor", Color.black);
            if (Mode == SocketBehaviour.Consume)
            {
                /*
                _consumeParticleSystem.startColor = Color.black;
                if (_consumeParticleSystem.isPlaying)
                    _consumeParticleSystem.Stop();*/
            }
        }

        private void SlotOnOnArtifactPlaced(Artifact artifact)
        {
            _meshRenderer.material.SetColor("_EmissionColor", artifact.ChargedColor);
            if (Mode == SocketBehaviour.Consume)
            {
                if (_consumeParticleSystem.isStopped)
                    _consumeParticleSystem.Play();

                artifact.UnCharge();
            }
        }
    }
}
