// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.DiagnosticsSystem;
using XRTK.Interfaces.DiagnosticsSystem;

namespace XRTK.Services.DiagnosticsSystem
{
    /// <summary>
    /// The default implementation of the <see cref="IMixedRealityDiagnosticsSystem"/>
    /// </summary>
    public class MixedRealityDiagnosticsSystem : BaseSystem, IMixedRealityDiagnosticsSystem
    {
        /// <inheritdoc />
        public Transform DiagnosticsTransform { get; private set; }

        /// <inheritdoc />
        public GameObject DiagnosticsWindow { get; private set; }

        /// <inheritdoc />
        public string ApplicationSignature => $"{Application.productName} v{Application.version}";

        private bool showWindow = false;
        /// <inheritdoc />
        public bool ShowWindow
        {
            get { return showWindow; }
            set
            {
                if (value && DiagnosticsWindow == null)
                {
                    CreateDiagnosticsWindow();
                }

                showWindow = value;
                DiagnosticsWindow.SetActive(showWindow);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">Diagnostics system configuration profile.</param>
        public MixedRealityDiagnosticsSystem(MixedRealityDiagnosticsSystemProfile profile)
            : base(profile) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (!Application.isPlaying)
            {
                return;
            }

            foreach (var diagnosticsDataProvider in MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.RegisteredDiagnosticsDataProviders)
            {
                //If the DataProvider cannot be resolved, this is likely just a configuration / package missmatch.  User simply needs to be warned, not errored.
                if (diagnosticsDataProvider.DataProviderType.Type == null)
                {
                    Debug.LogWarning($"Could not load the configured provider ({diagnosticsDataProvider.DataProviderName})\n\nThis is most likely because the XRTK UPM package for that provider is currently not registered\nCheck the installed packages in the Unity Package Manager\n\n");
                    continue;
                }

                if (!MixedRealityToolkit.CreateAndRegisterService<IMixedRealityDiagnosticsDataProvider>(
                    diagnosticsDataProvider.DataProviderType,
                    diagnosticsDataProvider.RuntimePlatform,
                    diagnosticsDataProvider.DataProviderName,
                    diagnosticsDataProvider.Priority,
                    diagnosticsDataProvider.Profile))
                {
                    Debug.LogError($"Failed to start {diagnosticsDataProvider.DataProviderName}!");
                }
            }

            CreateDiagnosticsRootGameObject();
            ShowWindow = MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.ShowDiagnosticsWindowOnStart;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (DiagnosticsTransform != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(DiagnosticsTransform.gameObject);
                }
                else
                {
                    DiagnosticsTransform.DetachChildren();
                    Object.Destroy(DiagnosticsTransform.gameObject);
                }

                DiagnosticsTransform = null;
            }
        }

        private void CreateDiagnosticsRootGameObject()
        {
            GameObject diagnosticGameObject = new GameObject("Diagnostics");
            DiagnosticsTransform = diagnosticGameObject.transform;
            DiagnosticsTransform.parent = MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform;
        }

        private void CreateDiagnosticsWindow()
        {
            if (MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.DiagnosticsWindowPrefab == null)
            {
                Debug.LogError($"Failed to create a diagnostics visuailzer for {GetType().Name}. Check if a visualizer prefab is assigned in the configuration profile.");
                return;
            }

            DiagnosticsWindow = Object.Instantiate(MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.DiagnosticsWindowPrefab, MixedRealityToolkit.DiagnosticsSystem.DiagnosticsTransform);
        }
    }
}
