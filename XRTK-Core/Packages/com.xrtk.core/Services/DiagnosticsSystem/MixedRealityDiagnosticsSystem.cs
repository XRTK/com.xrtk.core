// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using XRTK.Definitions.DiagnosticsSystem;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.DiagnosticsSystem;
using XRTK.Interfaces.DiagnosticsSystem;
using XRTK.Interfaces.DiagnosticsSystem.Handlers;
using Object = UnityEngine.Object;

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
            memoryEventData = new MemoryEventData(currentEventSystem);
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (!Application.isPlaying)
            {
                return;
            }

            if (profile.ShowDiagnosticsWindowOnStart == AutoStartBehavior.AutoStart)
            {
                IsWindowEnabled = true;
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (!Application.isPlaying)
            {
                return;
            }

            if (diagnosticsWindow != null)
            {
                Unregister(diagnosticsWindow);

                if (Application.isEditor)
                {
                    Object.DestroyImmediate(diagnosticsWindow);
                }
                else
                {
                    Object.Destroy(diagnosticsWindow);
                }
            }
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
                    Register(diagnosticsWindow);
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

        #region Console Events

        /// <inheritdoc />
        public void RaiseLogReceived(string message, string stackTrace, LogType logType)
        {
            consoleEventData.Initialize(message, stackTrace, logType);
            HandleEvent(consoleEventData, OnLogReceived);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityConsoleDiagnosticsHandler> OnLogReceived =
            delegate (IMixedRealityConsoleDiagnosticsHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<ConsoleEventData>(eventData);
                handler.OnLogReceived(casted);
            };

        #endregion Console Events

        #region Frame Events

        /// <inheritdoc />
        public void RaiseMissedFramesChanged(bool[] missedFrames)
        {
            frameEventData.Initialize(missedFrames);
            HandleEvent(frameEventData, OnFrameMissed);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFrameDiagnosticsHandler> OnFrameMissed =
            delegate (IMixedRealityFrameDiagnosticsHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FrameEventData>(eventData);
                handler.OnFrameMissed(casted);
            };

        /// <inheritdoc />
        public void RaiseFrameRateChanged(int frameRate, bool isGPU)
        {
            frameEventData.Initialize(frameRate, isGPU);
            HandleEvent(frameEventData, OnFrameRateChanged);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFrameDiagnosticsHandler> OnFrameRateChanged =
            delegate (IMixedRealityFrameDiagnosticsHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<FrameEventData>(eventData);
                handler.OnFrameRateChanged(casted);
            };

        #endregion Frame Events

        #region Memory Events

        /// <inheritdoc />
        public void RaiseMemoryLimitChanged(MemoryLimit currentMemoryLimit)
        {
            memoryEventData.Initialize(currentMemoryLimit);
            HandleEvent(memoryEventData, OnMemoryLimitChanged);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityMemoryDiagnosticsHandler> OnMemoryLimitChanged =
            delegate (IMixedRealityMemoryDiagnosticsHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MemoryEventData>(eventData);
                handler.OnMemoryLimitChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseMemoryUsageChanged(MemoryUsage currentMemoryUsage)
        {
            memoryEventData.Initialize(currentMemoryUsage);
            HandleEvent(memoryEventData, OnMemoryUsageChanged);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityMemoryDiagnosticsHandler> OnMemoryUsageChanged =
            delegate (IMixedRealityMemoryDiagnosticsHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MemoryEventData>(eventData);
                handler.OnMemoryUsageChanged(casted);
            };

        /// <inheritdoc />
        public void RaiseMemoryPeakChanged(MemoryPeak peakMemoryUsage)
        {
            memoryEventData.Initialize(peakMemoryUsage);
            HandleEvent(memoryEventData, OnMemoryPeakChanged);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityMemoryDiagnosticsHandler> OnMemoryPeakChanged =
            delegate (IMixedRealityMemoryDiagnosticsHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<MemoryEventData>(eventData);
                handler.OnMemoryPeakChanged(casted);
            };

        #endregion Memory Events

        #endregion IMixedRealityDiagnosticsSystem Implementation
    }
}