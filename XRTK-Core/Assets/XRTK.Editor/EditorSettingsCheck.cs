// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor
{
    [InitializeOnLoad]
    internal static class EditorSettingsCheck
    {
        [Flags]
        private enum ProjectGenerationFlag
        {
            None = 0,
            Embedded = 1,
            Local = 2,
            Registry = 4,
            Git = 8,
            BuiltIn = 16,
            Unknown = 32,
            PlayerAssemblies = 64,
            LocalTarBall = 128,
        }

        static EditorSettingsCheck()
        {
            if (Application.isBatchMode ||
                !EditorPrefs.HasKey("unity_project_generation_flag"))
            {
                return;
            }

            var current = (ProjectGenerationFlag)EditorPrefs.GetInt("unity_project_generation_flag");

            if ((current & ProjectGenerationFlag.Local) != ProjectGenerationFlag.Local ||
                (current & ProjectGenerationFlag.Embedded) != ProjectGenerationFlag.Embedded)
            {
                current |= ProjectGenerationFlag.Embedded | ProjectGenerationFlag.Local;
                EditorPrefs.SetInt("unity_project_generation_flag", (int)current);

                EditorApplication.delayCall += () => ToggleProjectGeneration((int)current);
            }
        }

        private static void ToggleProjectGeneration(int value)
        {
            var syncVsType = Type.GetType("UnityEditor.SyncVS,UnityEditor");
            Debug.Assert(syncVsType != null);
            var synchronizerField = syncVsType.GetField("Synchronizer", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(synchronizerField != null);
            var synchronizerValue = synchronizerField.GetValue(synchronizerField);
            Debug.Assert(synchronizerValue != null);

            var solutionSynchronizerType = Type.GetType("UnityEditor.VisualStudioIntegration.SolutionSynchronizer,UnityEditor");
            Debug.Assert(solutionSynchronizerType != null);

            object assemblyNameProviderValue = null;

            foreach (var propertyInfo in solutionSynchronizerType.GetRuntimeProperties())
            {
                if (propertyInfo.Name == "AssemblyNameProvider")
                {
                    assemblyNameProviderValue = propertyInfo.GetValue(synchronizerValue);
                    break;
                }
            }

            Debug.Assert(assemblyNameProviderValue != null);

            var assemblyNameProviderType = Type.GetType("UnityEditor.VisualStudioIntegration.AssemblyNameProvider,UnityEditor");
            Debug.Assert(assemblyNameProviderType != null);
            var toggleProjectGenerationMethod = assemblyNameProviderType.GetMethod("ToggleProjectGeneration");
            Debug.Assert(toggleProjectGenerationMethod != null);
            toggleProjectGenerationMethod.Invoke(assemblyNameProviderValue, new object[] { value });

            var syncSolutionMethod = syncVsType.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
            Debug.Assert(syncSolutionMethod != null);
            syncSolutionMethod.Invoke(null, null);
        }
    }
}
