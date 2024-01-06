using System;

using TES3Unity.ESM.Records;

using UnityEngine;
using UnityEngine.Rendering;

namespace TES3Unity.Components
{
	public class WeatherManager : MonoBehaviour
	{
		private const float TweetyFourHoursInSeconds = 86400.0f;
		private const float TwelveHoursInSeconds = 43200.0f;

		private Light m_Sun = null;
		private Transform m_SunTransform = null;
		private Color m_DefaultFogColor;
		[SerializeField] Color m_DefaultSunLightColor = Color.white;
		[SerializeField] Color32 m_DefaultAmbientColor = new Color32(137, 140, 160, 255);
		private float m_DefaultFogDensity;
		private bool m_DayNightCycle;

		[Range(0.0f, TweetyFourHoursInSeconds)]
		[SerializeField]
		private float m_TimeInSeconds = 34000;
		[SerializeField]
		private float m_TimeScale = 1.0f;

		public TimeSpan GameTime
		{
			get => TimeSpan.FromSeconds(m_TimeInSeconds);
			set => m_TimeInSeconds = (float)value.TotalSeconds;
		}

		public int Hours => GameTime.Hours;
		public int Minutes => GameTime.Minutes;
		public int Seconds => GameTime.Seconds;
		public int TotalSeconds => GameTime.Seconds;

		public float TimeScaleMultiplier { get; set; } = 1.0f;

		private void Start()
		{
			m_DefaultFogColor = RenderSettings.fogColor;
			m_DefaultFogDensity = RenderSettings.fogDensity;

			var config = GameSettings.Get();
			m_DayNightCycle = config.DayNightCycle;

			var engine = TES3Engine.Inst;
			engine.CurrentCellChanged += OnCurrentCellChanged;
			OnCurrentCellChanged(engine.CurrentCell);
		}

		public void SetSun(Light light)
		{
			m_Sun = light;
			m_Sun.shadows = GameSettings.GetRecommandedShadows(false);
			m_SunTransform = light.transform;
			RenderSettings.sun = light;

			if (!GameSettings.Get().DayNightCycle)
			{
				return;
			}

			// The Moon Light is a subtile directional light at the opposite of the sun light
			// It's used to light a bit the scene when the sun light is in night mode.
			var moonLightGo = new GameObject("MoonLight");
			moonLightGo.transform.parent = m_SunTransform;
			moonLightGo.transform.localPosition = Vector3.zero;
			moonLightGo.transform.localRotation = Quaternion.Euler(180.0f, 0.0f, 0.0f);

			var moonLight = moonLightGo.AddComponent<Light>();
			moonLight.type = LightType.Directional;
			moonLight.intensity = 0.32f;
			moonLight.color = new Color32(136, 163, 255, 255);
			moonLight.shadows = GameSettings.GetRecommandedShadows(true);
		}

		private void Update()
		{
			m_TimeInSeconds += Time.deltaTime * m_TimeScale * TimeScaleMultiplier;

			if (m_TimeInSeconds >= TweetyFourHoursInSeconds)
			{
				m_TimeInSeconds = 0.0f;
			}

			if (m_Sun == null || !m_DayNightCycle)
			{
				return;
			}

			var position = Quaternion.Euler((m_TimeInSeconds / TweetyFourHoursInSeconds) * 360.0f, 0, 0) * new Vector3(0.0f, -300.0f, 0.0f);
			m_SunTransform.position = position;
			m_SunTransform.rotation = Quaternion.LookRotation(-position);

			m_Sun.intensity = 1.25f - Mathf.Abs(m_TimeInSeconds / TwelveHoursInSeconds - 1.0f);
			m_Sun.color = new Color(
				 1.0f,
				 Mathf.Min(m_Sun.intensity + 0.05f, 1.0f),
				 Mathf.Min(m_Sun.intensity, 1.0f)
			);
		}

		private void OnCurrentCellChanged(CELLRecord cell)
		{
			if (cell == null)
			{
				return;
			}

			var ambientColor = m_DefaultAmbientColor;
			var sunColor = m_DefaultSunLightColor;
			var ambientData = cell.AMBI;
			var fogColor = m_DefaultFogColor;
			var fogDensity = m_DefaultFogDensity;
			var fog = !cell.isInterior;
			var rain = false;

			if (ambientData != null)
			{
				ambientColor = ColorUtils.B8G8R8ToColor32(ambientData.ambientColor);
				sunColor = ColorUtils.B8G8R8ToColor32(ambientData.sunlightColor);
				fogColor = ColorUtils.B8G8R8ToColor32(ambientData.fogColor);
				fogDensity = ambientData.fogDensity;
			}

			if (!cell.isInterior)
			{
				if (TES3Engine.Inst.DataReader.TryGetRegion(cell.RGNN.value, out var region))
				{
					fog = region.Data.Foggy > 0;
					rain = region.Data.Rain > 0;
				}
			}

			RenderSettings.fog = fog;
			RenderSettings.fogColor = fogColor;
			RenderSettings.fogDensity = fogDensity;
			RenderSettings.ambientLight = ambientColor;
			RenderSettings.ambientMode = cell.isInterior ? AmbientMode.Flat : AmbientMode.Skybox;
			RenderSettings.ambientIntensity = TES3Engine.Inst.ambientIntensity;

			if (rain)
			{
				Debug.Log($"It's raining!");
			}

			if (m_Sun != null)
			{
				m_Sun.enabled = !cell.isInterior;
				m_Sun.color = sunColor;
			}
		}
	}
}
