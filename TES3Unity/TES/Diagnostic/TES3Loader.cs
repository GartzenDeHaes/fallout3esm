using System;
using System.Collections;
using System.IO;

using TES3Unity.ESM;
using TES3Unity.ESS;
using TES3Unity.Rendering;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace TES3Unity
{
	using Logger = UnityEngine.Debug;

	public class TES3Loader : MonoBehaviour
	{
		[NonSerialized, HideInInspector]
		public TextureManager TextureManager;
		[NonSerialized, HideInInspector]
		public TES3Material MaterialManager;
		[NonSerialized, HideInInspector]
		public NIFManager NifManager;

		[SerializeField]
		public string[] m_AlternativeDataPaths = null;
		//[SerializeField]
		//public bool m_LoadExtensions = false;
		//[SerializeField]
		//private string[] m_LoadMods = null;
		[SerializeField]
		public string m_LoadSaveGameFilename = string.Empty;

		public event Action<ESSFile> SaveFileLoaded = null;

		bool _isInitialized;
		Action<TES3Loader> _onInitialized;
		public Action<TES3Loader> Initialized
		{
			get { return _onInitialized; }
			set 
			{
				_onInitialized += value;
				if (_isInitialized)
				{
					value(this);
				}
			}
		}

		void Awake()
		{
			var watch = new System.Diagnostics.Stopwatch();

			var dataPath = GameSettings.GetDataPath();

			// Load the game from the alternative dataPath when in editor.
			if (!GameSettings.IsValidPath(dataPath))
			{
				dataPath = string.Empty;

				foreach (var alt in m_AlternativeDataPaths)
				{
					if (GameSettings.IsValidPath(alt))
					{
						dataPath = alt;
						break;
					}
				}
			}

			if (String.IsNullOrEmpty(dataPath))
			{
				SceneManager.LoadScene("Menu");
				enabled = false;
				return;
			}

			watch.Start();

			if (TES3Engine.Inst.DataReader == null)
			{
				TES3Engine.Inst.DataReader = new TES3DataReader(dataPath);
				TES3Engine.Inst.DataReader.Load();
			}

			watch.Stop();
			Logger.Log($"DataReader took {watch.Elapsed.Seconds} seconds to load.");

			var config = GameSettings.Get();

			TextureManager = new TextureManager(TES3Engine.Inst.DataReader);
			MaterialManager = new TES3Material(TextureManager, config.ShaderType, config.GenerateNormalMaps);
			NifManager = new NIFManager(TES3Engine.Inst.DataReader, MaterialManager);

			// Mod loading
			//if (m_LoadMods != null && m_LoadMods.Length > 0)
			//{
			//    watch.Reset();
			//    watch.Start();

			//    foreach (var mod in m_LoadMods)
			//    {
			//        // TODO: load mod.
			//    }

			//    watch.Stop();
			//    Logger.Log($"Mods took {watch.Elapsed.Seconds} seconds to load.");
			//}

			// Check for a previously saved game.
			if (m_LoadSaveGameFilename != null)
			{
				var path = $"{TES3Engine.Inst.DataReader.FolderPath}\\Saves\\{m_LoadSaveGameFilename}.ess";

				if (File.Exists(path))
				{
					var ess = new ESS.ESSFile(path);
					SaveFileLoaded?.Invoke(ess);
				}
			}

			StartCoroutine(PostInit());
		}

		IEnumerator PostInit()
		{
			yield return null;

			_isInitialized = true;

			Initialized?.Invoke(this);
		}

		private void OnApplicationQuit()
		{
			TES3Engine.Inst.DataReader?.Close();
		}

#if UNITY_EDITOR
		private static TES3DataReader MWDataReader = null;

		[UnityEditor.MenuItem("Morrowind Unity/Export Random NPC")]
		private static void ExportFirstNPC()
		{
			if (MWDataReader == null)
			{
				Debug.LogWarning("Morrowind Data are not yet loaded. It'll take some time to load. The editor will be freezed a bit...");

				var dataPath = GameSettings.GetDataPath();

				var manager = FindObjectOfType<TES3Loader>();
				var alternativeDataPaths = manager?.m_AlternativeDataPaths ?? null;

				// Load the game from the alternative dataPath when in editor.
				if (!GameSettings.IsValidPath(dataPath))
				{
					dataPath = string.Empty;

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

				MWDataReader = new TES3DataReader(dataPath);
				MWDataReader.Load();

				Debug.Log("Morrowind Data are now loaded!");
			}

			var exportPath = $"Exports/NPCs";
			var npcs = MWDataReader.FindRecords<ESM.Records.NPC_Record>();

			if (npcs == null)
			{
				Debug.LogError("Can't retrieve NPCs from the ESM file. There is probably a big problem...");
				return;
			}

			if (!Directory.Exists(exportPath))
			{
				Directory.CreateDirectory(exportPath);
			}

			var npc = npcs[UnityEngine.Random.Range(0, npcs.Length - 1)];
			var sb = new System.Text.StringBuilder();
			sb.Append("[General]\n");
			sb.Append($"Name = {npc.Name}\n");
			sb.Append($"Model = {npc.Model}\n");
			sb.Append($"Head = {npc.HeadModel}\n");
			sb.Append($"Hair = {npc.HairModel}\n");
			sb.Append($"Faction = {npc.Faction}\n");
			sb.Append($"Race = {npc.Race}\n");
			sb.Append($"Gender = {(Utils.ContainsBitFlags((uint)npc.Flags, (uint)ESM.Records.NPCFlags.Female) ? "f" : "m")}\n");
			sb.Append($"Class: {npc.Class}\n");
			sb.Append($"Scale: {npc.Scale}\n");

			if (npc.Items.Count > 0)
			{
				sb.Append("[Items]\n");
				foreach (var item in npc.Items)
				{
					sb.Append($"{item.Name} x{item.Count}");
					sb.Append("\n");
				}
			}

			if (npc.Spells.Count > 0)
			{
				sb.Append("[Spells]\n");
				foreach (var item in npc.Spells)
				{
					sb.Append($"{item}\n");
				}
			}

			File.WriteAllText($"{exportPath}/{npc.Name}.ini", sb.ToString());

			Debug.Log("Export done.");
		}
#endif
	}
}