﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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

            if (!Application.isPlaying || !MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile.EnableDiagnostics)
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
        }
    }
}
