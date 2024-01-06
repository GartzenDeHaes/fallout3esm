using System;
using System.Collections;
using TES3Unity.ESM.Records;
using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public abstract class UIReadable : UIWindow
    {
        protected BOOKRecord m_BookRecord;

        [SerializeField]
        protected Image _background = null;

        public abstract string BackgroundImageName { get; }

        public event Action<BOOKRecord> OnTake = null;
        public event Action<BOOKRecord> OnClosed = null;

        protected virtual void Start()
        {
            var textureManager = TES3Engine.Inst?.TextureManager;

            if (textureManager != null)
            {
                var texture = textureManager.LoadTexture(BackgroundImageName, true);
                _background.sprite = GUIUtils.CreateSprite(texture);
            }

            // If the book is already opened, don't change its transform.
            if (m_BookRecord == null)
            {
                Close();
            }

            var gameplayActionMap = InputManager.GetActionMap("Gameplay");
            gameplayActionMap["Use"].started += (c) =>
            {
                if (m_Container.activeSelf)
                {
                    Take();
                }
            };

            gameplayActionMap["Cancel"].started += (c) =>
            {
                if (m_Container.activeSelf)
                {
                    Close();
                }
            };
        }

        public abstract void Show(BOOKRecord book);

        public void Take()
        {
            OnTake?.Invoke(m_BookRecord);
            Close();
        }

        public void Close()
        {
            OnClosed?.Invoke(m_BookRecord);

            m_Container.SetActive(false);
            m_BookRecord = null;
        }

        protected IEnumerator SetReadableActive(bool active)
        {
            yield return new WaitForEndOfFrame();

            m_Container.SetActive(active);
        }

        protected void NotifyTake() => OnTake?.Invoke(m_BookRecord);
        protected void NotifyClose() => OnClosed?.Invoke(m_BookRecord);
    }
}
