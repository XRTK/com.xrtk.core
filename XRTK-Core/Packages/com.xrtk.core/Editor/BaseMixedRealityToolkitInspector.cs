using UnityEditor;
using UnityEngine;
using XRTK.Editor.Utilities;

namespace XRTK.Editor.Profiles
{
    /// <summary>
    /// Base class for all editor inspectors of the Mixed Reality Toolkit to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityToolkitInspector : UnityEditor.Editor
    {
        /// <summary>
        /// Render the Mixed Reality Toolkit Logo.
        /// </summary>
        protected static void RenderMixedRealityToolkitLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(EditorGUIUtility.isProSkin ? MixedRealityInspectorUtility.LightThemeLogo : MixedRealityInspectorUtility.DarkThemeLogo, GUILayout.MaxHeight(128f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
        }
    }
}