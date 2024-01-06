using System;
using UnityEngine.Rendering;

namespace TES3Unity
{
    public static class VolumeProfileExtensions
    {
        public static void DisableEffect<T>(this VolumeProfile volume) where T : VolumeComponent
        {
            if (volume.TryGet<T>(out T component))
            {
                component.active = false;
            }
        }

        public static void UpdateEffect<T>(this VolumeProfile volume, Action<T> callback) where T : VolumeComponent
        {
            if (volume.TryGet<T>(out T component))
            {
                callback?.Invoke(component);
            }
        }
    }
}
