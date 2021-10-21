// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    /// <summary>
    /// This component checks which XR SDK pipeline is being used by the project
    /// and adds platform define symbols as needed.
    /// </summary>
    public class XRPipelineMonitor
    {
        /// <summary>
        /// Available XR pipeline types.
        /// </summary>
        public enum XRPipeline
        {
            /// <summary>
            /// The legacy VR pipeline available up until Unity 2019.4 LTS.
            /// Deprecated and no more available as off 2020.1.
            /// </summary>
            LegacyVR = 0,
            /// <summary>
            /// The new XR SDK Plugin management. As off 2020.1 the only viable
            /// option.
            /// </summary>
            XRSDK
        }

        private static ListRequest listRequest;
        private static double timeoutDuration = 10f;
        private static DateTimeOffset requestStartTime;

        private const string legacyVRScriptingDefineSymbol = "XRTK_USE_LEGACYVR";
        private const string xrSDKPackageId = "com.unity.xr.management";

        /// <summary>
        /// The detected pipeline in the project.
        /// </summary>
        public static XRPipeline DetectedPipeline { get; private set; }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
#if UNITY_2020_1_OR_NEWER
            DetectedPipeline = XRPipeline.XRSDK;
            UpdateProjectScriptingDefineSymbols();
#else
            requestStartTime = DateTimeOffset.UtcNow;
            listRequest = Client.List(true);
            EditorApplication.update += EditorApplication_Update;
#endif
        }

        private static void EditorApplication_Update()
        {
            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                {
                    DetectedPipeline = XRPipeline.LegacyVR;
                    foreach (var package in listRequest.Result)
                    {
                        // Either the XR SDK package is a first party citizen
                        // of the project...
                        if (package.packageId.Equals(xrSDKPackageId))
                        {
                            // Package found. We are using XR SDK Management.
                            DetectedPipeline = XRPipeline.XRSDK;
                            break;
                        }
                        // or it it's only here because some other package needs it...
                        else if (package.resolvedDependencies != null)
                        {
                            for (var i = 0; i < package.resolvedDependencies.Length; i++)
                            {
                                if (package.resolvedDependencies[i].name.Equals(xrSDKPackageId))
                                {
                                    // Package found. We are using XR SDK Management.
                                    DetectedPipeline = XRPipeline.XRSDK;
                                    break;
                                }
                            }
                        }
                    }

                    UpdateProjectScriptingDefineSymbols();
                }
                else
                {
                    Debug.LogError("Failed to determine XR pipeline in use.");
                }

                EditorApplication.update -= EditorApplication_Update;
            }
            else if ((DateTimeOffset.UtcNow - requestStartTime).TotalSeconds > timeoutDuration)
            {
                Debug.LogError("Failed to determine XR pipeline in use.");
                EditorApplication.update -= EditorApplication_Update;
            }
        }

        private static void UpdateProjectScriptingDefineSymbols()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (!string.IsNullOrWhiteSpace(scriptingDefineSymbols))
            {
                var updated = new StringBuilder();
                var alreadyAdded = false;
                var hadToRemove = false;

                var splits = scriptingDefineSymbols.Split(';');
                for (var i = 0; i < splits.Length; i++)
                {
                    var split = splits[i];
                    if (split.Equals(legacyVRScriptingDefineSymbol))
                    {
                        if (DetectedPipeline == XRPipeline.LegacyVR)
                        {
                            alreadyAdded = true;
                        }
                        else
                        {
                            hadToRemove = true;
                            continue;
                        }
                    }

                    updated.Append(i == splits.Length - 1 ? split : $"{split};");
                }

                if (!alreadyAdded && DetectedPipeline == XRPipeline.LegacyVR)
                {
                    updated.Append($";{legacyVRScriptingDefineSymbol}");
                }

                // If we didn't have to anything to the symbols,
                // then we can go get some coffee already.
                if (alreadyAdded && !hadToRemove)
                {
                    return;
                }

                // Update symbols otherwise and then get coffee.
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, updated.ToString());
            }
            else if (DetectedPipeline == XRPipeline.LegacyVR)
            {
                // There was no symbols at all defined yet, just add the new one.
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, legacyVRScriptingDefineSymbol);
            }
        }
    }
}
