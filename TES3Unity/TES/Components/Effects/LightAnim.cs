using UnityEngine;

namespace TES3Unity
{
    public enum LightAnimMode
    {
        None = 0,
        Flicker,
        FlickerSlow,
        Pulse,
        PulseSlow,
        Fire
    }

    public class LightAnim : MonoBehaviour
    {
        private Light _light;
        private float _baseIntensity = 1f;

        public LightAnimMode Mode { get; set; } = LightAnimMode.None;

        private void Start()
        {
            _light = GetComponent<Light>();
            _baseIntensity = _light.intensity;
        }

        private void Update()
        {
            var value = 1.0f;

            switch (Mode)
            {
                case LightAnimMode.None:
                    break;
                case LightAnimMode.Flicker:
                    value = Mathf.Round(Mathf.Clamp01(Random.value + 0.1f));
                    break;
                case LightAnimMode.FlickerSlow:
                    value = Mathf.Round(Mathf.Clamp01(Random.value + 0.47f));
                    break;
                case LightAnimMode.Pulse:
                    value = Mathf.Sin(Time.time) * 0.5f + 0.5f;
                    break;
                case LightAnimMode.PulseSlow:
                    value = Mathf.Sin(Time.time * 0.5f) * 0.5f + 0.5f;
                    break;
                case LightAnimMode.Fire:
                    value = Mathf.PerlinNoise(Time.time * 0.8f, transform.position.x + transform.position.z * 7.9253618f);
                    value = 1f - value;
                    value = value * value * value;
                    value = 1f - value;
                    break;
            }

            _light.intensity = Mathf.Lerp(0.2f * _baseIntensity, _baseIntensity, value);
        }
    }
}