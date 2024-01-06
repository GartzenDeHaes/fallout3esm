using System.Collections.Generic;
using TES3Unity.Rendering;
using UnityEngine;

namespace TES3Unity
{
    /// <summary>
    /// Manages loading and instantiation of NIF models.
    /// </summary>
    public sealed class NIFManager
    {
        private static NIFManager _instance;
        private ITESDataReader _dataReader;
        private TES3Material _materialManager;
        private GameObject _prefabContainerObj;
        private Dictionary<string, GameObject> nifPrefabs = new Dictionary<string, GameObject>();

        public static NIFManager Instance => _instance;

        public NIFManager(ITESDataReader dataReader, TES3Material materialManager)
        {
            _dataReader = dataReader;
            _materialManager = materialManager;
            _instance = this;
        }

        private void EnsurePrefabContainerObjectExists()
        {
            if (_prefabContainerObj == null)
            {
                _prefabContainerObj = new GameObject("NIF Prefabs");
                _prefabContainerObj.SetActive(false);
            }
        }

        /// <summary>
        /// Instantiates a NIF file.
        /// </summary>
        public GameObject InstantiateNIF(string filePath, bool isStatic = true)
        {
            EnsurePrefabContainerObjectExists();

            // Get the prefab.
            GameObject prefab;
            if (!nifPrefabs.TryGetValue(filePath, out prefab))
            {
                // Load & cache the NIF prefab.
                prefab = LoadNifPrefabDontAddToPrefabCache(filePath, isStatic);
                nifPrefabs[filePath] = prefab;
            }

            // Instantiate the prefab.
            var nifInstance = GameObject.Instantiate(prefab);

            if (isStatic)
            {
                // TODO: We need to transform object position first.
                //nifInstance.AddComponent<Graphics.MeshMerger>();
                //nifInstance.AddComponent<LODGenerator>();
            }

            return nifInstance;
        }

        private GameObject LoadNifPrefabDontAddToPrefabCache(string filePath, bool isStatic)
        {
            var file = _dataReader.LoadNif(filePath);
            var objBuilder = new NIFObjectBuilder(file, _materialManager, isStatic);
            var prefab = objBuilder.BuildObject();
            prefab.transform.parent = _prefabContainerObj.transform;

            // Add LOD support to the prefab.
            var lodGroup = prefab.AddComponent<LODGroup>();
            lodGroup.SetLODs(new LOD[1]
            {
                new LOD(0.015f, prefab.GetComponentsInChildren<Renderer>())
            });

            return prefab;
        }
    }
}