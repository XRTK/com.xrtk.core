using UnityEditor;
using UnityEngine;
using XRTK.Definitions;

namespace XRTK.Inspectors.Utilities
{
    public static class BaseMixedRealityProfileInspectorExtension
    {
        public static void CopySerializedValues(this BaseMixedRealityProfile target, BaseMixedRealityProfile source)
        {
            var serializedObject = new SerializedObject(target);
            Undo.RecordObject(target, "Paste Profile Values");
            var isEditable = serializedObject.FindProperty("isEditable").boolValue;
            var originalName = serializedObject.targetObject.name;
            EditorUtility.CopySerialized(source, serializedObject.targetObject);
            serializedObject.Update();
            serializedObject.FindProperty("isEditable").boolValue = isEditable;
            serializedObject.ApplyModifiedProperties();
            serializedObject.targetObject.name = originalName;
            Debug.Assert(serializedObject.FindProperty("isEditable").boolValue == isEditable);
            AssetDatabase.SaveAssets();
        }
    }
}