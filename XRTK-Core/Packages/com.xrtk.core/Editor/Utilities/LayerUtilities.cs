// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Extensions;

namespace XRTK.Editor.Utilities
{
    public static class LayerUtilities
    {
        private static UnityEditor.SerializedProperty tagManagerLayers = null;

        /// <summary>
        /// The current layers defined in the Tag Manager.
        /// </summary>
        public static UnityEditor.SerializedProperty TagManagerLayerProperties
        {
            get
            {
                if (tagManagerLayers == null)
                {
                    InitializeTagManager();
                }

                return tagManagerLayers;
            }
        }

        private static void InitializeTagManager()
        {
            var tagAssets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            if (tagAssets == null || tagAssets.Length == 0)
            {
                Debug.LogError("Failed to load TagManager!");
                return;
            }

            var tagsManager = new UnityEditor.SerializedObject(tagAssets);
            tagManagerLayers = tagsManager.FindProperty("layers");

            Debug.Assert(tagManagerLayers != null);
        }

        /// <summary>
        /// Attempts to set the layer in Project Settings Tag Manager.
        /// </summary>
        /// <remarks>
        /// If the layer is already set, then it attempts to set the next available layer.
        /// </remarks>
        /// <param name="layerId">The layer Id to attempt to set the layer on.</param>
        /// <param name="layerName">The layer name to attempt to set the layer on.</param>
        public static int SetupLayer(int layerId, string layerName)
        {
            if (CheckLayer(layerName, out var oldLayerId))
            {
                // layer already set.
                return oldLayerId;
            }

            bool wrapped = false;

            while (layerId != LayerExtensions.InvalidLayerId)
            {
                if (layerId > 31 && !wrapped)
                {
                    // Wrap back down to the beginning
                    layerId = 8;
                    wrapped = true;
                }

                var layer = TagManagerLayerProperties.GetArrayElementAtIndex(layerId);

                if (layer.stringValue == layerName)
                {
                    // layer already set.
                    return layerId;
                }

                if (!string.IsNullOrWhiteSpace(layer.stringValue))
                {
                    layerId++;
                    // Target layer in use and may be being used for something else already
                    // so let's set it to the next empty layer
                    continue;
                }

                // Set the layer name.
                layer.stringValue = layerName;
                layer.serializedObject.ApplyModifiedProperties();
                UnityEditor.AssetDatabase.SaveAssets();
                return layerId;
            }

            Debug.LogError($"Failed to set layer {layerName}. All Layers are in use.");
            return LayerExtensions.InvalidLayerId;
        }

        /// <summary>
        /// Check if the layer already has an id
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="layerId"></param>
        /// <returns></returns>
        public static bool CheckLayer(string layerName, out int layerId)
        {
            for (int i = 0; i < 32; i++)
            {
                var layer = TagManagerLayerProperties.GetArrayElementAtIndex(i);

                if (layer.stringValue == layerName)
                {
                    // layer already set.
                    layerId = i;
                    return true;
                }
            }

            layerId = LayerExtensions.InvalidLayerId;
            return false;
        }

        /// <summary>
        /// Attempts to remove the layer from the Project Settings Tag Manager.
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool RemoveLayer(string layerName)
        {
            for (int i = 0; i < TagManagerLayerProperties.arraySize; i++)
            {
                var layer = TagManagerLayerProperties.GetArrayElementAtIndex(i);

                if (layer.stringValue == layerName)
                {
                    layer.stringValue = string.Empty;
                    layer.serializedObject.ApplyModifiedProperties();
                    UnityEditor.AssetDatabase.SaveAssets();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the spatial awareness layers exists, and adds them.
        /// </summary>
        /// <returns>True, if the layers were added.</returns>
        public static bool CheckLayers(Tuple<int, string>[] layers)
        {
            var result = false;

            for (int i = 0; i < layers.Length; i++)
            {
                var (layerId, layerName) = layers[i];

                if (!CheckLayer(layerName, out _))
                {
                    layerId = SetupLayer(layerId, layerName);
                    var added = layerId != LayerExtensions.InvalidLayerId;
                    result = i == 0 ? added : result && added;
                }
                else
                {
                    return false;
                }
            }

            return result;
        }

        /// <summary>
        /// Removes the spatial awareness layers if they exist.
        /// </summary>
        /// <returns>True, if the layers were removed.</returns>
        public static bool RemoveLayers(Tuple<int, string>[] layers)
        {
            var result = false;

            for (int i = 0; i < layers.Length; i++)
            {
                var (_, layerName) = layers[i];
                var removed = RemoveLayer(layerName);
                result = i == 0 ? removed : result && removed;
            }

            return result;
        }
    }
}