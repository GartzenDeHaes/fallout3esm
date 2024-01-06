using System;
using System.Collections.Generic;
using System.IO;

using TES3Unity.ESM;
using TES3Unity.ESM.Records;
using TES3Unity.Rendering;

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TES3Unity
{
	public enum ShaderType
	{
		PBR = 0, Simple
	}

	public enum PostProcessingQuality
	{
		None = 0, Low, Medium, High
	}

	public enum SRPQuality
	{
		Low = 0, High
	}

	[Serializable]
	public sealed class GameSettings
	{
		private const string MorrowindPathKey = "tes3unity.path";
		private const string StorageKey = "tes3unity.settings";
		private const string AndroidFolderName = "TES3Unity";
		private static GameSettings Instance = null;

		public static readonly float[] CameraFarClipValues = new float[]
		{
				50, 150, 250, 500, 1000, 2500
		};

		public static readonly ushort[] RenderScaleValues = new ushort[]
		{
				50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 200
		};

		public static readonly ushort[] CellDistanceValues = new ushort[]
		{
				0, 1 , 2, 3, 4
		};

		public static readonly ushort[] VRFFRLevels = new ushort[]
		{
				0, 1, 2, 3, 4
		};

		public static readonly ushort[] VRRefreshRates = new ushort[]
		{
				60, 72, 80, 90, 120
		};

		public bool MusicEnabled = true;
		public PostProcessingQuality PostProcessingQuality = PostProcessingQuality.High;
		public SRPQuality SRPQuality = SRPQuality.High;
		public AntialiasingMode AntiAliasingMode = AntialiasingMode.FastApproximateAntialiasing;
		public bool GenerateNormalMaps = true;
		public ShaderType ShaderType = ShaderType.PBR;
		public float TerrainError = 5;
		public float TreeDistance = 80;
		public bool AnimateLights = true;
		public bool SunShadows = true;
		public bool PonctualLightShadows = true;
		public bool ExteriorLights = true;
		public float CameraFarClip = 500.0f;
		public ushort CellRadius = 2;
		public ushort CellDetailRadius = 2;
		public ushort CellRadiusOnLoad = 2;
		public bool KinematicRigidbody = true;
		public bool DayNightCycle = false;
		public bool VRFollowHead = true;
		public bool VRTeleportation = true;
		public bool VRRoomScale = false;
		public bool VRAppSpaceWarp = true;
		public ushort VRFFRLevel = 3;
		public ushort VRRefreshRate = 72;
		public ushort VRRenderScale = 100;
		//public bool LoadExtensions = false;
		public PlayerData Player;

		public static NPC_Record GetPlayerRecord()
		{
			var playerData = Get().Player;
			var items = new List<NPCOData>();
			var race = playerData.Race.ToString().ToLower().Replace("_", " ");
			var gender = playerData.Woman ? "f" : "m";

			return NPC_Record.CreateRaw(
				 playerData.Name,
				 playerData.Race.ToString(),
				 playerData.Faction,
				 playerData.ClassName,
				 $"b_n_{race}_{gender}_head_01",
				 $"b_n_{race}_{gender}_hair_00",
				 playerData.Woman ? 1 : 0,
				 items);
		}

		public static void Save()
		{
			var instance = Get();
			var json = JsonUtility.ToJson(instance);
			PlayerPrefs.SetString(StorageKey, json);
			PlayerPrefs.Save();
		}

		public static void Override(GameSettings settings)
		{
			Instance = settings;
		}

		public static GameSettings Get()
		{
			if (Instance == null)
			{
				Instance = new GameSettings();

				Instance.Player = new PlayerData
				{
					Name = "Player",
					Woman = false,
					Race = RaceType.Imperial
				};

#if UNITY_ANDROID || UNITY_IOS
                Instance.GenerateNormalMaps = false;
                Instance.SunShadows = false;
                Instance.VRRenderScale = 90;
                Instance.PostProcessingQuality = PostProcessingQuality.None;
                Instance.ExteriorLights = false;
                Instance.ShaderType = ShaderType.Simple;
                Instance.CameraFarClip = 200;
                Instance.DayNightCycle = false;
                Instance.AntiAliasingMode = AntialiasingMode.None;
                Instance.CellDetailRadius = 2;
                Instance.CellRadius = 1;
                Instance.VRAppSpaceWarp = true;
#endif
				if (PlayerPrefs.HasKey(StorageKey))
				{
					var json = PlayerPrefs.GetString(StorageKey);
					if (!string.IsNullOrEmpty(json) && json != "{}")
					{
						Instance = JsonUtility.FromJson<GameSettings>(json);
					}
				}
			}

			return Instance;
		}

		#region Static Functions

		public static bool IsMobile()
		{
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
			return false;
#endif
		}

		public static LightShadows GetRecommandedShadows(bool ponctual)
		{
			var config = Get();

			if (!config.PonctualLightShadows && ponctual || !config.SunShadows && !ponctual)
			{
				return LightShadows.None;
			}
			else if (IsMobile() || config.SRPQuality == SRPQuality.Low)
			{
				return LightShadows.Hard;
			}

			return LightShadows.Soft;
		}

		public static bool IsValidPath(string path)
		{
			return File.Exists(Path.Combine(path, "Morrowind.esm")) ||
				File.Exists(Path.Combine(path, "Fallout3.esm")) ||
				File.Exists(Path.Combine(path, "Falloutnv.esm")) ||
				File.Exists(Path.Combine(path, "Skyrim.esm")) ||
				File.Exists(Path.Combine(path, "Oblivion.esm"));
		}

		public static void SetDataPath(string dataPath)
		{
			PlayerPrefs.SetString(MorrowindPathKey, dataPath);
		}

		public static string GetDataPath()
		{
			var path = PlayerPrefs.GetString(MorrowindPathKey);

			if (!string.IsNullOrEmpty(path))
			{
				return path;
			}

#if UNITY_ANDROID
            return $"/sdcard/{AndroidFolderName}/Data Files";
#else
			return string.Empty;
#endif
		}

		#endregion
	}
}
