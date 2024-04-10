using System.Collections.Generic;
using UnityEngine;

namespace NaxtorGames.LightLODSystem
{
    /// <summary>
    /// The global <see cref="LightLODSourceMono"/> manager that keeps track of all registered <see cref="LightLODSourceMono"/> and updates all of them when <see cref="UpdateRegisteredLights(LightLODPositionInfo, bool)"/> is called.
    /// </summary>
    public static class LightLODManager
    {
        private static readonly List<LightLODSourceMono> s_registeredLightSources = new List<LightLODSourceMono>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            s_registeredLightSources.Clear();
        }

        /// <summary>
        /// Register a <see cref="LightLODSourceMono"/> that should be updated when any <see cref="LightLODControllerMono"/> ticks.
        /// </summary>
        /// <param name="lightLODSource">The light source</param>
        public static void Register(LightLODSourceMono lightLODSource)
        {
            if (s_registeredLightSources.Contains(lightLODSource))
            {
                return;
            }

            s_registeredLightSources.Add(lightLODSource);
        }

        /// <summary>
        /// Unregister a <see cref="LightLODSourceMono"/> from the tick list.
        /// </summary>
        /// <param name="lightLODSource"></param>
        public static void Unregister(LightLODSourceMono lightLODSource)
        {
            if (s_registeredLightSources.Contains(lightLODSource))
            {
                s_registeredLightSources.Remove(lightLODSource);
            }
        }

        /// <summary> Updates all registered <see cref="LightLODSourceMono"/>.
        /// <para>Called by any <see cref="LightLODControllerMono"/> every tick.</para>
        /// </summary>
        /// <param name="positionInfo"></param>
        /// <param name="considerDirection"></param>
        public static void UpdateRegisteredLights(LightLODPositionInfo positionInfo, bool considerDirection)
        {
            for (int i = 0; i < s_registeredLightSources.Count;)
            {
                LightLODSourceMono lightLODSource = s_registeredLightSources[i];
                if (lightLODSource == null)
                {
                    LightLODManager.Unregister(lightLODSource);
                    continue;
                }

                lightLODSource.ApplySettings(positionInfo, considerDirection);

                i++;
            }
        }
    }
}
