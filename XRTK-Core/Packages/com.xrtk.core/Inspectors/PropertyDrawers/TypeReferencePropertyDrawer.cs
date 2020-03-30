// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using Assembly = System.Reflection.Assembly;

namespace XRTK.Inspectors.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="SystemType"/> properties.
    /// </summary>
    [CustomPropertyDrawer(typeof(SystemType))]
    [CustomPropertyDrawer(typeof(SystemTypeAttribute), true)]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        private const string None = "(None)";
        private const string Missing = " {missing}";
        private const string TypeReferenceUpdated = "TypeReferenceUpdated";

        private static int selectionControlId;
        private static string selectedReference;
        private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();
        private static readonly int ControlHint = typeof(TypeReferencePropertyDrawer).GetHashCode();
        private static readonly GUIContent TempContent = new GUIContent();
        private static readonly GUIContent RepairContent = new GUIContent("Repair", "Try to repair the reference");
        private static readonly Color EnabledColor = Color.white;
        private static readonly Color DisabledColor = Color.Lerp(Color.white, Color.clear, 0.5f);

        #region Type Filtering

        /// <summary>
        /// Gets or sets a function that returns a collection of types that are
        /// to be excluded from drop-down. A value of <c>null</c> specifies that
        /// no types are to be excluded.
        /// </summary>
        /// <remarks>
        /// <para>This property must be set immediately before presenting a class
        /// type reference property field using <see cref="EditorGUI.PropertyField(Rect,SerializedProperty)"/>
        /// or <see cref="EditorGUILayout.PropertyField(SerializedProperty,UnityEngine.GUILayoutOption[])"/> since the value of this
        /// property is reset to <c>null</c> each time the control is drawn.</para>
        /// <para>Since filtering makes extensive use of <see cref="ICollection{Type}.Contains"/>
        /// it is recommended to use a collection that is optimized for fast
        /// look ups such as <see cref="HashSet{Type}"/> for better performance.</para>
        /// </remarks>
        /// <example>
        /// <para>Exclude a specific type from being selected:</para>
        /// <code language="csharp"><![CDATA[
        /// private SerializedProperty someTypeReferenceProperty;
        /// 
        /// public override void OnInspectorGUI()
        /// {
        ///     serializedObject.Update();
        /// 
        ///     ClassTypeReferencePropertyDrawer.ExcludedTypeCollectionGetter = GetExcludedTypeCollection;
        ///     EditorGUILayout.PropertyField(someTypeReferenceProperty);
        /// 
        ///     serializedObject.ApplyModifiedProperties();
        /// }
        /// 
        /// private ICollection<Type> GetExcludedTypeCollection()
        /// {
        ///     var set = new HashSet<Type>();
        ///     set.Add(typeof(SpecialClassToHideInDropdown));
        ///     return set;
        /// }
        /// ]]></code>
        /// </example>
        public static Func<ICollection<Type>> ExcludedTypeCollectionGetter { get; set; }

        /// <summary>
        /// Gets or sets a function that returns if the constraint has been satisfied.
        /// </summary>
        /// <remarks>
        /// <para>This will override any attribute filter already set.</para>
        /// <para>This property must be set immediately before presenting a class
        /// type reference property field using <see cref="EditorGUI.PropertyField(Rect,SerializedProperty)"/>
        /// or <see cref="EditorGUILayout.PropertyField(SerializedProperty,UnityEngine.GUILayoutOption[])"/> since the value of this
        /// property is reset to <c>null</c> each time the control is drawn.</para>
        /// </remarks>
        /// <example>
        /// <para>Override the constraint for the property:</para>
        /// <code language="csharp"><![CDATA[
        /// private SerializedProperty someTypeReferenceProperty;
        /// 
        /// public override void OnInspectorGUI()
        /// {
        ///     serializedObject.Update();
        /// 
        ///     ClassTypeReferencePropertyDrawer.FilterConstraintOverride = IsConstraintSatisfied;
        ///     EditorGUILayout.PropertyField(someTypeReferenceProperty);
        /// 
        ///     serializedObject.ApplyModifiedProperties();
        /// }
        /// 
        /// private bool IsConstraintSatisfied(Type type)
        /// {
        ///     return !type.IsAbstract && type.GetInterfaces().Any(interfaceType => interfaceType == ServiceConstraint);
        /// }
        /// ]]></code>
        /// </example>
        public static Func<Type, bool> FilterConstraintOverride { get; set; }

        private static IEnumerable<Type> GetFilteredTypes(SystemTypeAttribute filter)
        {
            var types = new List<Type>();
            var assemblies = CompilationPipeline.GetAssemblies();
            var excludedTypes = ExcludedTypeCollectionGetter?.Invoke();

            foreach (var assembly in assemblies)
            {
                var compiledAssembly = Assembly.Load(assembly.name);
                FilterTypes(compiledAssembly, filter, excludedTypes, types);
            }

            types.Sort((a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));
            return types;
        }

        private static void FilterTypes(Assembly assembly, SystemTypeAttribute filter, ICollection<Type> excludedTypes, List<Type> output)
        {
            output.AddRange(from type in assembly.GetTypes()
                            let isValid = (type.IsValueType && !type.IsEnum) || type.IsClass
                            where type.IsVisible && isValid
                            where (FilterConstraintOverride == null || FilterConstraintOverride.Invoke(type)) &&
                                  (filter == null || filter.IsConstraintSatisfied(type))
                            where excludedTypes == null || !excludedTypes.Contains(type)
                            select type);
        }

        #endregion Type Filtering

        #region Type Utility

        private static Type ResolveType(string classRef)
        {
            if (!TypeMap.TryGetValue(classRef, out var type))
            {
                type = !string.IsNullOrEmpty(classRef) ? Type.GetType(classRef) : null;
                TypeMap[classRef] = type;
            }

            return type;
        }

        #endregion Type Utility

        #region Control Drawing / Event Handling

        /// <summary>
        /// Draws the selection control for the type.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="label"></param>
        /// <param name="classRef"></param>
        /// <param name="filter"></param>
        /// <returns>True, if the class reference was successfully resolved.</returns>
        private static void DrawTypeSelectionControl(Rect position, GUIContent label, ref string classRef, SystemTypeAttribute filter)
        {
            if (label != null && label != GUIContent.none)
            {
                position = EditorGUI.PrefixLabel(position, label);
            }

            var triggerDropDown = false;
            var controlId = GUIUtility.GetControlID(ControlHint, FocusType.Keyboard, position);

            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.ExecuteCommand:
                    if (Event.current.commandName == TypeReferenceUpdated &&
                        selectionControlId == controlId)
                    {
                        if (classRef != selectedReference)
                        {
                            classRef = selectedReference;
                            GUI.changed = true;
                        }

                        selectionControlId = 0;
                        selectedReference = null;
                    }

                    break;

                case EventType.MouseDown:
                    if (GUI.enabled && position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.keyboardControl = controlId;
                        triggerDropDown = true;
                        Event.current.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (GUI.enabled && GUIUtility.keyboardControl == controlId)
                    {
                        if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space)
                        {
                            triggerDropDown = true;
                            Event.current.Use();
                        }
                    }

                    break;

                case EventType.Repaint:
                    // Remove assembly name and namespace from content of popup control.
                    var classRefParts = classRef.Split(',');
                    var className = classRefParts[0].Trim();
                    className = className.Substring(className.LastIndexOf(".", StringComparison.Ordinal) + 1);
                    TempContent.text = className;

                    if (TempContent.text == string.Empty)
                    {
                        TempContent.text = None;
                    }

                    EditorStyles.popup.Draw(position, TempContent, controlId);
                    break;
            }

            if (triggerDropDown)
            {
                selectionControlId = controlId;
                selectedReference = classRef;

                DisplayDropDown(position, GetFilteredTypes(filter), ResolveType(classRef), filter?.Grouping ?? TypeGrouping.ByNamespaceFlat);
            }
        }

        /// <summary>
        /// Draws the selection control for the type.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="systemTypeProperty"></param>
        /// <param name="label"></param>
        /// <param name="filter"></param>
        /// <returns>True, if the class reference was resolved successfully.</returns>
        private static void DrawTypeSelectionControl(Rect position, SerializedProperty systemTypeProperty, GUIContent label, SystemTypeAttribute filter)
        {
            try
            {
                var referenceProperty = systemTypeProperty.FindPropertyRelative("reference");

                EditorGUI.showMixedValue = referenceProperty.hasMultipleDifferentValues;

                var restoreColor = GUI.color;
                var reference = referenceProperty.stringValue;
                var restoreShowMixedValue = EditorGUI.showMixedValue;
                var isValidClassRef = string.IsNullOrEmpty(reference) || ResolveType(reference) != null;

                if (!isValidClassRef)
                {
                    isValidClassRef = TypeSearch(referenceProperty, ref reference, filter, false);

                    if (isValidClassRef)
                    {
                        Debug.LogWarning($"Fixed missing class reference for property '{label.text}' on {systemTypeProperty.serializedObject.targetObject.name}");
                    }
                    else
                    {
                        if (!reference.Contains(Missing))
                        {
                            reference += Missing;
                        }
                    }
                }

                if (isValidClassRef)
                {
                    GUI.color = EnabledColor;
                    DrawTypeSelectionControl(position, label, ref reference, filter);
                }
                else
                {
                    if (SystemTypeRepairWindow.WindowOpen)
                    {
                        GUI.color = DisabledColor;
                        DrawTypeSelectionControl(position, label, ref reference, filter);
                    }
                    else
                    {
                        const float leftPadding = 8f;
                        const float iconSize = 24f;
                        const float buttonWidth = 40f;
                        var errorContent = EditorGUIUtility.IconContent("d_console.erroricon.sml");
                        GUI.Label(new Rect(EditorGUIUtility.currentViewWidth - iconSize - leftPadding, position.y, iconSize, iconSize), errorContent);

                        var dropdownPosition = new Rect(position.x, position.y, position.width - buttonWidth - 28f, position.height);
                        var buttonPosition = new Rect(EditorGUIUtility.currentViewWidth - buttonWidth - leftPadding - iconSize, position.y, buttonWidth, position.height);

                        DrawTypeSelectionControl(dropdownPosition, label, ref reference, filter);

                        if (GUI.Button(buttonPosition, RepairContent, EditorStyles.miniButton))
                        {
                            TypeSearch(referenceProperty, ref reference, filter, true);
                        }
                    }
                }

                GUI.color = restoreColor;
                referenceProperty.stringValue = reference;
                referenceProperty.serializedObject.ApplyModifiedProperties();
                EditorGUI.showMixedValue = restoreShowMixedValue;
            }
            finally
            {
                FilterConstraintOverride = null;
                ExcludedTypeCollectionGetter = null;
            }
        }

        private static bool TypeSearch(SerializedProperty property, ref string typeName, SystemTypeAttribute filter, bool showPickerWindow)
        {
            if (typeName.Contains(Missing)) { return false; }

            var typeNameWithoutAssembly = typeName.Split(new[] { "," }, StringSplitOptions.None)[0];
            var typeNameWithoutNamespace = System.Text.RegularExpressions.Regex.Replace(typeNameWithoutAssembly, @"[.\w]+\.(\w+)", "$1");
            var repairedTypeOptions = FindTypesByName(typeNameWithoutNamespace, filter);

            switch (repairedTypeOptions.Length)
            {
                case 0:
                    if (showPickerWindow)
                    {
                        EditorApplication.delayCall += () =>
                            EditorUtility.DisplayDialog(
                                "No types found",
                                $"No types with the name '{typeNameWithoutNamespace}' were found.",
                                "OK");
                    }

                    return false;
                case 1:
                    typeName = SystemType.GetReference(repairedTypeOptions[0]);
                    return true;
                default:
                    if (showPickerWindow)
                    {
                        EditorApplication.delayCall += () =>
                            SystemTypeRepairWindow.Display(repairedTypeOptions, property);
                    }

                    return false;
            }
        }

        private static Type[] FindTypesByName(string typeName, SystemTypeAttribute filter)
        {
            return GetFilteredTypes(filter).Where(type => type.Name.Equals(typeName)).ToArray();
        }

        private static void DisplayDropDown(Rect position, IEnumerable<Type> types, Type selectedType, TypeGrouping grouping)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(None), selectedType == null, OnSelectedTypeName, null);
            menu.AddSeparator(string.Empty);

            foreach (var type in types)
            {
                var menuLabel = FormatGroupedTypeName(type, grouping);

                if (string.IsNullOrEmpty(menuLabel)) { continue; }

                var content = new GUIContent(menuLabel);
                menu.AddItem(content, type == selectedType, OnSelectedTypeName, type);
            }

            menu.DropDown(position);
        }

        private static string FormatGroupedTypeName(Type type, TypeGrouping grouping)
        {
            var name = type.FullName;

            switch (grouping)
            {
                case TypeGrouping.None:
                    return name;
                case TypeGrouping.ByNamespace:
                    return string.IsNullOrEmpty(name) ? string.Empty : name.Replace('.', '/');
                case TypeGrouping.ByNamespaceFlat:
                    int lastPeriodIndex = string.IsNullOrEmpty(name) ? -1 : name.LastIndexOf('.');
                    if (lastPeriodIndex != -1)
                    {
                        name = string.IsNullOrEmpty(name)
                            ? string.Empty
                            : $"{name.Substring(0, lastPeriodIndex)}/{name.Substring(lastPeriodIndex + 1)}";
                    }

                    return name;
                case TypeGrouping.ByAddComponentMenu:
                    var addComponentMenuAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
                    if (addComponentMenuAttributes.Length == 1)
                    {
                        return ((AddComponentMenu)addComponentMenuAttributes[0]).componentMenu;
                    }

                    Debug.Assert(type.FullName != null);
                    return $"Scripts/{type.FullName?.Replace('.', '/')}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(grouping), grouping, null);
            }
        }

        private static void OnSelectedTypeName(object userData)
        {
            selectedReference = SystemType.GetReference(userData as Type);
            var typeReferenceUpdatedEvent = EditorGUIUtility.CommandEvent(TypeReferenceUpdated);
            EditorWindow.focusedWindow.SendEvent(typeReferenceUpdatedEvent);
        }

        #endregion Control Drawing / Event Handling

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawTypeSelectionControl(position, property, label, attribute as SystemTypeAttribute);
        }
    }
}