using System.Collections;

using TES3Unity.ESM.Records;
using TES3Unity.Rendering;

using UnityEngine;

namespace TES3Unity.Effects
{
	public sealed class Water : MonoBehaviour
	{
		[SerializeField] MeshRenderer m_Water;
		private Transform m_Camera = null;

		// Underwater (URP)
		private Material m_DefaultSkybox = null;
		private Color m_DefaultFogColor;
		private bool m_DefaultFog;
		private float m_DefaultFogDensity;
		private bool m_IsUnderwater = false;
		private bool m_UnderWaterEnabled = false;

		[SerializeField]
		private float underwaterLevel = 0.0f;
		[SerializeField]
		private Color ambientColor = Color.white;
		[SerializeField]
		private Color fogColor = new Color(0, 0.4f, 0.7f, 0.6f);
		[SerializeField]
		private float fogDensity = 0.04f;

		public float Level
		{
			get { return underwaterLevel; }
			set { underwaterLevel = value; }
		}

		private void Awake()
		{
			transform.position = Vector3.zero;
		}

		private IEnumerator Start()
		{
			if (m_Water == null)
			{
				m_Water = GetComponentInChildren<MeshRenderer>();
			}

			var camera = Camera.main;

			while (camera == null)
			{
				camera = Camera.main;
				yield return null;
			}

			m_Camera = camera.transform;

			yield return new WaitForEndOfFrame();

			var engine = TES3Engine.Inst;
			engine.CurrentCellChanged += OnCurrentCellChanged;
			OnCurrentCellChanged(engine.CurrentCell);
		}

		private void Update()
		{
			if ((!m_UnderWaterEnabled || m_Camera == null))
			{
				return;
			}

			if (m_Camera.position.y < underwaterLevel && !m_IsUnderwater)
			{
				UpdateUnderWater(true);
			}
			else if (m_Camera.position.y > underwaterLevel && m_IsUnderwater)
			{
				UpdateUnderWater(false);
			}
		}

		public void UpdateUnderWater(bool enabled, bool force = false)
		{
			if ((m_IsUnderwater == enabled && !force))
			{
				return;
			}

			m_IsUnderwater = enabled;

			if (enabled)
			{
				m_DefaultFog = RenderSettings.fog;
				m_DefaultFogColor = RenderSettings.fogColor;
				m_DefaultFogDensity = RenderSettings.fogDensity;
				m_DefaultSkybox = RenderSettings.skybox;
			}

			RenderSettings.fog = enabled;
			RenderSettings.fogColor = enabled ? fogColor : m_DefaultFogColor;
			RenderSettings.fogDensity = enabled ? fogDensity : m_DefaultFogDensity;
			RenderSettings.skybox = enabled ? null : m_DefaultSkybox;
		}

		private void OnCurrentCellChanged(CELLRecord cell)
		{
			if (cell == null)
			{
				return;
			}

			var isInterior = cell.isInterior;

			if (isInterior)
			{
				var whgt = cell.WHGT;
				var offset = 1.6f; // Interiors cells needs this offset to render at the correct location.
				var waterHeight = whgt == null ? -4000 : whgt.value;
				m_Water.transform.position = new Vector3(0, (waterHeight / Convert.MeterInMWUnits) - offset, 0);
				m_Water.enabled = whgt != null;
				m_Water.enabled = false;// FIXME
			}
			else
			{
				m_Water.enabled = true;
				m_Water.transform.position = Vector3.zero;
			}

			m_UnderWaterEnabled = m_Water.enabled;
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			if (m_Water == null)
			{
				m_Water = GetComponent<MeshRenderer>();
			}
		}
#endif
	}
}