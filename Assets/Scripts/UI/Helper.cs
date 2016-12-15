using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Helper : MonoBehaviour
    {
        public enum HelperState
        {
            Visible,
            Hidden,
            ToVisible,
            ToHidden
        }

        public HelperState InitialState = HelperState.Hidden;
        public float FadeTime = 1f;

        private Text _text;
        private Image _icon;
        private CanvasGroup _canvasGroup;
        private float _transition = 0f;
        private HelperState _state;

        void Start ()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _text = GetComponentInChildren<Text>();
            _icon = GetComponentInChildren<Image>();

            Hide();
        }

        void Update()
        {
            if (_state == HelperState.ToVisible)
            {
                _transition += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, _transition / FadeTime);

                if (_transition > FadeTime)
                {
                    _state = HelperState.Visible;
                }
            }

            if (_state == HelperState.ToHidden)
            {
                _transition += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, _transition / FadeTime);

                if (_transition > FadeTime)
                    _state = HelperState.Hidden;
            }
        }


        public void Show(string text)
        {
            _text.text = text;

            if (_state == HelperState.Hidden || _state == HelperState.ToHidden)
            {
                _state = HelperState.ToVisible;
                _transition = 0;
            }
        }

        public void Hide()
        {
            if (_state == HelperState.Visible || _state == HelperState.ToVisible)
            {
                _state = HelperState.ToHidden;
                _transition = 0;
            }
        }
    }
}
