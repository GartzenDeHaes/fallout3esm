#if UNITY_EDITOR
using TES3Unity.ESM.Records;
using UnityEditor;
using UnityEngine;

namespace TES3Unity.Components.Records
{
    [CustomEditor(typeof(NPC))]
    public sealed class NPC_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (NPC)target;
            var npc = (NPC_Record)script.record;

            if (npc == null)
            {
                return;
            }

            GUILayout.Label($"Name: {npc.Name}");
            GUILayout.Label($"Race: {npc.Race}");
            GUILayout.Label($"Faction: {npc.Faction}");
            GUILayout.Label($"Scale: {npc.Scale}");
        }
    }
}
#endif