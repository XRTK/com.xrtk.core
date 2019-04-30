using UnityEditor;
using UnityEngine;
using XRTK.Utilities.Lines;

namespace XRTK.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(BezierDataProvider))]
    public class BezierDataProviderInspector : BaseMixedRealityLineDataProviderInspector
    {
        private const float HANDLE_SIZE_MODIFIER = 0.04f;
        private const float PICK_SIZE_MODIFIER = 0.06f;

        private SerializedProperty controlPoints;
        private SerializedProperty inertia;
        private SerializedProperty useLocalTangentPoints;

        private int selectedHandleIndex = -1;

        protected override void OnEnable()
        {
            base.OnEnable();

            controlPoints = serializedObject.FindProperty("controlPoints");
            inertia = serializedObject.FindProperty("inertia");
            useLocalTangentPoints = serializedObject.FindProperty("useLocalTangentPoints");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            // We always draw line points for bezier.
            DrawLinePoints = true;

            EditorGUILayout.PropertyField(controlPoints, true);
            EditorGUILayout.PropertyField(inertia, true);
            EditorGUILayout.PropertyField(useLocalTangentPoints);

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();

            for (int i = 0; i < 4; i++)
            {
                serializedObject.Update();

                bool isTangentHandle = i % 3 != 0;
                bool isLastPoint = i == 3;

                var controlPointPosition = LineData.GetPoint(i);
                var controlPointProperty = controlPoints.FindPropertyRelative("point" + (i + 1));

                // Draw our tangent lines
                Handles.color = Color.gray;
                if (i == 0)
                {
                    Handles.DrawLine(LineData.GetPoint(0), LineData.GetPoint(1));
                }
                else if (!isTangentHandle)
                {
                    Handles.DrawLine(LineData.GetPoint(i), LineData.GetPoint(i - 1));

                    if (!isLastPoint)
                    {
                        Handles.DrawLine(LineData.GetPoint(i), LineData.GetPoint(i + 1));
                    }
                }

                Handles.color = isTangentHandle ? Color.white : Color.green;
                float handleSize = HandleUtility.GetHandleSize(controlPointPosition);

                if (Handles.Button(controlPointPosition, Quaternion.identity, handleSize * HANDLE_SIZE_MODIFIER, handleSize * PICK_SIZE_MODIFIER, Handles.DotHandleCap))
                {
                    selectedHandleIndex = i;
                }

                // Draw our handles
                if (Tools.current == Tool.Move && selectedHandleIndex == i)
                {
                    EditorGUI.BeginChangeCheck();

                    var newTargetPosition = Handles.PositionHandle(controlPointPosition, Quaternion.identity);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(LineData, "Change Bezier Point Position");
                        LineData.SetPoint(i, newTargetPosition);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}