using System.IO;

using TES3Unity.ESM.Records;

using UnityEditor;

using UnityEngine;

namespace TES3Unity.Diagnostic
{
#if UNITY_EDITOR
	public class ScriptExporter : MonoBehaviour
	{
		[MenuItem("Morrowind Unity/Export Scripts")]
		private static void ExportScripts()
		{
			if (TES3Engine.Inst.DataReader == null)
			{
				Debug.LogWarning("Morrowind Data are not yet loaded. It'll take some time to load. The editor will be freezed a bit...");

				var dataPath = GameSettings.GetDataPath();

				// Load the game from the alternative dataPath when in editor.
				if (!GameSettings.IsValidPath(dataPath))
				{
					dataPath = string.Empty;

					var manager = FindObjectOfType<TES3Engine>();
					var alternativeDataPaths = manager?.AlternativeDataPaths ?? null;

					if (alternativeDataPaths == null)
					{
						Debug.LogError("No valid path was found. You can try to add a TESManager component on the scene with an alternative path.");
						return;
					}

					foreach (var alt in alternativeDataPaths)
					{
						if (GameSettings.IsValidPath(alt))
						{
							dataPath = alt;
						}
					}
				}

				TES3Engine.Inst.DataReader = new TES3DataReader(dataPath);
				TES3Engine.Inst.DataReader.Load();

				Debug.Log("Morrowind Data are now loaded!");
			}

			var exportPath = $"Exports/Scripts";

			if (TES3Engine.Inst.DataReader is TES3DataReader reader)
			{
				if (!TES3Engine.Inst.DataReader.TryDumpScripts(out var scripts))
				{
					Debug.LogError("Can't retrieve scripts from the ESM file. There is probably a big problem...");
					return;
				}

				Debug.Log($"Exporting {scripts.Length} scripts into {exportPath}");

				if (!Directory.Exists(exportPath))
				{
					Directory.CreateDirectory(exportPath);
				}

				foreach (var script in scripts)
				{
					// We use the .vb extention because it's easier to read scripts with a text editor in basic language mode.
					File.WriteAllText($"{exportPath}/{script.Metadata.Name}.vb", script.Text);
				}

				Debug.Log("Script export done.");
			}
			else
			{
				Debug.Log("TES4 script export not supported.");
			}
		}
	}
#endif
}
