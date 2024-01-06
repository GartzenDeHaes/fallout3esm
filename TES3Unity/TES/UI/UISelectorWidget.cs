using System;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.Toolbox.UI
{
    public sealed class UISelectorWidget : MonoBehaviour
    {
        private int _index = 0;
        private int _size = 0;

        [SerializeField]
        private Text text = null;
        [SerializeField]
        private string[] options = null;
        [SerializeField]
        private bool interactable = true;

        public string[] Options
        {
            get { return options; }
            set
            {
                options = value;
                _size = options.Length;
                Index = 0;
            }
        }

        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                if (_index >= _size)
                {
                    _index = 0;
                }
                else if (_index < 0)
                {
                    _index = _size - 1;
                }

                UpdateText();
            }
        }

        public string Value
        {
            get { return options[_index]; }
            set { SetValueActive(value); }
        }

        public event Action<string> ValueChanged = null;
        public event Action<int> IndexChanged = null;

        private void Awake()
        {
            if (_size == 0 && options != null)
            {
                _size = options.Length;
            }
        }

        private void Start()
        {
            UpdateText(false);
        }

        public void Setup<T>(int selected, Action<string> valueChanged)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new UnityException("This is not an Enum");
            }

            var values = Enum.GetNames(type);
            Options = values;
            Index = selected;
            ValueChanged += valueChanged;
        }

        public void Setup<T>(int selected, Action<int> indexChanged)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new UnityException("This is not an Enum");
            }

            var values = Enum.GetNames(type);
            Options = values;
            Index = selected;
            IndexChanged += indexChanged;
        }

        public void Setup(ref string[] values, string selected, Action<string> valueCHanged)
        {
            Options = values;
            Index = Array.IndexOf(values, selected);
            ValueChanged += valueCHanged;
        }

        public void SetInteractable(bool isInteractable)
        {
            var buttons = GetComponentsInChildren<Button>();
            for (int i = 0, l = buttons.Length; i < l; i++)
            {
                buttons[i].interactable = isInteractable;
            }

            interactable = isInteractable;
            enabled = interactable;
        }

        public void ChangeValue(bool inc)
        {
            if (interactable)
            {
                Index += inc ? 1 : -1;
            }
        }

        public void SetValueActive(string value)
        {
            var index = Array.IndexOf(options, value);
            if (index > -1)
            {
                Index = index;
            }
        }

        public void UpdateText(bool notify = true)
        {
            if (text != null)
            {
                text.text = options[_index];
            }

            if (!notify)
            {
                return;
            }

            if (ValueChanged != null && notify)
            {
                ValueChanged(options[_index]);
            }

            if (IndexChanged != null)
            {
                IndexChanged.Invoke(_index);
            }
        }
    }
}