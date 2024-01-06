using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TES3Unity.Components.Records;
using TES3Unity.ESM;
using TES3Unity.ESM.Records;
using TES3Unity.World;

using UnityEngine;

namespace TES3Unity
{
	public class InRangeCellInfo
	{
		public GameObject gameObject;
		public GameObject objectsContainerGameObject;
		public CELLRecord cellRecord;
		public IEnumerator objectsCreationCoroutine;

		public InRangeCellInfo(GameObject gameObject, GameObject objectsContainerGameObject, CELLRecord cellRecord, IEnumerator objectsCreationCoroutine)
		{
			this.gameObject = gameObject;
			this.objectsContainerGameObject = objectsContainerGameObject;
			this.cellRecord = cellRecord;
			this.objectsCreationCoroutine = objectsCreationCoroutine;
		}
	}

	public class RefCellObjInfo
	{
		public RefObjDataGroup refObjDataGroup;
		public Record referencedRecord;
		public string modelFilePath;
	}

	public class CellManager
	{
		public static int CellRadius = 4;
		public static int DetailRadius = 3;
		const string _defaultLandTextureFilePath = "textures/_land_default.dds";
		static Dictionary<Texture2D, TerrainLayer> _terrainLayers = new Dictionary<Texture2D, TerrainLayer>();

		ITES3DataReader _dataReader;
		TextureManager _textureManager;
		NIFManager _nifManager;
		TemporalLoadBalancer _temporalLoadBalancer;
		Dictionary<Vector2i, InRangeCellInfo> _cellObjects = new Dictionary<Vector2i, InRangeCellInfo>();

		public CellManager(ITES3DataReader dataReader, TextureManager textureManager, NIFManager nifManager, TemporalLoadBalancer temporalLoadBalancer)
		{
			this._dataReader = dataReader;
			this._textureManager = textureManager;
			this._nifManager = nifManager;
			this._temporalLoadBalancer = temporalLoadBalancer;
		}

		public Vector2i GetExteriorCellIndices(Vector3 point)
		{
			return new Vector2i(Mathf.FloorToInt(point.x / Convert.ExteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / Convert.ExteriorCellSideLengthInMeters));
		}

		public InRangeCellInfo StartCreatingExteriorCell(Vector2i cellIndices)
		{
			var CELL = _dataReader.FindExteriorCellRecord(cellIndices);

			if (CELL != null)
			{
				var cellInfo = StartInstantiatingCell(CELL);
				_cellObjects[cellIndices] = cellInfo;

				return cellInfo;
			}
			else
			{
				return null;
			}
		}

		public void UpdateExteriorCells(Vector3 currentPosition, bool immediate = false, int cellRadiusOverride = -1)
		{
			var cameraCellIndices = GetExteriorCellIndices(currentPosition);

			var cellRadius = (cellRadiusOverride >= 0) ? cellRadiusOverride : CellManager.CellRadius;
			var minCellX = cameraCellIndices.X - cellRadius;
			var maxCellX = cameraCellIndices.X + cellRadius;
			var minCellY = cameraCellIndices.Y - cellRadius;
			var maxCellY = cameraCellIndices.Y + cellRadius;

			// Destroy out of range cells.
			var outOfRangeCellIndices = new List<Vector2i>();

			foreach (var KVPair in _cellObjects)
			{
				if ((KVPair.Key.X < minCellX) || (KVPair.Key.X > maxCellX) || (KVPair.Key.Y < minCellY) || (KVPair.Key.Y > maxCellY))
				{
					outOfRangeCellIndices.Add(KVPair.Key);
				}
			}

			foreach (var cellIndices in outOfRangeCellIndices)
			{
				DestroyExteriorCell(cellIndices);
			}

			// Create new cells.
			for (int r = 0; r <= cellRadius; r++)
			{
				for (int x = minCellX; x <= maxCellX; x++)
				{
					for (int y = minCellY; y <= maxCellY; y++)
					{
						var cellIndices = new Vector2i(x, y);

						var cellXDistance = Mathf.Abs(cameraCellIndices.X - cellIndices.X);
						var cellYDistance = Mathf.Abs(cameraCellIndices.Y - cellIndices.Y);
						var cellDistance = Mathf.Max(cellXDistance, cellYDistance);

						if ((cellDistance == r) && !_cellObjects.ContainsKey(cellIndices))
						{
							var cellInfo = StartCreatingExteriorCell(cellIndices);

							if ((cellInfo != null) && immediate)
							{
								_temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
							}
						}
					}
				}
			}

			// Update LODs.
			foreach (var keyValuePair in _cellObjects)
			{
				Vector2i cellIndices = keyValuePair.Key;
				InRangeCellInfo cellInfo = keyValuePair.Value;

				var cellXDistance = Mathf.Abs(cameraCellIndices.X - cellIndices.X);
				var cellYDistance = Mathf.Abs(cameraCellIndices.Y - cellIndices.Y);
				var cellDistance = Mathf.Max(cellXDistance, cellYDistance);

				if (cellDistance <= DetailRadius)
				{
					if (!cellInfo.objectsContainerGameObject.activeSelf)
					{
						cellInfo.objectsContainerGameObject.SetActive(true);
					}
				}
				else
				{
					if (cellInfo.objectsContainerGameObject.activeSelf)
					{
						cellInfo.objectsContainerGameObject.SetActive(false);
					}
				}
			}
		}

