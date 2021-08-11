// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using XRTK.Definitions;
using XRTK.Extensions;
using XRTK.Interfaces;
using XRTK.Interfaces.BoundarySystem;
using XRTK.Interfaces.Events;
using XRTK.Interfaces.TeleportSystem;
using XRTK.Services;
using Assembly = System.Reflection.Assembly;

namespace XRTK.Editor.Utilities
{
    public class MixedRealityServiceWizard : EditorWindow
    {
        private const float MIN_VERTICAL_SIZE = 192f;
        private const float MIN_HORIZONTAL_SIZE = 384f;

        private const string TABS = "        ";
        private const string NAME = "#NAME#";
        private const string BASE = "#BASE#";
        private const string GUID = "#GUID#";
        private const string USING = "#USING#";
        private const string PROFILE = "#PROFILE#";
        private const string NAMESPACE = "#NAMESPACE#";
        private const string INTERFACE = "#INTERFACE#";
        private const string IMPLEMENTS = "#IMPLEMENTS#";
        private const string PARENT_INTERFACE = "#PARENT_INTERFACE#";

        private static MixedRealityServiceWizard window = null;

        // https://stackoverflow.com/questions/6402864/c-pretty-type-name-function
        [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
        private static readonly Dictionary<string, string> BuildInTypeMap = new Dictionary<string, string>
        {
            { "Void", "void" },
            { "Boolean", "bool" },
            { "Byte", "byte" },
            { "Char", "char" },
            { "Decimal", "decimal" },
            { "Double", "double" },
            { "Single", "float" },
            { "Int32", "int" },
            { "Int64", "long" },
            { "SByte", "sbyte" },
            { "Int16", "short" },
            { "String", "string" },
            { "UInt32", "uint" },
            { "UInt64", "ulong" },
            { "UInt16", "ushort" }
        };

        private Type interfaceType;
        private Type profileBaseType;
        private Type instanceBaseType;

        private string profileTemplatePath;
        private string instanceTemplatePath;
        private string outputPath = string.Empty;
        private string @namespace = string.Empty;
        private string instanceName = string.Empty;

        public static void ShowNewServiceWizard(Type interfaceType)
        {
            if (window != null)
            {
                window.Close();
            }

            if (interfaceType == null)
            {
                Debug.LogError($"{nameof(interfaceType)} was null");
                return;
            }

            var templatePath = $"{PathFinderUtility.XRTK_Core_AbsoluteFolderPath}\\Editor\\Templates~"; ;

            window = CreateInstance<MixedRealityServiceWizard>();
            window.minSize = new Vector2(MIN_HORIZONTAL_SIZE, MIN_VERTICAL_SIZE);
            window.maxSize = new Vector2(MIN_HORIZONTAL_SIZE, MIN_VERTICAL_SIZE);
            window.position = new Rect(0f, 0f, MIN_HORIZONTAL_SIZE, MIN_VERTICAL_SIZE);
            window.titleContent = new GUIContent("XRTK Service Wizard");
            window.interfaceType = interfaceType;

            switch (interfaceType)
            {
                case Type _ when typeof(IMixedRealityEventSystem).IsAssignableFrom(interfaceType):
                    window.profileTemplatePath = $"{templatePath}\\SystemProfile.txt";
                    window.instanceTemplatePath = $"{templatePath}\\System.txt";
                    window.instanceBaseType = typeof(BaseEventSystem);
                    window.profileBaseType = typeof(BaseMixedRealityServiceProfile<>);
                    break;
                case Type _ when typeof(IMixedRealitySystem).IsAssignableFrom(interfaceType):
                    window.profileTemplatePath = $"{templatePath}\\SystemProfile.txt";
                    window.instanceTemplatePath = $"{templatePath}\\System.txt";
                    window.instanceBaseType = typeof(BaseSystem);
                    window.profileBaseType = typeof(BaseMixedRealityServiceProfile<>);
                    break;
                case Type _ when typeof(IMixedRealityDataProvider).IsAssignableFrom(interfaceType):
                    window.profileTemplatePath = $"{templatePath}\\DataProviderProfile.txt";
                    window.instanceTemplatePath = $"{templatePath}\\DataProvider.txt";
                    window.instanceBaseType = typeof(BaseDataProvider);
                    window.profileBaseType = typeof(BaseMixedRealityProfile);
                    break;
                case Type _ when typeof(IMixedRealityService).IsAssignableFrom(interfaceType):
                    window.profileTemplatePath = $"{templatePath}\\ServiceProfile.txt";
                    window.instanceTemplatePath = $"{templatePath}\\Service.txt";
                    window.instanceBaseType = typeof(BaseServiceWithConstructor);
                    window.profileBaseType = typeof(BaseMixedRealityServiceProfile<>);
                    break;
                default:
                    Debug.LogError($"{interfaceType.Name} does not implement {nameof(IMixedRealityService)}");
                    return;
            }

            window.ShowUtility();
        }

        private void OnGUI()
        {
            if (interfaceType == null)
            {
                Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = Application.dataPath;
                @namespace = $"{Application.productName.Replace("-Core", string.Empty)}";
            }

            var interfaceStrippedName = interfaceType.Name.Replace("IMixedReality", string.Empty);

            if (string.IsNullOrWhiteSpace(instanceName))
            {
                instanceName = interfaceStrippedName;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"Let's create a {interfaceType.Name}!", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent($"Choose a path to put your new {Path.GetFileNameWithoutExtension(instanceTemplatePath).ToProperCase()}:"));
            const int maxCharacterLength = 56;
            EditorGUILayout.TextField(outputPath, new GUIStyle("label")
            {
                alignment = outputPath.Length > maxCharacterLength
                    ? TextAnchor.MiddleRight
                    : TextAnchor.MiddleLeft
            });

            if (GUILayout.Button("Choose the output path"))
            {
                outputPath = EditorUtility.OpenFolderPanel("Generation Location", outputPath, string.Empty);
                var namespaceRoot = $"{Application.productName.Replace("-Core", string.Empty)}";

                @namespace = outputPath.Replace($"{Directory.GetParent(Application.dataPath).FullName.ForwardSlashes()}/", string.Empty);

                if (@namespace.StartsWith("Assets/"))
                {
                    @namespace = @namespace.Replace("Assets/", $"{namespaceRoot}/");
                }
                else if (@namespace.StartsWith("Packages/"))
                {
                    var isEditor = @namespace.Contains("Editor/") ? "Editor/" : string.Empty;
                    @namespace = Regex.Replace(@namespace, "(?<path>(Packages.*Runtime.)|(Packages.*Editor.))", $"{namespaceRoot}/{isEditor}");
                }
                else
                {
                    Debug.LogError($"Failed to find a valid {nameof(outputPath)}");
                }

                @namespace = @namespace.Replace("/", ".");
            }

            EditorGUILayout.Space();
            @namespace = EditorGUILayout.TextField("Namespace", @namespace);

            EditorGUI.BeginChangeCheck();
            instanceName = EditorGUILayout.TextField("Instance Name", instanceName);

            GUILayout.FlexibleSpace();

            GUI.enabled = !string.IsNullOrWhiteSpace(instanceName) && !string.IsNullOrWhiteSpace(@namespace);

            if (GUILayout.Button("Generate!"))
            {
                EditorApplication.delayCall += () =>
                {
                    try
                    {
                        var usingList = new List<string>();

                        if (!usingList.Contains(instanceBaseType.Namespace))
                        {
                            usingList.Add(instanceBaseType.Namespace);
                        }

                        if (!usingList.Contains(interfaceType.Namespace))
                        {
                            usingList.Add(interfaceType.Namespace);
                        }

                        Type parentInterfaceType = null;

                        if (interfaceType.Name.Contains("DataProvider"))
                        {
                            var parentInterfaceName = interfaceType.Name.Replace("DataProvider", "System");

                            parentInterfaceType = GetType(parentInterfaceName);

                            if (parentInterfaceType == null)
                            {
                                parentInterfaceName = interfaceType.Name.Replace("DataProvider", "Service");
                            }

                            parentInterfaceType = GetType(parentInterfaceName);

                            if (parentInterfaceType != null)
                            {
                                if (!usingList.Contains(parentInterfaceType.Namespace))
                                {
                                    usingList.Add(parentInterfaceType.Namespace);
                                }
                            }
                            else
                            {
                                Debug.LogError($"Failed to resolve parent interface for {interfaceType.Name}");
                            }
                        }

                        var implements = string.Empty;

                        var members = interfaceType.GetMembers();
                        var events = new List<EventInfo>();
                        var properties = new List<PropertyInfo>();
                        var methods = new List<MethodInfo>();

                        foreach (var memberInfo in members)
                        {
                            switch (memberInfo)
                            {
                                case EventInfo eventInfo:
                                    events.Add(eventInfo);
                                    break;
                                case PropertyInfo propertyInfo:
                                    properties.Add(propertyInfo);
                                    break;
                                case MethodInfo methodInfo:
                                    methods.Add(methodInfo);
                                    break;
                            }
                        }

                        if (instanceBaseType != typeof(BaseServiceWithConstructor))
                        {
                            implements = events.Aggregate(implements, (current, eventInfo) => $"{current}{FormatMemberInfo(eventInfo, ref usingList)}");
                            implements = properties.Aggregate(implements, (current, propertyInfo) => $"{current}{FormatMemberInfo(propertyInfo, ref usingList)}");
                            implements = methods.Aggregate(implements, (current, methodInfo) => $"{current}{FormatMemberInfo(methodInfo, ref usingList)}");
                        }

                        Type profileType = null;
                        var profileBaseTypeName = profileBaseType.Name;

                        if (profileBaseTypeName.Contains("`1"))
                        {
                            var dataProviderInterfaceTypeName = interfaceType.Name
                                .Replace("System", "DataProvider")
                                .Replace("Service", "DataProvider");

                            var dataProviderType = GetType(dataProviderInterfaceTypeName);

                            if (dataProviderType != null)
                            {
                                if (!usingList.Contains(dataProviderType.Namespace))
                                {
                                    usingList.Add(dataProviderType.Namespace);
                                }

                                var constructors = dataProviderType.GetConstructors();

                                foreach (var constructorInfo in constructors)
                                {
                                    var parameters = constructorInfo.GetParameters();

                                    foreach (var parameterInfo in parameters)
                                    {
                                        if (parameterInfo.ParameterType.IsAbstract) { continue; }

                                        if (parameterInfo.ParameterType.IsSubclassOf(typeof(BaseMixedRealityProfile)))
                                        {
                                            profileType = parameterInfo.ParameterType;
                                            break;
                                        }
                                    }

                                    if (profileType != null)
                                    {
                                        profileBaseTypeName = profileType.Name;
                                        break;
                                    }
                                }

                            }
                            else
                            {
                                if (interfaceType == typeof(IMixedRealityBoundarySystem) ||
                                    interfaceType == typeof(IMixedRealityTeleportSystem))
                                {
                                    profileBaseTypeName = nameof(BaseMixedRealityProfile);
                                }
                                else
                                {
                                    Debug.LogError($"Failed to resolve {dataProviderInterfaceTypeName}");
                                }
                            }

                            profileBaseTypeName = profileBaseTypeName.Replace("`1", $"<{dataProviderInterfaceTypeName}>");
                        }

                        if (!usingList.Contains(profileBaseType.Namespace))
                        {
                            usingList.Add(profileBaseType.Namespace);
                        }

                        usingList.Sort();

                        var @using = usingList.Aggregate(string.Empty, (current, item) => $"{current}{Environment.NewLine}using {item};");

                        var instanceTemplate = File.ReadAllText(instanceTemplatePath ?? throw new InvalidOperationException());
                        instanceTemplate = instanceTemplate.Replace(USING, @using);
                        instanceTemplate = instanceTemplate.Replace(NAMESPACE, @namespace);
                        instanceTemplate = instanceTemplate.Replace(GUID, Guid.NewGuid().ToString());
                        instanceTemplate = instanceTemplate.Replace(NAME, instanceName);
                        instanceTemplate = instanceTemplate.Replace(BASE, instanceBaseType.Name);
                        instanceTemplate = instanceTemplate.Replace(INTERFACE, interfaceType.Name);
                        instanceTemplate = instanceTemplate.Replace(PARENT_INTERFACE, parentInterfaceType?.Name);
                        instanceTemplate = instanceTemplate.Replace(IMPLEMENTS, implements);
                        instanceTemplate = instanceTemplate.Replace(PROFILE, profileBaseTypeName);

                        File.WriteAllText($"{outputPath}/{instanceName}.cs", instanceTemplate);

                        if (profileBaseTypeName != nameof(BaseMixedRealityProfile))
                        {
                            usingList.Clear();

                            if (!usingList.Contains(profileBaseType.Namespace))
                            {
                                usingList.Add(profileBaseType.Namespace);
                            }

                            if (!usingList.Contains("XRTK.Interfaces"))
                            {
                                usingList.Add("XRTK.Interfaces");
                            }

                            if (!usingList.Contains("UnityEngine"))
                            {
                                usingList.Add("UnityEngine");
                            }
                            
                            usingList.Sort();

                            @using = usingList.Aggregate(string.Empty, (current, item) => $"{current}{Environment.NewLine}using {item};");

                            var profileTemplate = File.ReadAllText(profileTemplatePath ?? throw new InvalidOperationException());
                            profileTemplate = profileTemplate.Replace(USING, @using);
                            profileTemplate = profileTemplate.Replace(NAMESPACE, @namespace);
                            profileTemplate = profileTemplate.Replace(NAME, instanceName);
                            profileTemplate = profileTemplate.Replace(BASE, profileBaseTypeName);

                            File.WriteAllText($"{outputPath}/{instanceName}Profile.cs", profileTemplate);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    finally
                    {
                        Close();
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }
                };
            }

            GUI.enabled = true;
            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }

        private static string FormatMemberInfo(MemberInfo memberInfo, ref List<string> usingList)
        {
            var result = $"{Environment.NewLine}{Environment.NewLine}{TABS}/// <inheritdoc />{Environment.NewLine}{TABS}";

            switch (memberInfo)
            {
                case EventInfo eventInfo:
                    result += $"public event {PrettyPrintTypeName(eventInfo.EventHandlerType, ref usingList)} {eventInfo.Name};";
                    break;
                case PropertyInfo propertyInfo:
                    var getter = propertyInfo.CanRead ? " get;" : string.Empty;
                    var setter = propertyInfo.CanWrite ? " set;" : string.Empty;

                    if (!usingList.Contains(propertyInfo.PropertyType.Namespace))
                    {
                        usingList.Add(propertyInfo.PropertyType.Namespace);
                    }

                    result += $"public {PrettyPrintTypeName(propertyInfo.PropertyType, ref usingList)} {propertyInfo.Name} {{{getter}{setter} }}";
                    break;
                case MethodInfo methodInfo:
                    if (methodInfo.Name.Contains("get_") ||
                        methodInfo.Name.Contains("set_") ||
                        methodInfo.Name.Contains("add_") ||
                        methodInfo.Name.Contains("remove_"))
                    {
                        return string.Empty;
                    }

                    if (!usingList.Contains(methodInfo.ReturnType.Namespace))
                    {
                        usingList.Add(methodInfo.ReturnType.Namespace);
                    }

                    var returnTypeName = PrettyPrintTypeName(methodInfo.ReturnType, ref usingList);
                    var parameters = string.Empty;
                    var parameterList = new List<string>();

                    foreach (var parameterInfo in methodInfo.GetParameters())
                    {
                        var isByRef = parameterInfo.ParameterType.IsByRef
                            ? parameterInfo.IsOut
                                ? "out "
                                : "ref "
                            : string.Empty;
                        //var isOptional = parameterInfo.IsOptional
                        //    ? $" = {PrettyPrintTypeName(parameterInfo.GetOptionalCustomModifiers()[0], ref usingList)}"
                        //    : string.Empty;
                        parameterList.Add($"{isByRef}{PrettyPrintTypeName(parameterInfo.ParameterType, ref usingList)} {parameterInfo.Name}");
                    }

                    for (var i = 0; i < parameterList.Count; i++)
                    {
                        var isLast = i + 1 == parameterList.Count;
                        var comma = isLast ? string.Empty : ", ";
                        parameters += $"{parameterList[i]}{comma}";
                    }

                    result += $"public {returnTypeName} {methodInfo.Name}({parameters}){Environment.NewLine}{TABS}{{{Environment.NewLine}{TABS}    throw new NotImplementedException();{Environment.NewLine}{TABS}}}";
                    break;
                default:
                    Debug.LogWarning($"Unhandled {nameof(memberInfo)}\n{memberInfo}");
                    result += $"{memberInfo}";
                    break;
            }

            return result;
        }

        private static string PrettyPrintTypeName(Type type, ref List<string> usingList)
        {
            string typeName;

            if (BuildInTypeMap.ContainsKey(type.Name))
            {
                typeName = BuildInTypeMap[type.Name];
            }
            else
            {
                if (!usingList.Contains(type.Namespace))
                {
                    usingList.Add(type.Namespace);
                }

                typeName = type.IsByRef
                    ? PrettyPrintTypeName(type.GetElementType(), ref usingList)
                    : type.Name;

                if (type.IsNested)
                {
                    typeName = type.FullName?
                        .Replace($"{type.Namespace}.", string.Empty)
                        .Replace("+", ".");
                }
            }

            if (!type.IsGenericType)
            {
                return typeName;
            }

            var genericArguments = type.GetGenericArguments();

            if (genericArguments.Length == 0)
            {
                return typeName;
            }

            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return $"{PrettyPrintTypeName(Nullable.GetUnderlyingType(type), ref usingList)}?";
            }

            var genericTypeNames = new List<string>();

            foreach (var genericType in genericArguments)
            {
                var name = PrettyPrintTypeName(genericType, ref usingList);

                if (!genericTypeNames.Contains(name))
                {
                    genericTypeNames.Add(name);
                }
            }

            var mangledName = typeName.Contains("`")
                ? typeName.Substring(0, typeName.IndexOf("`", StringComparison.Ordinal))
                : typeName;
            return $"{mangledName}<{string.Join(",", genericTypeNames)}>";
        }

        private static Type GetType(string name) => GetTypes().FirstOrDefault(type => type.Name == name);

        private static IEnumerable<Type> GetTypes()
        {
            var types = new List<Type>();
            var assemblies = CompilationPipeline.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var compiledAssembly = Assembly.Load(assembly.name);
                types.AddRange(compiledAssembly.ExportedTypes);
            }

            types.Sort((a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));
            return types;
        }
    }
}
