using UnityEditor;
using XRTK.Definitions.Controllers;

namespace XRTK.Inspectors.Profiles.InputSystem.Controllers
{
    [CustomEditor(typeof(MixedRealityInteractionMappingProfile))]
    public class MixedRealityInteractionMappingProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty interactionMapping;
        private SerializedProperty pointerProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            interactionMapping = serializedObject.FindProperty(nameof(interactionMapping));
            pointerProfile = serializedObject.FindProperty(nameof(pointerProfile));
        }

        public override void OnInspectorGUI()
        {
            RenderHeader();

            serializedObject.Update();

            EditorGUILayout.PropertyField(interactionMapping, true);
            EditorGUILayout.PropertyField(pointerProfile);

            serializedObject.ApplyModifiedProperties();
        }
    }
}