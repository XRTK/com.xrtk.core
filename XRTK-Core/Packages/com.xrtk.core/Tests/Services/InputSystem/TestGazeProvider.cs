// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Tests.Services.InputSystem
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