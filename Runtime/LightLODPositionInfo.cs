using UnityEngine;

namespace NaxtorGames.LightLODSystem
{
    public readonly struct LightLODPositionInfo
    {
        public readonly Vector3 Position;
        public readonly Vector3 Forward;

        public LightLODPositionInfo(Vector3 position, Vector3 forward)
        {
            this.Position = position;
            this.Forward = forward;
        }

        public LightLODPositionInfo(Transform transform)
        {
            this.Position = transform.position;
            this.Forward = transform.forward;
        }
    }
}