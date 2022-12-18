using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XRTK.Editor.Utilities
{
    public static class ComponentEditorUtility
    {
        private class ComponentUpgradePopup : EditorWindow
        {
            private bool initializedPosition;

            private TypeCache.TypeCollection types;

            private Component component;

            private void OnEnable()
            {
                GetWindow(typeof(ComponentUpgradePopup));
            }

            public void Init(Component component, TypeCache.TypeCollection types)
            {
                this.component = component;
                this.types = types;
            }

            private void OnGUI()
            {
                if (!initializedPosition)
                {
                    Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                    position = new Rect(mousePos.x - (position.width / 2), mousePos.y, position.width, position.height);
                    initializedPosition = true;
                }

                GUILayout.Space(20);

                foreach (var type in types)
                {
                    if (GUILayout.Button(new GUIContent(type.Name)))
                    {
                        ReplaceComponent(type, component);
                        Close();
                    }
                }
            }
        }

        [MenuItem("CONTEXT/Component/Upgrade Component", true, 9998)]
        private static bool UpgradeValidate(MenuCommand command)
        {
            if (command.context is not Component component)
            {
                return false;
            }

            var types = TypeCache.GetTypesDerivedFrom(component.GetType());
            return types.Count > 0;
        }

        [MenuItem("CONTEXT/Component/Upgrade Component", false, 9998)]
        private static void Upgrade(MenuCommand command)
        {
            if (command.context is not Component component)
            {
                return;
            }

            var types = TypeCache.GetTypesDerivedFrom(command.context.GetType());

            if (types.Count == 1)
            {
                ReplaceComponent(types[0], component);
            }
            else
            {
                Debug.Log(EditorWindow.focusedWindow.GetType().Name);
                var typePicker = ScriptableObject.CreateInstance<ComponentUpgradePopup>();
                typePicker.Init(component, types);
                typePicker.ShowPopup();
            }
        }

        [MenuItem("CONTEXT/Component/Downgrade Component", true, 9999)]
        private static bool DowngradeValidate(MenuCommand command)
        {
            if (command.context is not Component component)
            {
                return false;
            }

            var baseType = component.GetType().BaseType;
            return baseType != typeof(MonoBehaviour) &&
                   baseType != typeof(Component) &&
                   baseType != typeof(Behaviour) &&
                   baseType != typeof(Transform) &&
                   baseType!.IsClass && !baseType!.IsAbstract;
        }

        [MenuItem("CONTEXT/Component/Downgrade Component", false, 9999)]
        private static void Downgrade(MenuCommand command)
        {
            if (command.context is not Component component)
            {
                return;
            }

            ReplaceComponent(component.GetType().BaseType, component);
        }

        private static void ReplaceComponent(Type type, Component @base)
        {
            var baseType = @base.GetType();
            var serializedSource = new SerializedObject(@base);
            var gameObject = @base.gameObject;
            Object.DestroyImmediate(@base);
            var component = gameObject.AddComponent(type);

            // if something bad happened revert
            if (component == null)
            {
                component = gameObject.AddComponent(baseType);
            }

            SerializedObject serializedTarget = new SerializedObject(component);
            SerializedProperty iterator = serializedSource.GetIterator();

            // jump into serialized object, this will skip script type so that we don't override the destination component's type
            if (iterator.NextVisible(true))
            {
                // iterate through all serializedProperties
                while (iterator.NextVisible(true))
                {
                    // try obtaining the property in destination component
                    SerializedProperty element = serializedTarget.FindProperty(iterator.name);

                    // validate that the properties are present in both components, and that they're the same type
                    if (element != null && element.propertyType == iterator.propertyType)
                    {
                        // copy value from source to destination component
                        serializedTarget.CopyFromSerializedProperty(iterator);
                    }
                }
            }

            serializedTarget.ApplyModifiedProperties();
        }
    }
}
