using TES3Unity.Rendering;
using UnityEngine;

namespace TES3Unity.Components
{
    /// <summary>
    /// This class is used in editor mode only. It allows to override the GameSettings class.
    /// This is usefull to test custom parameters without the need to change them from the option menu.
    /// </summary>
    public sealed class GameSettingsOverride : MonoBehaviour
    {
        public bool Override = false;
        public GameSettings m_GameSettings = new GameSettings();

        /// <summary>
        /// Apply overriden settings from the editor for testing purpose only.
        /// This method do nothing in a build.
        /// </summary>
        public void ApplyEditorSettingsOverride()
        {
#if UNITY_EDITOR
            if (Override)
            {
                GameSettings.Override(m_GameSettings);
            }
#endif
        }
    }
}
