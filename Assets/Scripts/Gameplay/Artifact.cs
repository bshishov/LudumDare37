using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class Artifact : MonoBehaviour
    {
        public Color InitialColor;
        public bool InitiallyCharged;

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

        void Start ()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();

            if (InitiallyCharged)
            {
                Charge(InitialColor);
            }
            else
            {
                UnCharge();
            }
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

        public void UnCharge()
        {
            _chargedColor = Color.black;
            _isCharged = false;
            SetColor(Color.black);

            if (_particleSystem.isPlaying)
                _particleSystem.Stop();
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
        }

        void OnInteract(Interactor interactor)
        {
        }
    }
}