		public InRangeCellInfo StartCreatingInteriorCell(string cellName)
		{
			var CELL = _dataReader.FindInteriorCellRecord(cellName);
			return StartInstantiatingCell(CELL);
		}

		public InRangeCellInfo StartCreatingInteriorCell(Vector2i gridCoords)
		{
			var CELL = _dataReader.FindInteriorCellRecord(gridCoords);
			return StartInstantiatingCell(CELL);
		}

		public InRangeCellInfo StartCreatingInteriorCell(CELLRecord record)
		{
			if (record != null)
			{
				var cellInfo = StartInstantiatingCell(record);
				_cellObjects[Vector2i.Zero] = cellInfo;
				return cellInfo;
			}

			return null;
		}

		InRangeCellInfo StartInstantiatingCell(CELLRecord CELL)
		{
			Debug.Assert(CELL != null);

			string cellObjName = null;
			LANDRecord LAND = null;

			if (!CELL.isInterior)
			{
				cellObjName = "cell " + CELL.gridCoords.ToString();
				LAND = _dataReader.FindLANDRecord(CELL.gridCoords);
			}
			else
			{
				cellObjName = CELL.NAME.value;
			}

			var cellObj = new GameObject(cellObjName);
			cellObj.tag = "Cell";

			var cellObjectsContainer = new GameObject("objects");
			cellObjectsContainer.transform.parent = cellObj.transform;

			var cellObjectsCreationCoroutine = InstantiateCellObjectsCoroutine(CELL, LAND, cellObj, cellObjectsContainer);
			_temporalLoadBalancer.AddTask(cellObjectsCreationCoroutine);

			return new InRangeCellInfo(cellObj, cellObjectsContainer, CELL, cellObjectsCreationCoroutine);
		}

		public void DestroyAllCells()
		{
			foreach (var keyValuePair in _cellObjects)
			{
				_temporalLoadBalancer.CancelTask(keyValuePair.Value.objectsCreationCoroutine);
				GameObject.Destroy(keyValuePair.Value.gameObject);
			}

			_cellObjects.Clear();
		}

		/// <summary>
		/// A coroutine that instantiates the terrain for, and all objects in, a cell.
		/// </summary>
		private IEnumerator InstantiateCellObjectsCoroutine(CELLRecord CELL, LANDRecord LAND, GameObject cellObj, GameObject cellObjectsContainer)
		{
			if (CELL == null && LAND == null)
			{
				yield break;
			}

			// Start pre-loading all required textures for the terrain.
			if (LAND != null)
			{
				GetLANDTextureFilePaths(LAND);
			}

			// Extract information about referenced objects.
			var refCellObjInfos = GetRefCellObjInfos(CELL);

			// Instantiate terrain.
			if (LAND != null)
			{
				var instantiateLANDTaskEnumerator = InstantiateLANDCoroutine(LAND, cellObj);

				// Run the LAND instantiation coroutine.
				while (instantiateLANDTaskEnumerator.MoveNext())
				{
					// Yield every time InstantiateLANDCoroutine does to avoid doing too much work in one frame.
					yield return null;
				}

				yield return null;
			}

			// Instantiate objects.
			foreach (var refCellObjInfo in refCellObjInfos)
			{
				InstantiateCellObject(CELL, cellObjectsContainer, refCellObjInfo);
			}

			InstantiateReflectionProbe(CELL, cellObj.transform);

			yield return null;
		}

