using System;
using System.IO;

using TES3Unity.Components;
using TES3Unity.Components.Records;
using TES3Unity.ESM.Records;
using TES3Unity.Rendering;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace TES3Unity
{
	public sealed class TES3Engine : MonoBehaviour
	{
		// Static.
		public const float NormalMapGeneratorIntensity = 0.75f;
		public static int MarkerLayer => LayerMask.NameToLayer("Marker");
		public static int CellRadiusOnLoad = 2;
		public static bool AutoLoadSavedGame = false;
		public static bool LogEnabled = false;

		static TES3Engine _instance = null;

		[NonSerialized]
		public ITES3DataReader DataReader;

		TemporalLoadBalancer m_TemporalLoadBalancer;
		TES3Material m_MaterialManager;
		NIFManager m_NIFManager;

		[Header("Global")]
		public float ambientIntensity = 1.5f;
		public float desiredWorkTimePerFrame = 0.0005f;

		[Header("Debug")]
		public string loadSaveGameFilename = string.Empty;

#if UNITY_EDITOR
		[Header("Editor Only")]
		[SerializeField] bool ForceAlternateDataPath;
		public string[] AlternativeDataPaths = null;
		[SerializeField] int CellRadius = 1;
		[SerializeField] int CellDetailRadius = 1;
		[SerializeField] bool ForceAutoloadSavedGame = true;
		[SerializeField] bool OverrideLogEnabled = false;
#endif

		CELLRecord m_CurrentCell;
		Transform m_PlayerTransform;
		Transform m_CameraTransform;
		bool m_Initialized = false;

		// Public.
		[NonSerialized]
		public CellManager CellManager;
		[NonSerialized]
		public TextureManager TextureManager;

		public CELLRecord CurrentCell
		{
			get => m_CurrentCell;
			private set
			{
				if (m_CurrentCell == value)
				{
					return;
				}

				m_CurrentCell = value;
				CurrentCellChanged?.Invoke(m_CurrentCell);
			}
		}

		public static TES3Engine Inst
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<TES3Engine>();
				}

				return _instance;
			}
		}

		public event Action<CELLRecord> CurrentCellChanged = null;

		void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Destroy(this);
				return;
			}
			else
			{
				_instance = this;
			}

			// When loaded from the Menu, this variable is already preloaded.
			if (DataReader == null)
			{
				var dataPath = GameSettings.GetDataPath();

#if UNITY_EDITOR
				// Load the game from the alternative dataPath when in editor.
				if (ForceAlternateDataPath || !GameSettings.IsValidPath(dataPath))
				{
					dataPath = string.Empty;

					foreach (var alt in AlternativeDataPaths)
					{
						if (GameSettings.IsValidPath(alt))
						{
							dataPath = alt;
							GameSettings.SetDataPath(dataPath);
							break;
						}
					}
				}

				if (ForceAutoloadSavedGame)
				{
					AutoLoadSavedGame = true;
				}

				if (OverrideLogEnabled)
				{
					LogEnabled = true;
				}
#endif

				if (string.IsNullOrEmpty(dataPath))
				{
					SceneManager.LoadScene("Menu");
					enabled = false;
					return;
				}

				if (File.Exists(Path.Combine(dataPath, "Morrowind.esm")))
				{
					DataReader = new TES3DataReader(dataPath);
				}
				else
				{
					//DataReader = new TES4DataReader(dataPath);
				}
				DataReader.Load();
			}
		}

		void Start()
		{
			var config = GameSettings.Get();

#if UNITY_EDITOR
			CellRadius = config.CellRadius;
			CellDetailRadius = config.CellDetailRadius;
#endif

			CellManager.CellRadius = config.CellRadius;
			CellManager.DetailRadius = config.CellDetailRadius;
			CellRadiusOnLoad = config.CellRadiusOnLoad;

			TextureManager = new TextureManager(DataReader);
			m_MaterialManager = new TES3Material(TextureManager, config.ShaderType, config.GenerateNormalMaps);
			m_NIFManager = new NIFManager(DataReader, m_MaterialManager);
			m_TemporalLoadBalancer = new TemporalLoadBalancer();
			CellManager = new CellManager(DataReader, TextureManager, m_NIFManager, m_TemporalLoadBalancer);

			var sunLight = GameObjectUtils.CreateSunLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
			var weatherManager = FindObjectOfType<WeatherManager>();
			weatherManager.SetSun(sunLight);

			var waterPrefab = Resources.Load<GameObject>("Prefabs/WaterRP");
			GameObject.Instantiate(waterPrefab);

#if UNITY_STANDALONE
			var texture = TextureManager.LoadTexture("tx_cursor", true);
			Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
#endif

			var uiCanvasPrefab = Resources.Load<GameObject>("Prefabs/GameUI");
			var uiCanvas = GameObject.Instantiate(uiCanvasPrefab);

			m_Initialized = true;

			var soundManager = FindObjectOfType<SoundManager>();
			soundManager?.Initialize(GameSettings.GetDataPath());

			// Start Position
			var cellGridCoords = new Vector2i(-2, -9);
			var cellIsInterior = false;
			var spawnPosition = new Vector3(-137.94f, 2.30f, -1037.6f);
			var spawnRotation = Quaternion.identity;

			if (AutoLoadSavedGame)
			{
				var save = TES3Save.Get();
				if (!save.IsEmpty())
				{
					cellGridCoords = save.CellGrid;
					cellIsInterior = save.IsInterior;
					spawnPosition = save.Position;
					spawnRotation = save.Rotation;
					GameSettings.Get().Player = save.Data;
				}

				AutoLoadSavedGame = false;
			}

#if UNITY_EDITOR
			// Check for a previously saved game.
			if (loadSaveGameFilename != null)
			{
				var path = $"{DataReader.FolderPath}\\Saves\\{loadSaveGameFilename}.ess";

				if (File.Exists(path))
				{
					var ess = new ESS.ESSFile(path);

					ess.FindStartLocation(out string cellName, out float[] pos, out float[] rot);
					// TODO: Find the correct grid/cell from these data.

					var grid = TES3Engine.Inst.CellManager.GetExteriorCellIndices(new Vector3(pos[0], pos[1], pos[2]));
					var exterior = DataReader.FindExteriorCellRecord(grid);
					var interior = DataReader.FindInteriorCellRecord(cellName);
				}
			}
#endif

			if (GameSettings.IsMobile())
			{
				var touchPrefab = Resources.Load<GameObject>("Input/TouchJoysticks");
				Instantiate(touchPrefab, Vector3.zero, Quaternion.identity);
			}

			SpawnPlayer(cellGridCoords, cellIsInterior, spawnPosition, spawnRotation);
		}

		void OnDisable()
		{
			_instance = null;
		}

		#region Player Spawn

		/// <summary>
		/// Spawns the player inside. Be carefull, the name of the cell is not the same for each languages.
		/// Use it with the correct name.
		/// </summary>
		/// <param name="playerPrefab">The player prefab.</param>
		/// <param name="interiorCellName">The name of the desired cell.</param>
		/// <param name="position">The target position of the player.</param>
		public void SpawnPlayerInside(string interiorCellName, Vector3 position, Quaternion rotation)
		{
			CurrentCell = DataReader.FindInteriorCellRecord(interiorCellName);

			Debug.Assert(m_CurrentCell != null);

			CreatePlayer(position, rotation);

			var cellInfo = CellManager.StartCreatingInteriorCell(interiorCellName);
			m_TemporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
		}

		void SpawnPlayer(Vector2i gridCoords, bool outside, Vector3 position, Quaternion rotation)
		{
			InRangeCellInfo cellInfo = null;

			if (outside)
			{
				CurrentCell = DataReader.FindExteriorCellRecord(gridCoords);
				cellInfo = CellManager.StartCreatingExteriorCell(gridCoords);
			}
			else
			{
				CurrentCell = DataReader.FindInteriorCellRecord(gridCoords);
				cellInfo = CellManager.StartCreatingInteriorCell(gridCoords);
			}

			CreatePlayer(position, rotation);

			m_TemporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
		}

		#endregion

		void LateUpdate()
		{
			if (!m_Initialized)
			{
				return;
			}

			// The current cell can be null if the player is outside of the defined game world.
			if ((m_CurrentCell == null) || !m_CurrentCell.isInterior)
			{
				CellManager.UpdateExteriorCells(m_CameraTransform.position);
			}

			m_TemporalLoadBalancer.RunTasks(desiredWorkTimePerFrame);
		}

		void OnApplicationQuit()
		{
			DataReader?.Close();
		}

		public void OpenDoor(Door component)
		{
			if (!component.doorData.leadsToAnotherCell)
			{
				component.Interact();
			}
			else
			{
				// The door leads to another cell, so destroy all currently loaded cells.
				CellManager.DestroyAllCells();

				// Move the player.
				m_PlayerTransform.position = component.doorData.doorExitPos;
				m_PlayerTransform.localEulerAngles = new Vector3(0, component.doorData.doorExitOrientation.eulerAngles.y, 0);

				// Load the new cell.
				CELLRecord newCell;

				if (component.doorData.leadsToInteriorCell)
				{
					var cellInfo = CellManager.StartCreatingInteriorCell(component.doorData.doorExitName);
					m_TemporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

					newCell = cellInfo.cellRecord;
				}
				else
				{
					var cellIndices = CellManager.GetExteriorCellIndices(component.doorData.doorExitPos);
					newCell = DataReader.FindExteriorCellRecord(cellIndices);

					CellManager.UpdateExteriorCells(m_CameraTransform.position, true, CellRadiusOnLoad);
				}

				CurrentCell = newCell;
			}
		}

		private GameObject CreatePlayer(Vector3 position, Quaternion rotation)
		{
			var player = GameObject.FindWithTag("Player");
			if (player == null)
			{
				var playerPrefabPath = "Prefabs/Player";
				var playerPrefab = Resources.Load<GameObject>(playerPrefabPath);
				player = GameObject.Instantiate(playerPrefab);
				player.name = "Player";
			}

			m_PlayerTransform = player.transform;
			m_PlayerTransform.position = position;
			m_PlayerTransform.rotation = rotation;

			m_CameraTransform = player.GetComponentInChildren<Camera>().transform;

			return player;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			CellManager.CellRadius = CellRadius;
			CellManager.DetailRadius = CellDetailRadius;
		}
#endif
	}
}