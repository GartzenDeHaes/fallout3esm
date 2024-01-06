using System;
using UnityEngine;

namespace TES3Unity.UI
{
    public class UIWindow : MonoBehaviour
    {
        [SerializeField]
        protected GameObject m_Container = null;

        public event Action<UIWindow> CloseRequest = null;

        public virtual void SetVisible(bool visible)
        {
            m_Container?.SetActive(visible);
        }

        public virtual void OnValidateClicked()
        {
        }

        public virtual void OnBackClicked()
        {
        }

        public virtual void OnCloseRequest()
        {
        }

        public virtual void OnNextClicked()
        {
        }

        public virtual void OnPreviousClicked()
        {
        }

        protected void NotifyCloseRequest() => CloseRequest?.Invoke(this);
    }
}
