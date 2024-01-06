using System;
using TES3Unity.ESM.Records;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public class UIBook : UIReadable
    {
        private int _numberOfPages;
        private int _cursor;
        private string[] _pages;

        [SerializeField]
        private int _numCharPerPage = 565;
        [SerializeField]
        protected Text _page1 = null;
        [SerializeField]
        protected Text _page2 = null;
        [SerializeField]
        protected Text _numPage1 = null;
        [SerializeField]
        protected Text _numPage2 = null;
        [SerializeField]
        private Button _nextButton = null;
        [SerializeField]
        private Button _previousButton = null;

        public override string BackgroundImageName => "tx_menubook";

        public override void Show(BOOKRecord book)
        {
            m_BookRecord = book;

            var words = m_BookRecord.Text;
            words = words.Replace("<BR><BR>", "\n");
            words = words.Replace("<BR>", "\n");
            words = System.Text.RegularExpressions.Regex.Replace(words, @"<[^>]*>", string.Empty);

            var countChar = 0;
            var j = 0;

            for (var i = 0; i < words.Length; i++)
            {
                if (words[i] != '\n')
                {
                    countChar++;
                }
            }

            // Ceil returns the bad value... 16.6 returns 16..
            _numberOfPages = Mathf.CeilToInt(countChar / _numCharPerPage) + 1;
            _pages = new string[_numberOfPages];

            for (int i = 0; i < countChar; i++)
            {
                if (i % _numCharPerPage == 0 && i > 0)
                {
                    _pages[j] = _pages[j].TrimEnd('\n');
                    j++;
                }

                if (_pages[j] == null)
                {
                    _pages[j] = String.Empty;
                }

                _pages[j] += words[i];
            }

            _cursor = 0;

            UpdateBook();

            StartCoroutine(SetReadableActive(true));
        }

        private void UpdateBook()
        {
            if (_numberOfPages > 1)
            {
                _page1.text = _pages[_cursor];
                _page2.text = _cursor + 1 >= _numberOfPages ? "" : _pages[_cursor + 1];
            }
            else
            {
                _page1.text = _pages[0];
                _page2.text = string.Empty;
            }

            _nextButton.interactable = _cursor + 2 < _numberOfPages;
            _previousButton.interactable = _cursor - 2 >= 0;

            if (_cursor + 2 < _numberOfPages && _pages[_cursor + 2] == string.Empty)
            {
                _nextButton.interactable = false;
            }

            _numPage1.text = (_cursor + 1).ToString();
            _numPage2.text = (_cursor + 2).ToString();
        }

        public void Next()
        {
            if (_cursor + 2 >= _numberOfPages)
            {
                return;
            }

            if (_pages[_cursor + 2] == string.Empty)
            {
                return;
            }

            _cursor += 2;

            UpdateBook();
        }

        public void Previous()
        {
            if (_cursor - 2 < 0)
            {
                return;
            }

            _cursor -= 2;

            UpdateBook();
        }
    }
}
