using TES3Unity.ESM.Records;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public class UIScroll : UIReadable
    {
        [SerializeField]
        private Text _content = null;

        public override string BackgroundImageName => "scroll";

        public override void Show(BOOKRecord book)
        {
            m_BookRecord = book;

            var words = m_BookRecord.Text;
            words = words.Replace("\r\n", "");
            words = words.Replace("<BR><BR>", "");
            words = words.Replace("<BR>", "\n");
            words = System.Text.RegularExpressions.Regex.Replace(words, @"<[^>]*>", string.Empty);

            _content.text = words;

            StartCoroutine(SetReadableActive(true));
        }
    }
}
