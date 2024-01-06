using UnityEngine;

namespace TES3Unity.Diagnostic
{
	public class ModelLoader : MonoBehaviour
	{
		[SerializeField]
		private string m_ModelToLoad = null;

		void OnEnable()
		{
			var tes = GetComponent<TES3Loader>();
			tes.Initialized += Tes_Initialized;
		}

		void Tes_Initialized(TES3Loader tes)
		{
			if (string.IsNullOrEmpty(m_ModelToLoad))
			{
				Debug.LogError("No model to load.");
				return;
			}

			var path = $"meshes\\{m_ModelToLoad.Replace("/", "\\")}";

			Debug.Log($"Loading model {path}");

			GameObject model = tes.NifManager.InstantiateNIF(path, false);

			var colliders = model.GetComponentsInChildren<Collider>();
			var maxY = 0.0f;

			foreach (var collider in colliders)
			{
				maxY = Mathf.Max(maxY, collider.bounds.size.y);
			}

			model.transform.Translate(0, maxY, 0);
		}
	}
}