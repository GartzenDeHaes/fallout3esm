using UnityEditor;
using UnityEngine;

namespace TES3Unity.Components.Utilities
{
    [CustomEditor(typeof(ScreenshotMaker))]
    public class ScreenshotCapturerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (ScreenshotMaker)target;

            if (GUILayout.Button("Capture Screenshot"))
                script.CaptureScreenshot();
        }
    }
}
