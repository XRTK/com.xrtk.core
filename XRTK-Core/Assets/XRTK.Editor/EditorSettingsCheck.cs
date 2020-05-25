// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace XRTK.Editor
{
    [InitializeOnLoad]
    public static class EditorSettingsCheck
    {
        static EditorSettingsCheck()
        {
            if (EditorPrefs.GetInt("unity_project_generation_flag", 3) <= 3)
            {
                EditorPrefs.SetInt("unity_project_generation_flag", 3);
            }
        }
    }
}
