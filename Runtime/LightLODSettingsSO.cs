using UnityEngine;

namespace NaxtorGames.LightLODSystem
{
    [CreateAssetMenu(fileName = "_LightLODSettings", menuName = "NaxtorGames/Light LOD System/Light LOD Settings")]
    public sealed class LightLODSettingsSO : ScriptableObject
    {
        [SerializeField] private LightLODSettings[] _lodSettings = new LightLODSettings[0];

        public LightLODSettings[] LODSettings => _lodSettings;
    }
}
