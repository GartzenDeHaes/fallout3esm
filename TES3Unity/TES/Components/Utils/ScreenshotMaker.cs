using System;
using System.IO;
using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity
{
    public sealed class ScreenshotMaker : MonoBehaviour
    {
        private InputActionMap m_ToolsActionMap = null;

        [SerializeField]
        private int m_ScreenshotSuperSampling = 1;

        private void OnEnable() => m_ToolsActionMap?.Enable();
        private void OnDisable() => m_ToolsActionMap?.Disable();

        private void Start()
        {
            m_ToolsActionMap = InputManager.GetActionMap("Tools");
            m_ToolsActionMap.Enable();
            m_ToolsActionMap["Screenshot"].started += (c) => CaptureScreenshot();
        }

        private string GetSavePath(string folder)
        {
            var path = string.Format("{0}/..", Application.dataPath);

            if (folder != string.Empty)
            {
                path = string.Format("{0}/{1}", path, folder);
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public void CaptureScreenshot()
        {
            var name = string.Format("{0}_{1}.png", Application.productName, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            var folder = GetSavePath("Screenshots");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            ScreenCapture.CaptureScreenshot(Path.Combine(folder, name), m_ScreenshotSuperSampling);
        }
    }
}