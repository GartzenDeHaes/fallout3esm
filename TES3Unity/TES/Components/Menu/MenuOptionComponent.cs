using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace TES3Unity.Components
{
    public class MenuOptionComponent : MonoBehaviour
    {
        private GameSettings m_Settings = null;

        [Header("Audio")]
        [SerializeField]
        private Toggle m_AudioToggle = null;

        [Header("General")]
        [SerializeField]
        private Dropdown m_CellRadiusDd = null;
        [SerializeField]
        private Dropdown m_CellDistanceDd = null;
        [SerializeField]
        private Dropdown m_CellRadiusLoad = null;

        [Header("Graphics")]
        [SerializeField]
        private Dropdown m_PostProcessDd = null;
        [SerializeField]
        private Dropdown m_SRPQuality = null;
        [SerializeField]
        private Dropdown m_ShaderQuality = null;
        [SerializeField]
        private Dropdown m_AntiAliasing = null;
        [SerializeField]
        private Dropdown m_CameraFarClipDropdown = null;
        [SerializeField]
        private Dropdown m_RenderScaleDd = null;
        [SerializeField]
        private Toggle m_GenerateNormalMapsToggle = null;
        [SerializeField]
        private Toggle m_AnimateLightsToggle = null;
        [SerializeField]
        private Toggle m_SunShadowsToggle = null;
        [SerializeField]
        private Toggle m_LightShadowsToggle = null;
        [SerializeField]
        private Toggle m_ExteriorLightsToggle = null;
        [SerializeField]
        private Toggle m_DayNightCycleToggle = null;

        [Header("XR")]
        [SerializeField]
        private Toggle m_FollowHeadToggle = null;
        [SerializeField]
        private Toggle m_RoomScaleToggle = null;
        [SerializeField]
        private Toggle m_Teleportation = null;
        [SerializeField]
        private Toggle m_VRAsyncSpaceWarp = null;
        [SerializeField]
        private Dropdown m_VRFFRLevel = null;
        [SerializeField]
        private Dropdown m_VRFrequency = null;

        private void Awake()
        {
            m_Settings = GameSettings.Get();

            // Cell Distance
            SetupUShortDropdown(m_CellRadiusDd, GameSettings.CellDistanceValues, m_Settings.CellRadius, (i) => m_Settings.CellRadius = GameSettings.CellDistanceValues[i]);
            SetupUShortDropdown(m_CellDistanceDd, GameSettings.CellDistanceValues, m_Settings.CellDetailRadius, (i) => m_Settings.CellDetailRadius = GameSettings.CellDistanceValues[i]);
            SetupUShortDropdown(m_CellRadiusLoad, GameSettings.CellDistanceValues, m_Settings.CellRadiusOnLoad, (i) => m_Settings.CellRadiusOnLoad = GameSettings.CellDistanceValues[i]);

            // Post Processing
            SetupDropdown<PostProcessingQuality>(m_PostProcessDd, (int)m_Settings.PostProcessingQuality, (i) => m_Settings.PostProcessingQuality = (PostProcessingQuality)i);

            // SRP
            SetupDropdown<SRPQuality>(m_SRPQuality, (int)m_Settings.SRPQuality, (i) => m_Settings.SRPQuality = (SRPQuality)i);

            // Shader
            SetupDropdown<ShaderType>(m_ShaderQuality, (int)m_Settings.ShaderType, (i) => m_Settings.ShaderType = (ShaderType)i);

            // AntiAliasing
            SetupDropdown<AntialiasingMode>(m_AntiAliasing, (int)m_Settings.AntiAliasingMode, (i) => m_Settings.AntiAliasingMode = (AntialiasingMode)i);

            // Camera Far Clip
            SetupFloatDropdown(m_CameraFarClipDropdown, GameSettings.CameraFarClipValues, m_Settings.CameraFarClip, (i) => m_Settings.CameraFarClip = GameSettings.CameraFarClipValues[i]);

            // RenderScale
            SetupUShortDropdown(m_RenderScaleDd, GameSettings.RenderScaleValues, m_Settings.VRRenderScale, (i) => m_Settings.VRRenderScale = GameSettings.RenderScaleValues[i]);

            m_AudioToggle.isOn = m_Settings.MusicEnabled;
            m_AudioToggle.onValueChanged.AddListener((b) => m_Settings.MusicEnabled = b);

            m_GenerateNormalMapsToggle.isOn = m_Settings.GenerateNormalMaps;
            m_GenerateNormalMapsToggle.onValueChanged.AddListener((b) => m_Settings.GenerateNormalMaps = b);

            m_AnimateLightsToggle.isOn = m_Settings.AnimateLights;
            m_AnimateLightsToggle.onValueChanged.AddListener((b) => m_Settings.AnimateLights = b);

            m_SunShadowsToggle.isOn = m_Settings.SunShadows;
            m_SunShadowsToggle.onValueChanged.AddListener((b) => m_Settings.SunShadows = b);

            m_LightShadowsToggle.isOn = m_Settings.PonctualLightShadows;
            m_LightShadowsToggle.onValueChanged.AddListener((b) => m_Settings.PonctualLightShadows = b);

            m_ExteriorLightsToggle.isOn = m_Settings.ExteriorLights;
            m_ExteriorLightsToggle.onValueChanged.AddListener((b) => m_Settings.ExteriorLights = b);

            m_DayNightCycleToggle.isOn = m_Settings.DayNightCycle;
            m_DayNightCycleToggle.onValueChanged.AddListener((b) => m_Settings.DayNightCycle = b);

            m_FollowHeadToggle.isOn = m_Settings.VRFollowHead;
            m_FollowHeadToggle.onValueChanged.AddListener((b) => m_Settings.VRFollowHead = b);

            m_RoomScaleToggle.isOn = m_Settings.VRRoomScale;
            m_RoomScaleToggle.onValueChanged.AddListener((b) => m_Settings.VRRoomScale = b);

            m_Teleportation.isOn = m_Settings.VRTeleportation;
            m_Teleportation.onValueChanged.AddListener((b) => m_Settings.VRTeleportation = b);

            m_VRAsyncSpaceWarp.isOn = m_Settings.VRAppSpaceWarp;
            m_VRAsyncSpaceWarp.onValueChanged.AddListener(b => m_Settings.VRAppSpaceWarp = b);

            SetupUShortDropdown(m_VRFFRLevel, GameSettings.VRFFRLevels, m_Settings.VRFFRLevel, (i) => m_Settings.VRFFRLevel = GameSettings.VRFFRLevels[i]);
            SetupUShortDropdown(m_VRFrequency, GameSettings.VRRefreshRates, m_Settings.VRRefreshRate, (i) => m_Settings.VRRefreshRate = GameSettings.VRRefreshRates[i]);
        }

        private void SetupDropdown<T>(Dropdown dropdown, int value, UnityAction<int> callback)
        {
            var names = Enum.GetNames(typeof(T));

            dropdown.options.Clear();

            for (var i = 0; i < names.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData
                {
                    text = names[i]
                });
            }

            dropdown.value = value;
            dropdown.onValueChanged.AddListener(callback);
        }

        private void SetupFloatDropdown(Dropdown dropdown, float[] array, float value, UnityAction<int> callback)
        {
            dropdown.options.Clear();

            var valueIndex = 0;

            for (var i = 0; i < array.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData
                {
                    text = array[i].ToString()
                });

                if (Mathf.Approximately(array[i], value))
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;
            dropdown.onValueChanged.AddListener(callback);
        }

        private void SetupUShortDropdown(Dropdown dropdown, ushort[] array, ushort value, UnityAction<int> callback)
        {
            dropdown.options.Clear();

            var valueIndex = 0;

            for (var i = 0; i < array.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData
                {
                    text = array[i].ToString()
                });

                if (array[i] == value)
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;
            dropdown.onValueChanged.AddListener(callback);
        }

        public void ShowMenu()
        {
            GameSettings.Save();
            var menu = GetComponent<MenuComponent>();
            menu.ShowOptions(false);
        }
    }
}