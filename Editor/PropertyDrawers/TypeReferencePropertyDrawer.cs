﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Utilities;
using XRTK.Editor.Utilities;
using XRTK.Extensions;
using Assembly = System.Reflection.Assembly;

namespace XRTK.Editor.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="SystemType"/> properties.
    /// </summary>
    [CustomPropertyDrawer(typeof(SystemType))]
    [CustomPropertyDrawer(typeof(SystemTypeAttribute), true)]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        public const string TypeReferenceUpdated = "TypeReferenceUpdated";

        private const string NONE = "(None)";

        private static int selectionControlId;
        private static readonly int ControlHint = typeof(TypeReferencePropertyDrawer).GetHashCode();
        private static readonly GUIContent TempContent = new GUIContent();

        /// <summary>
        /// The currently selected <see cref="Type"/> in the dropdown menu.
        /// </summary>
        public static Type SelectedType { get; private set; }

        /// <summary>
        /// Gets or sets a Type to be created using the "Create new ..." menu.
        /// </summary>
        /// <remarks>
        /// <para>This property must be set immediately before presenting a class
        /// type reference property field using <see cref="EditorGUI.PropertyField(Rect,SerializedProperty)"/>
        /// or <see cref="EditorGUILayout.PropertyField(SerializedProperty,UnityEngine.GUILayoutOption[])"/> since the value of this
        /// property is reset to <c>null</c> each time the control is drawn.</para>
        /// </remarks>
        public static Type CreateNewTypeOverride { get; set; }

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

        /// <summary>
        /// <para>This will override any grouping drawn for the drop-down.</para>
        /// <para>This property must be set immediately before presenting a class
        /// type reference property field using <see cref="EditorGUI.PropertyField(Rect,SerializedProperty)"/>
        /// or <see cref="EditorGUILayout.PropertyField(SerializedProperty,UnityEngine.GUILayoutOption[])"/> since the value of this
        /// property is reset to <c>null</c> each time the control is drawn.</para>
        /// </summary>
        public static TypeGrouping? GroupingOverride { get; set; }

        private static IEnumerable<Type> GetFilteredTypes(SystemTypeAttribute filter)
        {
            var types = new List<Type>();
            var assemblies = CompilationPipeline.GetAssemblies();
            var excludedTypes = ExcludedTypeCollectionGetter?.Invoke();

            foreach (var assembly in assemblies)
            {
                Assembly compiledAssembly;

                try
                {
                    compiledAssembly = Assembly.Load(assembly.name);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    continue;
                }

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

        #region Control Drawing / Event Handling

        /// <summary>
        /// Draws the selection control for the type.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="label"></param>
        /// <param name="referenceProperty"></param>
        /// <param name="selectedType"></param>
        /// <param name="filter"></param>
        /// <returns>True, if the class reference was successfully resolved.</returns>
        private static void DrawTypeSelectionControl(Rect position, GUIContent label, SerializedProperty referenceProperty, Type selectedType, SystemTypeAttribute filter)
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
                        if (selectedType != SelectedType)
                        {
                            selectedType = SelectedType;

                            if (selectedType == null)
                            {
                                referenceProperty.stringValue = string.Empty;
                            }
                            else
                            {
                                if (selectedType.GUID != Guid.Empty)
                                {
                                    referenceProperty.stringValue = selectedType.GUID.ToString();
                                }
                                else
                                {
                                    Debug.LogWarning($"{selectedType.Name} is missing {nameof(System.Runtime.InteropServices.GuidAttribute)}");
                                    referenceProperty.stringValue = selectedType.AssemblyQualifiedName;
                                }
                            }

                            GUI.changed = true;
                        }

                        selectionControlId = 0;
                        SelectedType = null;
                        CreateNewTypeOverride = null;
                    }

                    break;

                case EventType.MouseDown:
                    if (GUI.enabled && position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.keyboardControl = controlId;
                        triggerDropDown = true;
                        Event.current.Use();
                    }
                    else
                    {
                        CreateNewTypeOverride = null;
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

                        if (Event.current.keyCode == KeyCode.Escape)
                        {
                            CreateNewTypeOverride = null;
                        }
                    }

                    break;

                case EventType.Repaint:
                    TempContent.text = selectedType == null ? NONE : selectedType.Name;

                    if (TempContent.text == string.Empty)
                    {
                        TempContent.text = NONE;
                    }

                    EditorStyles.popup.Draw(position, TempContent, controlId);
                    break;
            }

            if (triggerDropDown)
            {
                selectionControlId = controlId;
                SelectedType = selectedType;

                Type createInterfaceType = null;
                if (filter is ImplementsAttribute implementsAttribute)
                {
                    createInterfaceType = implementsAttribute.InterfaceType;
                }

                DisplayDropDown(position, GetFilteredTypes(filter), selectedType, GroupingOverride ?? filter?.Grouping ?? TypeGrouping.ByNamespaceFlat, CreateNewTypeOverride ?? createInterfaceType);
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
                var restoreShowMixedValue = EditorGUI.showMixedValue;

                if (TypeExtensions.TryResolveType(referenceProperty.stringValue, out var resolvedType))
                {
                    if (resolvedType.GUID != Guid.Empty)
                    {
                        referenceProperty.stringValue = resolvedType.GUID.ToString();
                    }
                    else
                    {
                        var qualifiedNameComponents = resolvedType.AssemblyQualifiedName?.Split(',');
                        Debug.Assert(qualifiedNameComponents?.Length >= 2);
                        referenceProperty.stringValue = $"{qualifiedNameComponents[0]}, {qualifiedNameComponents[1].Trim()}";
                    }
                }

                DrawTypeSelectionControl(position, label, referenceProperty, resolvedType, filter);
                EditorGUI.showMixedValue = restoreShowMixedValue;
            }
            finally
            {
                GroupingOverride = null;
                FilterConstraintOverride = null;
                ExcludedTypeCollectionGetter = null;
            }
        }

        /// <summary>
        /// Displays the type picker dropdown.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="types"></param>
        /// <param name="selectedType"></param>
        /// <param name="grouping"></param>
        /// <param name="newType"></param>
        public static void DisplayDropDown(Rect position, IEnumerable<Type> types, Type selectedType, TypeGrouping grouping, Type newType = null)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(NONE), selectedType == null, OnSelectedTypeName, null);
            menu.AddSeparator(string.Empty);

            foreach (var type in types)
            {
                var menuLabel = FormatGroupedTypeName(type, grouping);

                if (string.IsNullOrEmpty(menuLabel)) { continue; }

                var content = new GUIContent(menuLabel);
                menu.AddItem(content, type == selectedType, OnSelectedTypeName, type);
            }

            if (newType != null)
            {
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent($"Create new {newType.Name}..."), false, data =>
                {
                    MixedRealityServiceWizard.ShowNewServiceWizard(newType);
                    CreateNewTypeOverride = null;
                }, null);
            }

            menu.DropDown(position);

            void OnSelectedTypeName(object typeRef)
            {
                SelectedType = typeRef as Type;
                EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(TypeReferenceUpdated));
            }
        }

        private static string FormatGroupedTypeName(Type type, TypeGrouping grouping)
        {
            var name = type.FullName;

            switch (grouping)
            {
                case TypeGrouping.None:
                    return name;
                case TypeGrouping.NoneByNameNoNamespace:
                    return type.Name;
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