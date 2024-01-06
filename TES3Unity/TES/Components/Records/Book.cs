using TES3Unity.ESM.Records;
using TES3Unity.UI;

namespace TES3Unity.Components.Records
{
    public class Book : RecordComponent
    {
        private static PlayerController _player = null;
        private static UIManager _uiManager = null;

        public static PlayerController Player
        {
            get
            {
                if (_player == null)
                {
                    _player = FindObjectOfType<PlayerController>();
                }

                return _player;
            }
        }

        public static UIManager UIManager
        {
            get
            {
                if (_uiManager == null)
                {
                    _uiManager = FindObjectOfType<UIManager>();
                }

                return _uiManager;
            }
        }

        private void Start()
        {
            usable = true;
            pickable = false;

            var BOOK = (BOOKRecord)record;
            objData.interactionPrefix = "Read ";
            objData.name = BOOK.Name != null ? BOOK.Name : BOOK.EditorId;

            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(BOOK.ITEX.value, "icons");
            objData.weight = BOOK.Data.Weight.ToString();
            objData.value = BOOK.Data.Value.ToString();

            TryAddScript(BOOK.Script);
        }

        public override void Interact()
        {
            var BOOK = (BOOKRecord)record;

            if (BOOK.Text == null)
            {
                if (BOOK.Data.Scroll == 1)
                {
                    OnTakeScroll(BOOK);
                }
                else
                {
                    OnTakeBook(BOOK);
                }
            }

            if (BOOK.Data.Scroll == 1)
            {
                UIManager.Scroll.Show(BOOK);
                UIManager.Scroll.OnClosed += OnCloseScroll;
                UIManager.Scroll.OnTake += OnTakeScroll;
            }
            else
            {
                UIManager.Book.Show(BOOK);
                UIManager.Book.OnClosed += OnCloseBook;
                UIManager.Book.OnTake += OnTakeBook;
            }
        }

        private void OnTakeScroll(BOOKRecord obj)
        {
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.AddItem(this);
        }

        private void OnCloseScroll(BOOKRecord obj)
        {
            UIManager.Scroll.OnClosed -= OnCloseScroll;
            UIManager.Scroll.OnTake -= OnTakeScroll;
        }

        private void OnTakeBook(BOOKRecord obj)
        {
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.AddItem(this);
        }

        private void OnCloseBook(BOOKRecord obj)
        {
            UIManager.Book.OnClosed -= OnCloseBook;
            UIManager.Book.OnTake -= OnTakeBook;
        }
    }
}