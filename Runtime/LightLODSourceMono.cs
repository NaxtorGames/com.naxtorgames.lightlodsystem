using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace NaxtorGames.LightLODSystem
{
    /// <summary>
    /// The Light LOD Source Component wich controlls the <see cref="UnityEngine.Light"/> depending on its set <see cref="Settings"/>.
    /// </summary>
    [AddComponentMenu("NaxtorGames/Light LOD System/Light LOD Source")]
    [RequireComponent(typeof(Light))]
    public sealed class LightLODSourceMono : MonoBehaviour
    {
        [SerializeField] private Light _light = null;
        [SerializeField] private LightLODSettingsSO _settings = null;

        [SerializeField] private bool _rangeIsThreshold = true;
        [SerializeField] private float _dotThreshold = 0.0f;

#if UNITY_EDITOR
        [Space(10.0f)]
        [SerializeField] private float DEBUG_directionDot = 0.0f;
#endif

        private int _lastSettingsIndex = -1;
        private bool _lookedInDirecion = false;

        /// <summary>
        /// The controlled <see cref="UnityEngine.Light"/> component
        /// </summary>
        public Light Light
        {
            get => _light;
            set
            {
                _light = value;
                CanPerform();
            }
        }

        /// <summary>
        /// The scriptable object for all configuration settings.
        /// </summary>
        public LightLODSettingsSO Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                CanPerform();
            }
        }

        /// <summary>
        /// If <see cref="DotThreshold"/> is overriden by the <see cref="Light"/> components <see cref="Light.range"/> property.
        /// </summary>
        public bool RangeIsThreshold
        {
            get => _rangeIsThreshold;
            set => _rangeIsThreshold = value;
        }

        /// <summary>
        /// The threshold on how far the look direction is considered to be visible.
        /// </summary>
        public float DotThreshold
        {
            get => RangeIsThreshold ? Light.range : _dotThreshold;
            set => _dotThreshold = value;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _light = this.gameObject.GetComponent<Light>();
        }
