using UnityEngine;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Tests.InputSystem.TestAssets
{
    public class TestGazeProvider : MonoBehaviour, IMixedRealityGazeProvider
    {
        public bool Enabled { get; set; }
        public IMixedRealityInputSource GazeInputSource { get; } = null;
        public IMixedRealityPointer GazePointer { get; } = null;
        public IMixedRealityCursor GazeCursor { get; } = null;
        public GameObject GazeTarget { get; } = null;
        public RaycastHit HitInfo { get; } = default;
        public Vector3 HitPosition { get; } = Vector3.zero;
        public Vector3 HitNormal { get; } = Vector3.zero;
        public Vector3 GazeOrigin { get; } = Vector3.zero;
        public Vector3 GazeDirection { get; } = Vector3.zero;
        public Vector3 HeadVelocity { get; } = Vector3.zero;
        public Vector3 HeadMovementDirection { get; } = Vector3.zero;
        public GameObject GameObjectReference { get; } = null;
    }
}