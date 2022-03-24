// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XRTK.Editor.Extensions;
using XRTK.Utilities.Lines.DataProviders;
using XRTK.Utilities.Lines.Renderers;
using XRTK.Utilities.Physics.Distorters;

namespace XRTK.Editor.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(BaseMixedRealityLineDataProvider))]
    public class BaseMixedRealityLineDataProviderInspector : UnityEditor.Editor
    {
        private const string DrawLinePointsKey = "XRTK_Line_Inspector_DrawLinePoints";
        private const string DrawLineRotationsKey = "XRTK_Line_Inspector_DrawLineRotations";
        private const string EditorSettingsFoldoutKey = "XRTK_Line_Inspector_EditorSettings";
        private const string RotationArrowLengthKey = "XRTK_Line_Inspector_RotationArrowLength";
        private const string ManualUpVectorLengthKey = "XRTK_Line_Inspector_ManualUpVectorLength";
        private const string LinePreviewResolutionKey = "XRTK_Line_Inspector_LinePreviewResolution";
        private const string DrawLineManualUpVectorsKey = "XRTK_Line_Inspector_DrawLineManualUpVectors";

        private const float ManualUpVectorHandleSizeModifier = 0.1f;

        private static readonly GUIContent BasicSettingsContent = new GUIContent("Basic Settings");
        private static readonly GUIContent EditorSettingsContent = new GUIContent("Editor Settings");
        private static readonly GUIContent ManualUpVectorContent = new GUIContent("Manual Up Vectors");
        private static readonly GUIContent RotationSettingsContent = new GUIContent("Rotation Settings");
        private static readonly GUIContent DistortionSettingsContent = new GUIContent("Distortion Settings");

        private static bool editorSettingsFoldout = false;
        protected static int LinePreviewResolution = 16;

        protected static bool DrawLinePoints = false;
        protected static bool DrawLineRotations = false;
        protected static bool DrawLineManualUpVectors = false;

        protected static float ManualUpVectorLength = 1f;
        protected static float RotationArrowLength = 0.5f;

        private SerializedProperty transformMode;
        private SerializedProperty customLineTransform;
        private SerializedProperty lineStartClamp;
        private SerializedProperty lineEndClamp;
        private SerializedProperty loops;
        private SerializedProperty rotationMode;
        private SerializedProperty flipUpVector;
        private SerializedProperty originOffset;
        private SerializedProperty manualUpVectorBlend;
        private SerializedProperty manualUpVectors;
        private SerializedProperty velocitySearchRange;
        private SerializedProperty distorters;
        private SerializedProperty distortionMode;
        private SerializedProperty distortionStrength;
        private SerializedProperty uniformDistortionStrength;

        private ReorderableList manualUpVectorList;

        protected BaseMixedRealityLineDataProvider LineData;
        protected bool RenderLinePreview = true;

        protected virtual void OnEnable()
        {
            editorSettingsFoldout = SessionState.GetBool(EditorSettingsFoldoutKey, editorSettingsFoldout);
            LinePreviewResolution = SessionState.GetInt(LinePreviewResolutionKey, LinePreviewResolution);
            DrawLinePoints = SessionState.GetBool(DrawLinePointsKey, DrawLinePoints);
            DrawLineRotations = SessionState.GetBool(DrawLineRotationsKey, DrawLineRotations);
            RotationArrowLength = SessionState.GetFloat(RotationArrowLengthKey, RotationArrowLength);
            DrawLineManualUpVectors = SessionState.GetBool(DrawLineManualUpVectorsKey, DrawLineManualUpVectors);
            ManualUpVectorLength = SessionState.GetFloat(ManualUpVectorLengthKey, ManualUpVectorLength);

            LineData = (BaseMixedRealityLineDataProvider)target;
            transformMode = serializedObject.FindProperty(nameof(transformMode));
            customLineTransform = serializedObject.FindProperty(nameof(customLineTransform));
            lineStartClamp = serializedObject.FindProperty(nameof(lineStartClamp));
            lineEndClamp = serializedObject.FindProperty(nameof(lineEndClamp));
            loops = serializedObject.FindProperty(nameof(loops));
            rotationMode = serializedObject.FindProperty(nameof(rotationMode));
            flipUpVector = serializedObject.FindProperty(nameof(flipUpVector));
            originOffset = serializedObject.FindProperty(nameof(originOffset));
            manualUpVectorBlend = serializedObject.FindProperty(nameof(manualUpVectorBlend));
            manualUpVectors = serializedObject.FindProperty(nameof(manualUpVectors));
            velocitySearchRange = serializedObject.FindProperty(nameof(velocitySearchRange));
            distorters = serializedObject.FindProperty(nameof(distorters));
            distortionMode = serializedObject.FindProperty(nameof(distortionMode));
            distortionStrength = serializedObject.FindProperty(nameof(distortionStrength));
            uniformDistortionStrength = serializedObject.FindProperty(nameof(uniformDistortionStrength));

            manualUpVectorList = new ReorderableList(serializedObject, manualUpVectors, false, true, true, true);
            manualUpVectorList.drawElementCallback += DrawManualUpVectorListElement;
            manualUpVectorList.drawHeaderCallback += DrawManualUpVectorHeader;

            RenderLinePreview = LineData.gameObject.GetComponent<BaseMixedRealityLineRenderer>() == null;

            var newDistorters = LineData.gameObject.GetComponents<Distorter>();
            distorters.arraySize = newDistorters.Length;

            for (int i = 0; i < newDistorters.Length; i++)
            {
                var distorterProperty = distorters.GetArrayElementAtIndex(i);
                distorterProperty.objectReferenceValue = newDistorters[i];
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            editorSettingsFoldout = EditorGUILayoutExtensions.FoldoutWithBoldLabel(editorSettingsFoldout, EditorSettingsContent);
            SessionState.SetBool(EditorSettingsFoldoutKey, editorSettingsFoldout);

            if (editorSettingsFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();

                GUI.enabled = RenderLinePreview;
                EditorGUI.BeginChangeCheck();
                LinePreviewResolution = EditorGUILayout.IntSlider("Preview Resolution", LinePreviewResolution, 2, 128);

                if (EditorGUI.EndChangeCheck())
                {
                    SessionState.SetInt(LinePreviewResolutionKey, LinePreviewResolution);
                }

                EditorGUI.BeginChangeCheck();
                DrawLinePoints = EditorGUILayout.Toggle("Draw Line Points", DrawLinePoints);

                if (EditorGUI.EndChangeCheck())
                {
                    SessionState.SetBool(DrawLinePointsKey, DrawLinePoints);
                }

                GUI.enabled = true;
                EditorGUI.BeginChangeCheck();
                DrawLineRotations = EditorGUILayout.Toggle("Draw Line Rotations", DrawLineRotations);

                if (EditorGUI.EndChangeCheck())
                {
                    SessionState.SetBool(DrawLineRotationsKey, DrawLineRotations);
                }

                if (DrawLineRotations)
                {
                    EditorGUI.BeginChangeCheck();
                    RotationArrowLength = EditorGUILayout.Slider("Rotation Arrow Length", RotationArrowLength, 0.01f, 5f);

                    if (EditorGUI.EndChangeCheck())
                    {
                        SessionState.SetFloat(RotationArrowLengthKey, RotationArrowLength);
                    }
                }

                EditorGUI.BeginChangeCheck();
                DrawLineManualUpVectors = EditorGUILayout.Toggle("Draw Manual Up Vectors", DrawLineManualUpVectors);

                if (EditorGUI.EndChangeCheck())
                {
                    SessionState.SetBool(DrawLineManualUpVectorsKey, DrawLineManualUpVectors);
                }

                if (DrawLineManualUpVectors)
                {
                    EditorGUI.BeginChangeCheck();
                    ManualUpVectorLength = EditorGUILayout.Slider("Manual Up Vector Length", ManualUpVectorLength, 1f, 10f);

                    if (EditorGUI.EndChangeCheck())
                    {
                        SessionState.SetFloat(ManualUpVectorLengthKey, ManualUpVectorLength);
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    SceneView.RepaintAll();
                }

                EditorGUI.indentLevel--;
            }

            if (transformMode.FoldoutWithBoldLabelPropertyField(BasicSettingsContent))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(customLineTransform);
                EditorGUILayout.PropertyField(lineStartClamp);
                EditorGUILayout.PropertyField(lineEndClamp);
                EditorGUILayout.PropertyField(loops);

                EditorGUI.indentLevel--;
            }

            if (rotationMode.FoldoutWithBoldLabelPropertyField(RotationSettingsContent))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(rotationMode);
                EditorGUILayout.PropertyField(flipUpVector);
                EditorGUILayout.PropertyField(originOffset);
                EditorGUILayout.PropertyField(velocitySearchRange);

                if (DrawLineManualUpVectors)
                {
                    manualUpVectorList.DoLayoutList();

                    if (GUILayout.Button("Normalize Up Vectors"))
                    {

                        for (int i = 0; i < manualUpVectors.arraySize; i++)
                        {
                            var manualUpVectorProperty = manualUpVectors.GetArrayElementAtIndex(i);

                            Vector3 upVector = manualUpVectorProperty.vector3Value;

                            if (upVector == Vector3.zero)
                            {
                                upVector = Vector3.up;
                            }

                            manualUpVectorProperty.vector3Value = upVector.normalized;
                        }
                    }

                    EditorGUILayout.PropertyField(manualUpVectorBlend);
                }

                EditorGUI.indentLevel--;
            }

            if (distortionMode.FoldoutWithBoldLabelPropertyField(DistortionSettingsContent))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(distortionMode);
                EditorGUILayout.PropertyField(distortionStrength);
                EditorGUILayout.PropertyField(uniformDistortionStrength);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            if (DrawLineManualUpVectors)
            {
                if (LineData.ManualUpVectors == null || LineData.ManualUpVectors.Length < 2)
                {
                    LineData.ManualUpVectors = new[] { Vector3.up, Vector3.up };
                }

                for (int i = 0; i < LineData.ManualUpVectors.Length; i++)
                {
                    float normalizedLength = (1f / (LineData.ManualUpVectors.Length - 1)) * i;
                    var position = LineData.GetPoint(normalizedLength);
                    float handleSize = HandleUtility.GetHandleSize(position);
                    LineData.ManualUpVectors[i] = MixedRealityInspectorUtility.VectorHandle(LineData, position, LineData.ManualUpVectors[i], false, true, ManualUpVectorLength * handleSize, handleSize * ManualUpVectorHandleSizeModifier);
                }
            }

            if (Application.isPlaying)
            {
                Handles.EndGUI();
                return;
            }

            Vector3 firstPosition = LineData.FirstPoint;
            Vector3 lastPosition = firstPosition;

            for (int i = 1; i < LinePreviewResolution; i++)
            {
                Vector3 currentPosition;
                Quaternion rotation;

                if (i == LinePreviewResolution - 1)
                {
                    currentPosition = LineData.LastPoint;
                    rotation = LineData.GetRotation(LineData.PointCount - 1);
                }
                else
                {
                    float normalizedLength = (1f / (LinePreviewResolution - 1)) * i;
                    currentPosition = LineData.GetPoint(normalizedLength);
                    rotation = LineData.GetRotation(normalizedLength);
                }

                if (RenderLinePreview)
                {
                    Handles.color = Color.magenta;
                    Handles.DrawLine(lastPosition, currentPosition);
                }

                if (DrawLineRotations)
                {
                    float arrowSize = HandleUtility.GetHandleSize(currentPosition) * RotationArrowLength;
                    Handles.color = MixedRealityInspectorUtility.LineVelocityColor;
                    Handles.color = Color.Lerp(MixedRealityInspectorUtility.LineVelocityColor, Handles.zAxisColor, 0.75f);
                    Handles.ArrowHandleCap(0, currentPosition, Quaternion.LookRotation(rotation * Vector3.forward), arrowSize, EventType.Repaint);
                    Handles.color = Color.Lerp(MixedRealityInspectorUtility.LineVelocityColor, Handles.xAxisColor, 0.75f);
                    Handles.ArrowHandleCap(0, currentPosition, Quaternion.LookRotation(rotation * Vector3.right), arrowSize, EventType.Repaint);
                    Handles.color = Color.Lerp(MixedRealityInspectorUtility.LineVelocityColor, Handles.yAxisColor, 0.75f);
                    Handles.ArrowHandleCap(0, currentPosition, Quaternion.LookRotation(rotation * Vector3.up), arrowSize, EventType.Repaint);
                }

                lastPosition = currentPosition;
            }

            if (LineData.Loops && RenderLinePreview)
            {
                Handles.color = Color.magenta;
                Handles.DrawLine(lastPosition, firstPosition);
            }
        }

        private static void DrawManualUpVectorHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, ManualUpVectorContent);
        }

        private void DrawManualUpVectorListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.BeginChangeCheck();

            var property = manualUpVectors.GetArrayElementAtIndex(index);
            property.vector3Value = EditorGUI.Vector3Field(rect, GUIContent.none, property.vector3Value);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