		private RefCellObjInfo[] GetRefCellObjInfos(CELLRecord CELL)
		{
			var count = CELL.refObjDataGroups.Count;
			var refCellObjInfos = new RefCellObjInfo[count];

			for (int i = 0; i < count; i++)
			{
				var refObjInfo = new RefCellObjInfo();
				refObjInfo.refObjDataGroup = CELL.refObjDataGroups[i];

				// Get the record the RefObjDataGroup references.
				_dataReader.TryGetObjectById(refObjInfo.refObjDataGroup.NAME.value, out refObjInfo.referencedRecord);

				if (refObjInfo.referencedRecord != null)
				{
					var modelFileName = string.Empty;

					var modelRecord = refObjInfo.referencedRecord as IModelRecord;
					if (modelRecord != null)
					{
						modelFileName = modelRecord.Model;
					}

					// If the model file name is valid, store the model file path.
					if (!string.IsNullOrEmpty(modelFileName))
					{
						refObjInfo.modelFilePath = "meshes\\" + modelFileName;
					}
				}

				refCellObjInfos[i] = refObjInfo;
			}

			return refCellObjInfos;
		}

		/// <summary>
		/// Instantiates an object in a cell. Called by InstantiateCellObjectsCoroutine after the object's assets have been pre-loaded.
		/// </summary>
		private void InstantiateCellObject(CELLRecord CELL, GameObject parent, RefCellObjInfo refCellObjInfo)
		{
			if (refCellObjInfo.referencedRecord != null)
			{
				GameObject modelObj = null;

				// If the object has a model, instantiate it.
				if (refCellObjInfo.modelFilePath != null)
				{
					modelObj = _nifManager.InstantiateNIF(refCellObjInfo.modelFilePath);
					PostProcessInstantiatedCellObject(modelObj, refCellObjInfo);

					modelObj.transform.parent = parent.transform;
				}

				if (refCellObjInfo.referencedRecord is NPC_Record)
				{
					var NPC_ = (NPC_Record)refCellObjInfo.referencedRecord;
					var npcGameObject = NPCFactory.InstanciateNPC(_nifManager, NPC_);

					PostProcessInstantiatedCellObject(npcGameObject, refCellObjInfo);
					npcGameObject.transform.parent = parent.transform;
				}

				// If the object has a light, instantiate it.
				if (refCellObjInfo.referencedRecord is LIGHRecord)
				{
					var lightObj = InstantiateLight((LIGHRecord)refCellObjInfo.referencedRecord, CELL.isInterior);

					// If the object also has a model, parent the model to the light.
					if (modelObj != null)
					{
						// Some NIF files have nodes named "AttachLight". Parent it to the light if it exists.
						GameObject attachLightObj = GameObjectUtils.FindChildRecursively(modelObj, "AttachLight");

						if (attachLightObj == null)
						{
							//attachLightObj = GameObjectUtils.FindChildWithNameSubstringRecursively(modelObj, "Emitter");
							attachLightObj = modelObj;
						}

						if (attachLightObj != null)
						{
							lightObj.transform.position = attachLightObj.transform.position;
							lightObj.transform.rotation = attachLightObj.transform.rotation;

							lightObj.transform.parent = attachLightObj.transform;
						}
						else // If there is no "AttachLight", center the light in the model's bounds.
						{
							lightObj.transform.position = GameObjectUtils.CalcVisualBoundsRecursive(modelObj).center;
							lightObj.transform.rotation = modelObj.transform.rotation;

							lightObj.transform.parent = modelObj.transform;
						}
					}
					else // If the light has no associated model, instantiate the light as a standalone object.
					{
						PostProcessInstantiatedCellObject(lightObj, refCellObjInfo);
						lightObj.transform.parent = parent.transform;
					}
				}
			}
			else
			{
				if (TES3Engine.LogEnabled)
				{
					Debug.Log("Unknown Object: " + refCellObjInfo.refObjDataGroup.NAME.value);
				}
			}
		}

