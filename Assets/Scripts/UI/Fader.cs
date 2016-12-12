using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour
    {
        public enum FaderState
        {
            FadedIn,
            FadingIn,
            FadedOut,
            FadingOut
        }

        public float FadeTime = 1f;
        public FaderState InitialState = FaderState.FadedOut;
        

        public FaderState State
        {
            get { return _state; }
        }

        private Image _image;
        private Color _fadedIn;
        private Color _fadedOut;
        private FaderState _state;
        private float _transition = 0f;
        
        void Awake()
        {
            _image = GetComponent<Image>();
            _fadedIn = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
            _fadedOut = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
            
            _state = InitialState;
            if (_state == FaderState.FadedIn)
                _image.color = _fadedIn;

            if (_state == FaderState.FadedOut)
                _image.color = _fadedIn;
            FadeOut();
        }

        void Start ()
        {
        }
	
        void Update ()
        {
            if (_state == FaderState.FadingIn)
            {
                _transition += Time.deltaTime;
                _image.color = Color.Lerp(_fadedOut, _fadedIn, _transition/FadeTime);

                if (_transition > FadeTime)
                    _state = FaderState.FadedIn;
            }

            if (_state == FaderState.FadingOut)
            {
                _transition += Time.deltaTime;
                _image.color = Color.Lerp(_fadedIn, _fadedOut, _transition / FadeTime);

                if (_transition > FadeTime)
                    _state = FaderState.FadedOut;
            }
        }

        public void FadeIn()
        {
            if (_state == FaderState.FadedOut)
            {
                _transition = 0;
                _state = FaderState.FadingIn;
            }
        }

        public void FadeOut()
        {
            if (_state == FaderState.FadedIn)
            {
                _transition = 0;
                _state = FaderState.FadingOut;
            }
        }

        public void FadeInOut(float delayBetween = 0.2f)
        {
            StartCoroutine(FadeInOutCoroutine(delayBetween));
        }

        private IEnumerator FadeInOutCoroutine(float delaybetween)
        {
            FadeIn();
            yield return new WaitForSeconds(FadeTime + delaybetween);
            FadeOut();
        } 
    }
}
