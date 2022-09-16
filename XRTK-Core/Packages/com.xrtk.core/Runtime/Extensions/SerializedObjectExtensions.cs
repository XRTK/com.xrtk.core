// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using UnityEditor;

namespace XRTK.Extensions
{
    public static class SerializedObjectExtensions
    {
        /// <summary>
        /// Copies any matching serialized properties of the same name from the source to the target.
        /// </summary>
        /// <param name="serializedSource"></param>
        /// <param name="serializedTarget"></param>
        public static void CopySerializedProperties(this SerializedObject serializedSource, SerializedObject serializedTarget)
        {
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

#endif // UNITY_EDITOR