		private GameObject InstantiateLight(LIGHRecord LIGH, bool indoors)
		{
			var config = GameSettings.Get();

			var lightObj = new GameObject("Light");
			lightObj.isStatic = true;

			var lightComponent = lightObj.AddComponent<Light>();
			var data = LIGH.Data.GetValueOrDefault();
			lightComponent.range = 3 * (data.Radius / Convert.MeterInMWUnits);
			lightComponent.color = new Color32(data.Red, data.Green, data.Blue, 255);
			lightComponent.intensity = 1.5f;
			lightComponent.bounceIntensity = 0f;
			lightComponent.shadows = GameSettings.GetRecommandedShadows(true);

#if UNITY_ANDROID || UNITY_IOS
				lightComponent.renderMode = LightRenderMode.ForceVertex;
#endif

			if (!indoors && !config.ExteriorLights) // disabling exterior cell lights because there is no day/night cycle
			{
				lightComponent.enabled = false;
			}

			return lightObj;
		}

		/// <summary>
		/// Finishes initializing an instantiated cell object.
		/// </summary>
		private void PostProcessInstantiatedCellObject(GameObject gameObject, RefCellObjInfo refCellObjInfo)
		{
			var refObjDataGroup = refCellObjInfo.refObjDataGroup;

			// Handle object transforms.
			if (refObjDataGroup.XSCL != null)
			{
				gameObject.transform.localScale = Vector3.one * refObjDataGroup.XSCL.value;
			}

			gameObject.transform.position += NIFUtils.NifPointToUnityPoint(refObjDataGroup.DATA.position);
			gameObject.transform.rotation *= NIFUtils.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DATA.eulerAngles);

			var tagTarget = gameObject;
			var coll = gameObject.GetComponentInChildren<Collider>(); // if the collider is on a child object and not on the object with the component, we need to set that object's tag instead.
			if (coll != null)
			{
				tagTarget = coll.gameObject;
			}

			ProcessObjectType<DOORRecord>(tagTarget, refCellObjInfo, "Door");
			ProcessObjectType<ACTIRecord>(tagTarget, refCellObjInfo, "Activator");
			ProcessObjectType<CONTRecord>(tagTarget, refCellObjInfo, "Container");
			ProcessObjectType<LIGHRecord>(tagTarget, refCellObjInfo, "Light");
			ProcessObjectType<LOCKRecord>(tagTarget, refCellObjInfo, "Lock");
			ProcessObjectType<PROBRecord>(tagTarget, refCellObjInfo, "Probe");
			ProcessObjectType<REPARecord>(tagTarget, refCellObjInfo, "RepairTool");
			ProcessObjectType<WEAPRecord>(tagTarget, refCellObjInfo, "Weapon");
			ProcessObjectType<CLOTRecord>(tagTarget, refCellObjInfo, "Clothing");
			ProcessObjectType<ARMORecord>(tagTarget, refCellObjInfo, "Armor");
			ProcessObjectType<INGRRecord>(tagTarget, refCellObjInfo, "Ingredient");
			ProcessObjectType<ALCHRecord>(tagTarget, refCellObjInfo, "Alchemical");
			ProcessObjectType<APPARecord>(tagTarget, refCellObjInfo, "Apparatus");
			ProcessObjectType<BOOKRecord>(tagTarget, refCellObjInfo, "Book");
			ProcessObjectType<MISCRecord>(tagTarget, refCellObjInfo, "MiscObj");
			ProcessObjectType<CREARecord>(tagTarget, refCellObjInfo, "Creature");
			ProcessObjectType<NPC_Record>(tagTarget, refCellObjInfo, "NPC");
		}

		private void ProcessObjectType<RecordType>(GameObject gameObject, RefCellObjInfo info, string tag) where RecordType : Record
		{
			var record = info.referencedRecord;
			if (record is RecordType)
			{
				var obj = GameObjectUtils.FindTopLevelObject(gameObject);
				if (obj == null)
				{
					return;
				}

				var component = RecordComponent.Create(obj, record, tag);

				//only door records need access to the cell object data group so far
				if (record is DOORRecord)
				{
					((Door)component).refObjDataGroup = info.refObjDataGroup;
				}
			}
		}

