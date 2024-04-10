using UnityEngine;
using UnityEngine.Rendering;

namespace NaxtorGames.LightLODSystem
{
    [System.Serializable]
    public struct LightLODSettings
    {
        [Header("Distance")]

        [Tooltip("The minimum distance wich this setting will be applied.")]
        [Min(0.0f)]
        [SerializeField] private float _minDistance;

        [Header("Light Source Settings")]
        [Tooltip("If the light should be enabled at all.")]
        public bool IsEnabled;
        [Tooltip("If the light enabled state should always be enforced (The LightLODCamera disables the light when its not visible by the camera if disabled).")]
        public bool ForceIsEnabledState;
        [Tooltip("The Quality of the light (This settings is equal to the light render mode but named Force Pixel instead of Important and Force Vertex instead of Not Important).")]
        public LightRenderMode LightRenderMode;
        [Tooltip("If the shadows should be off, hard or soft (Soft shadows are only enabled if the quality preset allows so.)")]
        public LightShadows ShadowQuality;
        [Tooltip("The resolution of the shadow map")]
        public LightShadowResolution ShadowResolution;

        /// <summary>
        /// The minimum distance squared.
        /// </summary>
        public readonly float MinDistanceSqrd => _minDistance * _minDistance;

        public LightLODSettings(float minDistance = 0.0f,
            bool isEnabled = true, bool forceIsEnabled = false, LightRenderMode lightRenderMode = LightRenderMode.Auto,
            LightShadowResolution shadowResolution = LightShadowResolution.FromQualitySettings, LightShadows shadowQuality = LightShadows.Hard)
        {
            this._minDistance = minDistance;
            this.IsEnabled = isEnabled;
            this.ForceIsEnabledState = forceIsEnabled;
            this.LightRenderMode = lightRenderMode;
            this.ShadowResolution = shadowResolution;
            this.ShadowQuality = shadowQuality;
        }

        public readonly void ApplyTo(Light light)
        {
            ApplyTo_Internal(light, this.IsEnabled);
        }

        public readonly void ApplyTo(Light light, bool enabled)
        {
            bool isLightOn = ForceIsEnabledState ? this.IsEnabled : enabled;

            ApplyTo_Internal(light, isLightOn);
        }

        private readonly void ApplyTo_Internal(Light light, bool isOn)
        {
            if (isOn)
            {
                light.renderMode = this.LightRenderMode;
                light.shadowResolution = this.ShadowResolution;
                light.shadows = this.ShadowQuality;
            }

            light.enabled = isOn;
        }
    }
}