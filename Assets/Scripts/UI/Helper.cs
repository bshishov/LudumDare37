using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class Helper : MonoBehaviour
    {
        private Text _text;
        private Image _icon;
        private bool _isVisible = true;

        void Start ()
        {
            _text = GetComponentInChildren<Text>();
            _icon = GetComponentInChildren<Image>();

            Hide();
        }

        public void Show(string text)
        {
            if (!_isVisible)
            {
                _text.text = text;
                //_text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1f);
                _icon.color = new Color(_icon.color.r, _icon.color.g, _icon.color.b, 1f);
                _isVisible = true;
            }
        }

        public void Hide()
        {
            if (_isVisible)
            {
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 0f);
                _icon.color = new Color(_icon.color.r, _icon.color.g, _icon.color.b, 0f);
                _isVisible = false;
            }
        }
    }
}