#endif
        private void OnEnable()
        {
            if (!CanPerform())
            {
                return;
            }

            LightLODManager.Register(this);
        }

        private void OnDisable()
        {
            LightLODManager.Unregister(this);
        }
        
        /// <summary>
        /// Apply the assigned <see cref="Settings"/> to the <see cref="Light"/> component.
        /// </summary>
        /// <param name="lODPositionInfo">The origins position and forward direction.</param>
        /// <param name="considerDirection">If the origins direction should control if the <see cref="Light"/> component should be disabled even if its in range but dose not look at the light.</param>
        public void ApplySettings(LightLODPositionInfo lODPositionInfo, bool considerDirection)
        {
            if (_settings.LODSettings.Length == 0)
            {
                return;
            }

            int lastSettingIndex = _settings.LODSettings.Length - 1;
            LightLODSettings settingsToApply = _settings.LODSettings[lastSettingIndex];

            Vector3 direction = _light.transform.position - lODPositionInfo.Position;
            float distanceSqrd = direction.sqrMagnitude;
            int newSettingsIndex = -1;
            for (int i = lastSettingIndex; i > -1; i--)
            {
                LightLODSettings lODSettings = _settings.LODSettings[i];
                if (distanceSqrd < lODSettings.MinDistanceSqrd && i > 0)
                {
                    continue;
                }
                else
                {
                    settingsToApply = lODSettings;
                    newSettingsIndex = i;
                    break;
                }
            }

            bool looksInDirection = !considerDirection || LooksInDirection(new LightLODPositionInfo(_light.transform), lODPositionInfo);

            if (_lastSettingsIndex != newSettingsIndex || _lookedInDirecion != looksInDirection)
            {
                _lastSettingsIndex = newSettingsIndex;
                _lookedInDirecion = looksInDirection;
                settingsToApply.ApplyTo(_light, looksInDirection);
            }
        }

        public void Set(Light light, LightLODSettingsSO settingsSO, bool logErrors)
        {
            Set(light, settingsSO, false, logErrors);
        }

        internal void Set(Light light, LightLODSettingsSO settingsSO, bool skipCheck, bool logErrors)
        {
            this._light = light;
            this._settings = settingsSO;

            if (!skipCheck)
            {
                CanPerform(logErrors);
            }
        }

        private bool LooksInDirection(LightLODPositionInfo lightPositionInfo, LightLODPositionInfo targetPositionInfo)
        {
            Vector3 direction = targetPositionInfo.Position - lightPositionInfo.Position;

            float dot = Vector3.Dot(direction, targetPositionInfo.Forward);

#if UNITY_EDITOR
            DEBUG_directionDot = dot;
#endif
            return dot < DotThreshold;
        }

        private bool CanPerform(bool logWarnings = true)
        {
            bool hasLight = _light != null;
            bool hasSettings = _settings != null;
            bool hasCorrectLightMode = hasLight && _light.lightmapBakeType != LightmapBakeType.Baked;
            bool anyError = !hasLight || !hasSettings || !hasCorrectLightMode;

            if (anyError)
            {
                this.enabled = false;

#if UNITY_EDITOR
                if (logWarnings)
                {
                    Debug.LogWarning($"[Light LOD System] '{this.name}' is not properly set up and was disabled!", this);
                    if (!hasLight)
                    {
                        Debug.LogWarning("[Light LOD System] Light source is missing!", this);
                    }
                    if (!hasSettings)
                    {
                        Debug.LogWarning("[Light LOD System] Settings are missing!", this);
                    }
                    if (!hasCorrectLightMode)
                    {
                        Debug.LogWarning("[Light LOD System] Light Mode can not be 'Baked'!", this);
                    }
                }
#endif
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Create a runtime <see cref="LightLODSourceMono"/>.
        /// </summary>
        /// <param name="lightType">The type of light wich the <see cref="UnityEngine.Light"/> should be applied</param>
        /// <param name="settings">The settings scriptable object for the lod light</param>
        /// <param name="worldPosition">Where the lod light should be in world space</param>
        /// <param name="worldRotation">The world rotation of the lod light</param>
        /// <param name="lodLight">the reference to the created lod light</param>
        /// <returns>If the light source could be created.</returns>
        public static bool CreateLODLight(LightType lightType, LightLODSettingsSO settings, Vector3 worldPosition, Quaternion worldRotation, out LightLODSourceMono lodLight)
        {
            return LightLODSourceMono.CreateLODLight(lightType, settings, worldPosition, worldRotation, out lodLight, false);
        }

        /// <summary>
        /// Create a runtime <see cref="LightLODSourceMono"/>.
        /// </summary>
        /// <param name="lightType">The type of light wich the <see cref="UnityEngine.Light"/> should be applied</param>
        /// <param name="settings">The settings scriptable object for the lod light</param>
        /// <param name="worldPosition">Where the lod light should be in world space</param>
        /// <param name="worldRotation">The world rotation of the lod light</param>
        /// <param name="lodLight">the reference to the created lod light</param>
        /// <param name="logWarning">if warnings should be loged when any value is invalid</param>
        /// <returns>If the light source could be created.</returns>
        public static bool CreateLODLight(LightType lightType, LightLODSettingsSO settings, Vector3 worldPosition, Quaternion worldRotation, out LightLODSourceMono lodLight, bool logWarning = true)
        {
            if (LightLODSourceMono.CreateLODLight(lightType, settings, out lodLight, false, logWarning))
            {
                lodLight.transform.SetPositionAndRotation(worldPosition, worldRotation);
            }

            return lodLight != null;
        }

        /// <summary>
        /// Create a runtime <see cref="LightLODSourceMono"/>.
        /// </summary>
        /// <param name="lightType">The type of light wich the <see cref="UnityEngine.Light"/> should be applied</param>
        /// <param name="settings">The settings scriptable object for the lod light</param>
        /// <param name="lodLight">the reference to the created lod light</param>
        /// <param name="logWarning">if warnings should be loged when any value is invalid</param>
        /// <returns>If the light source could be created.</returns>
        public static bool CreateLODLight(LightType lightType, LightLODSettingsSO settings, out LightLODSourceMono lodLight, bool skipCheck = false, bool logWarning = true)
        {
            string name;
            switch (lightType)
            {
                case LightType.Spot:
                    name = "LOD_SLight";
                    break;
                case LightType.Directional:
                    name = "LOD_DLight";
                    break;
                case LightType.Point:
                    name = "LOD_PLight";
                    break;
                default:
                case LightType.Area:
                case LightType.Disc:
                    Debug.LogError("Not supported light type!");
                    lodLight = null;
                    return false;
            }

            GameObject lodLightGameObject = new GameObject(name);

            Light light = lodLightGameObject.AddComponent<Light>();
            light.type = lightType;

            if (lightType == LightType.Directional)
            {
                light.bounceIntensity = 1.0f;
            }
            else
            {
                light.bounceIntensity = 0.0f;
            }

            lodLight = lodLightGameObject.AddComponent<LightLODSourceMono>();
            lodLight.Set(light, settings, skipCheck, logWarning);
            return true;
        }
    }
}
