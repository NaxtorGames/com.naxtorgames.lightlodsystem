using UnityEngine;

namespace NaxtorGames.LightLODSystem
{
    /// <summary>
    /// Provides every tick where it is and where it looks to any registered <see cref="LightLODSourceMono"/> in the <see cref="LightLODManager"/> to control wich settings will be applied by each <see cref="LightLODSourceMono"/>.
    /// </summary>
    [AddComponentMenu("NaxtorGames/Light LOD System/Light LOD Controller")]
    public sealed class LightLODControllerMono : MonoBehaviour
    {
        [Min(0.01f)]
        [SerializeField] private float _updateTick = 0.5f;
        [SerializeField] private bool _considerDirection = true;

        [SerializeField] private float _currentTick = 0.0f;

        /// <summary>
        /// How long one update tick is in seconds.
        /// </summary>
        public float UpdateTick
        {
            get => _updateTick;
            set => _updateTick = Mathf.Max(0.01f, value);
        }

        /// <summary>
        /// If the direction to the light source should be considered. (When the controller dose not look at the light source dose it need to be enabled?)
        /// </summary>
        public bool ConsiderDirection
        {
            get => _considerDirection;
            set => _considerDirection = value;
        }

        private void Update()
        {
            _currentTick += Time.unscaledDeltaTime;

            if (_currentTick > _updateTick)
            {
                LightLODManager.UpdateRegisteredLights(new LightLODPositionInfo(this.transform),_considerDirection);
                _currentTick -= _updateTick;
            }
        }
    }
}
