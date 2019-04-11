using UnityEditor;

namespace XRTK.Inspectors.Utilities.Packages
{
    [CustomEditor(typeof(MixedRealityPackageSettings))]
    public class MixedRealityPackageSettingsInspector : Editor
    {
        private SerializedProperty mixedRealityPackages;

        private void OnEnable()
        {
            mixedRealityPackages = serializedObject.FindProperty("mixedRealityPackages");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(mixedRealityPackages, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
