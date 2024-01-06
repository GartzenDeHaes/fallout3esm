using System;
using System.Collections;
using TES3Unity.Components.Records;
using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity.UI
{
    public enum UIWindowType
    {
        None = 0, Book, Scroll, Inventory, Menu, Rest, Journal
    }

    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        private InputActionMap m_UIActionMap = null;
        private InputActionMap m_GameplayActionMap = null;
        private UIWindow m_CurrentWindow = null;
        private UIWindowType m_CurrentWindowType = UIWindowType.None;
        private bool m_XREnabled;

        [Header("HUD Elements")]
        [SerializeField]
        private UICrosshair _crosshair = null;
        [SerializeField]
        private UIInteractiveText _interactiveText = null;

        [Header("UI Elements")]
        [SerializeField]
        private UIBook m_Book = null;
        [SerializeField]
        private UIScroll m_Scroll = null;
        [SerializeField]
        private UIInventory m_Inventory = null;
        [SerializeField]
        private UIMenu m_Menu = null;
        [SerializeField]
        private UIRest m_Rest = null;
        [SerializeField]
        private UIJournal m_Journal = null;

        public UIBook Book => m_Book;
        public UIInteractiveText InteractiveText => _interactiveText;
        public UIScroll Scroll => m_Scroll;
        public UICrosshair Crosshair => _crosshair;

        public event Action<UIWindow, bool> WindowOpenChanged = null;

        public IEnumerator Start()
        {
            var playerTag = "Player";
            var player = GameObject.FindWithTag(playerTag);

            m_UIActionMap = InputManager.GetActionMap("UI");
            m_UIActionMap["Validate"].started += (c) => m_CurrentWindow?.OnValidateClicked();
            m_UIActionMap["Back"].started += (c) => m_CurrentWindow?.OnBackClicked();
            m_UIActionMap["Next"].started += (c) => m_CurrentWindow?.OnNextClicked();
            m_UIActionMap["Previous"].started += (c) => m_CurrentWindow?.OnPreviousClicked();

            while (player == null)
            {
                player = GameObject.FindWithTag(playerTag);
                yield return null;
            }

            var windows = GetComponentsInChildren<UIWindow>();
            foreach (var window in windows)
            {
                window.CloseRequest += (c) => CloseWindow();
            }

            var playerCharacter = player.GetComponent<PlayerCharacter>();
            playerCharacter.InteractiveTextChanged += OnInteractiveTextChanged;

            m_GameplayActionMap = InputManager.GetActionMap("Gameplay");
            m_GameplayActionMap["Rest"].started += (c) => OpenWindow(UIWindowType.Rest);
            m_GameplayActionMap["Journal"].started += (c) => OpenWindow(UIWindowType.Journal);
            m_GameplayActionMap["Inventory"].started += (c) => OpenWindow(UIWindowType.Inventory);
            m_GameplayActionMap["Menu"].started += (c) => OpenWindow(UIWindowType.Menu);

#if UNITY_STANDALONE
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
        }

        private void OnInteractiveTextChanged(RecordComponent component, bool visible)
        {
            if (visible)
            {
                var data = component.objData;
                _interactiveText.Show(GUIUtils.CreateSprite(data.icon), data.interactionPrefix, data.name, data.value, data.weight);
            }
            else
            {
                _interactiveText.Close();
            }
        }

        public UIWindow OpenWindow(UIWindowType type)
        {
            // Toggle
            if (type != UIWindowType.None && type == m_CurrentWindowType)
            {
                CloseWindow();
                SetActionMapEnabled(false);
                return null;
            }

            // One window at a time.
            CloseWindow();

            if (type == UIWindowType.Book)
            {
                m_CurrentWindow = m_Book;
            }
            else if (type == UIWindowType.Scroll)
            {
                m_CurrentWindow = m_Scroll;
            }
            else if (type == UIWindowType.Inventory)
            {
                m_CurrentWindow = m_Inventory;
            }
            else if (type == UIWindowType.Menu)
            {
                m_CurrentWindow = m_Menu;
            }
            else if (type == UIWindowType.Rest)
            {
                m_CurrentWindow = m_Rest;
            }
            else if (type == UIWindowType.Journal)
            {
                m_CurrentWindow = m_Journal;
            }

            if (m_CurrentWindow != null)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                m_UIActionMap.Enable();
                m_CurrentWindow.SetVisible(true);
                m_CurrentWindowType = type;
                WindowOpenChanged?.Invoke(m_CurrentWindow, true);
            }

            SetActionMapEnabled(m_CurrentWindow != null);

            return m_CurrentWindow;
        }

        public void CloseWindow()
        {
            if (m_CurrentWindow == null)
            {
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            m_CurrentWindow.OnCloseRequest();
            m_CurrentWindow.SetVisible(false);
            m_CurrentWindowType = UIWindowType.None;
            SetActionMapEnabled(false);
            WindowOpenChanged?.Invoke(m_CurrentWindow, false);
        }

        private void SetActionMapEnabled(bool ui)
        {
            if (ui)
            {
                InputManager.Disable("Movement");
                m_UIActionMap.Enable();
            }
            else
            {
                InputManager.Enable("Movement");
                m_UIActionMap.Disable();
            }

            if (!m_XREnabled)
            {
                _crosshair.Enabled = !ui;
            }
        }
    }
}