		private List<string> GetLANDTextureFilePaths(LANDRecord LAND)
		{
			var heightField = LAND.VertexHeightFieldData;
			var vertexIndice = LAND.VertexIndiceData;

			// Don't return anything if the LAND doesn't have height data or texture data.
			if ((heightField.HeightOffsets == null) || (vertexIndice == null))
			{
				return null;
			}

			var textureFilePaths = new List<string>();
			var distinctTextureIndices = vertexIndice.Distinct().ToList();
			for (int i = 0; i < distinctTextureIndices.Count; i++)
			{
				short textureIndex = (short)((short)distinctTextureIndices[i] - 1);

				if (textureIndex < 0)
				{
					textureFilePaths.Add(_defaultLandTextureFilePath);
					continue;
				}
				else
				{
					var LTEX = _dataReader.FindLTEXRecord(textureIndex);
					var textureFilePath = LTEX.Texture;
					textureFilePaths.Add(textureFilePath);
				}
			}

			return textureFilePaths;
		}

		/// <summary>
		/// Creates terrain representing a LAND record.
		/// </summary>
		private IEnumerator InstantiateLANDCoroutine(LANDRecord LAND, GameObject parent)
		{
			Debug.Assert(LAND != null);

			var heightField = LAND.VertexHeightFieldData;
			var heightOffsets = heightField.HeightOffsets;
			var textureIndice = LAND.VertexIndiceData;

			// Don't create anything if the LAND doesn't have height data.
			if (heightOffsets == null)
			{
				yield break;
			}

			const int LAND_SIDE_LENGTH_IN_SAMPLES = 65;
			var heights = new float[LAND_SIDE_LENGTH_IN_SAMPLES, LAND_SIDE_LENGTH_IN_SAMPLES];

			// Read in the heights in Morrowind units.
			const int VHGTIncrementToMWUnits = 8;
			float rowOffset = heightField.ReferenceHeight;

			for (int y = 0; y < LAND_SIDE_LENGTH_IN_SAMPLES; y++)
			{
				rowOffset += heightOffsets[y * LAND_SIDE_LENGTH_IN_SAMPLES];
				heights[y, 0] = VHGTIncrementToMWUnits * rowOffset;

				float colOffset = rowOffset;

				for (int x = 1; x < LAND_SIDE_LENGTH_IN_SAMPLES; x++)
				{
					colOffset += heightOffsets[(y * LAND_SIDE_LENGTH_IN_SAMPLES) + x];
					heights[y, x] = VHGTIncrementToMWUnits * colOffset;
				}
			}

			// Change the heights to percentages.
			float minHeight, maxHeight;
			ArrayUtils.GetExtrema(heights, out minHeight, out maxHeight);

			for (int y = 0; y < LAND_SIDE_LENGTH_IN_SAMPLES; y++)
			{
				for (int x = 0; x < LAND_SIDE_LENGTH_IN_SAMPLES; x++)
				{
					heights[y, x] = Utils.ChangeRange(heights[y, x], minHeight, maxHeight, 0, 1);
				}
			}

			// Texture the terrain.
			TerrainLayer[] splatPrototypes = null;
			TerrainLayer splat = null;
			float[,,] alphaMap = null;

			const int LAND_TEXTURE_INDICES_COUNT = 256;
			var textureIndices = textureIndice ?? new ushort[LAND_TEXTURE_INDICES_COUNT];

			// Create splat prototypes.
			var splatPrototypeList = new List<TerrainLayer>();
			var texInd2splatInd = new Dictionary<ushort, int>();

			for (int i = 0; i < textureIndices.Length; i++)
			{
				short textureIndex = (short)((short)textureIndices[i] - 1);

				if (!texInd2splatInd.ContainsKey((ushort)textureIndex))
				{
					// Load terrain texture.
					string textureFilePath;

					if (textureIndex < 0)
					{
						textureFilePath = _defaultLandTextureFilePath;
					}
					else
					{
						var LTEX = _dataReader.FindLTEXRecord(textureIndex);
						textureFilePath = LTEX.Texture;
					}

					var texture = _textureManager.LoadTexture(textureFilePath);

					if (!_terrainLayers.ContainsKey(texture))
					{
						// Create the splat prototype.
						splat = new TerrainLayer();
						splat.diffuseTexture = texture;
						splat.smoothness = 0.075f;
						splat.metallic = 0.05f;
						splat.specular = Color.black;
						splat.maskMapTexture = TextureManager.CreateMaskTexture(splat.metallic, 1, 0, splat.smoothness);

						if (GameSettings.Get().GenerateNormalMaps)
						{
							splat.normalMapTexture = TextureManager.CreateNormalMapTexture(texture);
						}

						splat.tileSize = new Vector2(6, 6);

						_terrainLayers.Add(texture, splat);
					}
					else
					{
						splat = _terrainLayers[texture];
					}

					// Update collections.
					var splatIndex = splatPrototypeList.Count;
					splatPrototypeList.Add(splat);
					texInd2splatInd.Add((ushort)textureIndex, splatIndex);
				}
			}

			splatPrototypes = splatPrototypeList.ToArray();

			yield return null;

			// Create the alpha map.
			int VTEX_ROWS = 16;
			int VTEX_COLUMNS = VTEX_ROWS;
			alphaMap = new float[VTEX_ROWS, VTEX_COLUMNS, splatPrototypes.Length];

			for (int y = 0; y < VTEX_ROWS; y++)
			{
				var yMajor = y / 4;
				var yMinor = y - (yMajor * 4);

				for (int x = 0; x < VTEX_COLUMNS; x++)
				{
					var xMajor = x / 4;
					var xMinor = x - (xMajor * 4);

					var texIndex = (short)((short)textureIndices[(yMajor * 64) + (xMajor * 16) + (yMinor * 4) + xMinor] - 1);

					if (texIndex >= 0)
					{
						var splatIndex = texInd2splatInd[(ushort)texIndex];

						alphaMap[y, x, splatIndex] = 1;
					}
					else
					{
						alphaMap[y, x, 0] = 1;
					}
				}
			}

			// Create the terrain.
			var gridCoords = LAND.GridCoords;
			var heightRange = maxHeight - minHeight;
			var terrainPosition = new Vector3(Convert.ExteriorCellSideLengthInMeters * gridCoords.X, minHeight / Convert.MeterInMWUnits, Convert.ExteriorCellSideLengthInMeters * gridCoords.Y);
			var heightSampleDistance = Convert.ExteriorCellSideLengthInMeters / (LAND_SIDE_LENGTH_IN_SAMPLES - 1);
			var terrainGameObject = GameObjectUtils.CreateTerrain(heights, heightRange / Convert.MeterInMWUnits, heightSampleDistance, splatPrototypes, alphaMap, terrainPosition);
			terrainGameObject.transform.parent = parent.transform;

			yield return null;
		}

