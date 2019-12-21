// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Diagnostics;
using XRTK.Interfaces.Diagnostics;

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
        public string ApplicationSignature => $"{Application.productName} v{Application.version}";

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

            CreateDiagnosticsGameObject();
            CreateVisualizer();
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

        private void CreateDiagnosticsGameObject()
        {
            GameObject diagnosticGameObject = new GameObject("Diagnostics");
            DiagnosticsTransform = diagnosticGameObject.transform;
            DiagnosticsTransform.parent = MixedRealityToolkit.CameraSystem?.CameraRig.PlayspaceTransform;
        }

        private void CreateVisualizer()
        {
            if (MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.VisualizationPrefab == null)
            {
                Debug.LogError($"Failed to create a diagnostics visuailzer for {GetType().Name}. Check if a visualizer prefab is assigned in the configuration profile.");
                return;
            }

            Object.Instantiate(MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.VisualizationPrefab, MixedRealityToolkit.DiagnosticsSystem.DiagnosticsTransform);
        }
    }
}
