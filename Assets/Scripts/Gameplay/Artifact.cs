using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Artifact : MonoBehaviour
    {
        public Color InitialColor;

        public bool IsCharged
        {
            get { return _isCharged; }
        }

        private Color _chargedColor;
        private bool _isCharged;
        private ParticleSystem _particleSystem;
        private MeshRenderer _meshRenderer;

        void Start ()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();

            UnCharge(InitialColor);
        }
	
        void Update ()
        {
	
        }

        public void Charge(Color color)
        {
            _chargedColor = color;
            SetColor(color);
            _isCharged = true;

            if(_particleSystem.isStopped)
                _particleSystem.Play();
        }

        public void UnCharge(Color color)
        {
            _chargedColor = InitialColor;
            _isCharged = false;
            SetColor(InitialColor);

            if (_particleSystem.isPlaying)
                _particleSystem.Stop();
        }

        private void SetColor(Color color)
        {
            _meshRenderer.material.SetColor("_EmissionColor", color);
            _particleSystem.startColor = color;
        }

        void OnPickedUp()
        {
            Charge(Random.ColorHSV());
        }
    }
}
