// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.DiagnosticsSystem;
using XRTK.EventDatum.DiagnosticsSystem;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// The default implementation of the <see cref="IMixedRealityDiagnosticsSystem"/>
    /// </summary>
    public class MixedRealityDiagnosticsSystem : BaseEventSystem, IMixedRealityDiagnosticsSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">Diagnostics system configuration profile.</param>
        public MixedRealityDiagnosticsSystem(MixedRealityDiagnosticsSystemProfile profile)
            : base(profile)
        {
            this.profile = profile;
        }

        private readonly MixedRealityDiagnosticsSystemProfile profile;

        private FrameEventData frameEventData;
        private MemoryEventData memoryEventData;
        private ConsoleEventData consoleEventData;
        private MissedFrameEventData missedFrameEventData;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (!Application.isPlaying)
            {
                return;
            }

            var currentEventSystem = EventSystem.current;
            frameEventData = new FrameEventData(currentEventSystem);
            consoleEventData = new ConsoleEventData(currentEventSystem);
            missedFrameEventData = new MissedFrameEventData(currentEventSystem);
            memoryEventData = new MemoryEventData(currentEventSystem);
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (diagnosticsRoot != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(diagnosticsRoot.gameObject);
                }
                else
                {
                    Object.Destroy(diagnosticsRoot.gameObject);
                }
            }
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealityDiagnosticsSystem Implementation

        private Transform diagnosticsRoot = null;

        /// <inheritdoc />
        public Transform DiagnosticsRoot
        {
            get
            {
                if (diagnosticsRoot == null)
                {
                    diagnosticsRoot = new GameObject("Diagnostics").transform;
                    diagnosticsRoot.parent = MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform;
                }

                return diagnosticsRoot;
            }
        }

        private GameObject diagnosticsWindow = null;

        /// <inheritdoc />
        public GameObject DiagnosticsWindow
        {
            get
            {
                if (diagnosticsWindow == null)
                {
                    diagnosticsWindow = Object.Instantiate(profile.DiagnosticsWindowPrefab, DiagnosticsRoot);
                }

                return diagnosticsWindow;
            }
        }

        /// <inheritdoc />
        public string ApplicationSignature => $"{Application.productName} v{Application.version}";

        private bool isWindowEnabled = false;

        /// <inheritdoc />
        public bool IsWindowEnabled
        {
            get => DiagnosticsWindow.activeInHierarchy && isWindowEnabled;
            set
            {
                if (isWindowEnabled == value) { return; }

                isWindowEnabled = value;
                DiagnosticsWindow.SetActive(isWindowEnabled);
            }
        }

        /// <inheritdoc />
        public void RaiseMissedFramesChanged(bool[] missedFrames)
        {
            missedFrameEventData.Initialize(missedFrames);
        }

        /// <inheritdoc />
        public void RaiseFrameRateChanged(int frameRate, bool isGPU)
        {
            frameEventData.Initialize(frameRate, isGPU);
        }

        /// <inheritdoc />
        public void RaiseLogReceived(string message, string stackTrace, LogType logType)
        {
            consoleEventData.Initialize(message, stackTrace, logType);
        }

        /// <inheritdoc />
        public void RaiseMemoryLimitChanged(MemoryLimit currentMemoryLimit)
        {
            memoryEventData.Initialize(currentMemoryLimit);
        }

        /// <inheritdoc />
        public void RaiseMemoryUsageChanged(MemoryUsage currentMemoryUsage)
        {
            memoryEventData.Initialize(currentMemoryUsage);
        }

        /// <inheritdoc />
        public void RaiseMemoryPeakChanged(MemoryPeak peakMemoryUsage)
        {
            memoryEventData.Initialize(peakMemoryUsage);
        }

        #endregion IMixedRealityDiagnosticsSystem Implementation
    }
}