using System.Collections;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components.Records
{
    public class TESLight : RecordComponent
    {
        [System.Serializable]
        public class LightData
        {
            public UnityEngine.Light lightComponent;
            public enum LightFlags
            {
                Dynamic = 0x0001,
                CanCarry = 0x0002,
                Negative = 0x0004,
                Flicker = 0x0008,
                Fire = 0x0010,
                OffDefault = 0x0020,
                FlickerSlow = 0x0040,
                Pulse = 0x0080,
                PulseSlow = 0x0100
            }

            public int flags;
        }

        public LightData lightData = null;

        private void Start()
        {
            LIGHRecord lightRecord = (LIGHRecord)record;

            lightData = new LightData();
            lightData.lightComponent = gameObject.GetComponentInChildren<UnityEngine.Light>(true);

            if (lightRecord.Name != null)
            {
                objData.name = lightRecord.Name;
            }

            objData.interactionPrefix = "Take ";

            TryAddScript(lightRecord.Script);

            if (lightRecord.Data.HasValue)
            {
                var data = lightRecord.Data.Value;
                lightData.flags = data.Flags;

                if (Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.CanCarry))
                {
                    if (GameSettings.Get().KinematicRigidbody)
                    {
                        gameObject.AddComponent<Rigidbody>().isKinematic = true;
                        gameObject.AddComponent<BoxCollider>().size *= 0.5f;
                    }
                }

                StartCoroutine(ConfigureLightComponent());
            }
        }

        public IEnumerator ConfigureLightComponent()
        {
            var time = 0f;
            //wait until we have found the light component. this will typically be the frame /after/ object creation as the light component is added after this component is created
            while (lightData.lightComponent == null && time < 5f)
            {
                lightData.lightComponent = gameObject.GetComponentInChildren<UnityEngine.Light>(true);
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }

            if (lightData.lightComponent != null) //if we have found the light component by the end of the loop
            {
                // Only disable the light based on flags if the light component hasn't already been disabled due to settings.
                if (lightData.lightComponent.enabled)
                {
                    lightData.lightComponent.enabled = !Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.OffDefault);
                }

                var flicker = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Flicker);
                var flickerSlow = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.FlickerSlow);
                var pulse = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Pulse);
                var pulseSlow = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.PulseSlow);
                var fire = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Fire);
                var animated = flicker || flickerSlow || pulse || pulseSlow || fire;

                if (animated && GameSettings.Get().AnimateLights)
                {
                    var lightAnim = lightData.lightComponent.gameObject.AddComponent<LightAnim>();
                    if (flicker)
                    {
                        lightAnim.Mode = LightAnimMode.Flicker;
                    }

                    if (flickerSlow)
                    {
                        lightAnim.Mode = LightAnimMode.FlickerSlow;
                    }

                    if (pulse)
                    {
                        lightAnim.Mode = LightAnimMode.Pulse;
                    }

                    if (pulseSlow)
                    {
                        lightAnim.Mode = LightAnimMode.PulseSlow;
                    }

                    if (fire)
                    {
                        lightAnim.Mode = LightAnimMode.Fire;
                    }
                }
            }
            else
            {
                Debug.Log("Light Record Object Created Without Light Component. Search Timed Out.");
            }
        }
    }
}