		private void InstantiateReflectionProbe(CELLRecord cell, Transform parent)
		{
			if (cell.isInterior)
			{
				// FIXME
				return;
			}

			var gridCoords = cell.gridCoords;
			//var bounds = GameObjectUtils.CalcVisualBoundsRecursive(parent.gameObject);
			var position = new Vector3(Convert.ExteriorCellSideLengthInMeters * gridCoords.X, 0, Convert.ExteriorCellSideLengthInMeters * gridCoords.Y);
			//var position = bounds.center;

			var probe = new GameObject("ReflectionProbe");
			probe.transform.parent = parent;
			probe.transform.position = position;

			var rp = probe.AddComponent<ReflectionProbe>();
			rp.size = new Vector3(120, 120, 120);
			rp.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
			rp.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
			rp.RenderProbe();
		}

		private void DestroyExteriorCell(Vector2i indices)
		{
			InRangeCellInfo cellInfo;

			if (_cellObjects.TryGetValue(indices, out cellInfo))
			{
				_temporalLoadBalancer.CancelTask(cellInfo.objectsCreationCoroutine);
				GameObject.Destroy(cellInfo.gameObject);
				_cellObjects.Remove(indices);
			}
			else
			{
				Debug.LogError("Tried to destroy a cell that isn't created.");
			}
		}
	}
}
