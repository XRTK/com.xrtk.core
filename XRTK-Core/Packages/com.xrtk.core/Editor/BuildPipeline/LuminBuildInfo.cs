// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using UnityEditor.Build.Reporting;
using UnityEngine;
using XRTK.Extensions;
using Debug = UnityEngine.Debug;

namespace XRTK.Editor.BuildPipeline
{
    public class LuminBuildInfo : BuildInfo
    {
        public override async void OnPostprocessBuild(BuildReport buildReport)
        {
            if (!Application.isBatchMode ||
                buildReport.summary.result != BuildResult.Succeeded)
            {
                return;
            }

            // TODO Check if installation flag is set in build window

            Debug.Log("Starting installation...");

            var canInstall = false;

            try
            {
                var deviceResult = await new Process().RunAsync("mldb devices", false);

                if (deviceResult.ExitCode == 0)
                {
                    foreach (var deviceId in deviceResult.Output)
                    {
                        if (!string.IsNullOrWhiteSpace(deviceId))
                        {
                            Debug.Log(deviceId);

                            if (!deviceId.Contains("List"))
                            {
                                canInstall = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (!canInstall)
            {
                Debug.Log("No devices found, skipping installation");
                return;
            }

            try
            {
                await new Process().RunAsync($"mldb install -u \"{OutputDirectory}\"", true);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}