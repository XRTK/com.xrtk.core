// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace XRTK.Editor.Utilities
{
    public class ChannelPackerWindow : EditorWindow
    {
        private enum Channel
        {
            Red = 0,
            Green = 1,
            Blue = 2,
            Alpha = 3,
            RGBAverage = 4
        }

        private const float DEFAULT_UNIFORM_VALUE = -0.01f;
        private const string StandardShaderName = "Standard";
        private const string StandardRoughnessShaderName = "Standard (Roughness setup)";
        private const string StandardSpecularShaderName = "Standard (Specular setup)";


        private static readonly GUIContent metallicUniformContent = new GUIContent("Metallic Uniform");
        private static readonly GUIContent occlusionUniformContent = new GUIContent("Occlusion Uniform");
        private static readonly GUIContent smoothnessUniformContent = new GUIContent("Smoothness Uniform");
        private static readonly GUIContent emissionUniformContent = new GUIContent("Emission Uniform");

        private static readonly int OcclusionMap = Shader.PropertyToID("_OcclusionMap");
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
        private static readonly int SmoothnessTextureChannel = Shader.PropertyToID("_SmoothnessTextureChannel");
        private static readonly int SpecGlossMap = Shader.PropertyToID("_SpecGlossMap");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int MetallicMap = Shader.PropertyToID("_MetallicMap");
        private static readonly int MetallicMapChannel = Shader.PropertyToID("_MetallicMapChannel");
        private static readonly int MetallicUniform = Shader.PropertyToID("_MetallicUniform");
        private static readonly int OcclusionMapChannel = Shader.PropertyToID("_OcclusionMapChannel");
        private static readonly int OcclusionUniform = Shader.PropertyToID("_OcclusionUniform");
        private static readonly int EmissionMapChannel = Shader.PropertyToID("_EmissionMapChannel");
        private static readonly int EmissionUniform = Shader.PropertyToID("_EmissionUniform");
        private static readonly int SmoothnessMap = Shader.PropertyToID("_SmoothnessMap");
        private static readonly int SmoothnessMapChannel = Shader.PropertyToID("_SmoothnessMapChannel");
        private static readonly int SmoothnessUniform = Shader.PropertyToID("_SmoothnessUniform");

        private Texture2D metallicMap;
        private Channel metallicMapChannel = Channel.Red;
        private float metallicUniform = DEFAULT_UNIFORM_VALUE;
        private Texture2D occlusionMap;
        private Channel occlusionMapChannel = Channel.Green;
        private float occlusionUniform = DEFAULT_UNIFORM_VALUE;
        private Texture2D emissionMap;
        private Channel emissionMapChannel = Channel.RGBAverage;
        private float emissionUniform = DEFAULT_UNIFORM_VALUE;
        private Texture2D smoothnessMap;
        private Channel smoothnessMapChannel = Channel.Alpha;
        private float smoothnessUniform = DEFAULT_UNIFORM_VALUE;
        private Material standardMaterial;

        [MenuItem("Mixed Reality Toolkit/Tools/Channel Packer", false, 20)]
        private static void ShowWindow()
        {
            var window = GetWindow<ChannelPackerWindow>();
            window.titleContent = new GUIContent("Channel Packer");
            window.minSize = new Vector2(380.0f, 680.0f);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Import", EditorStyles.boldLabel);
            GUI.enabled = metallicUniform < 0.0f;
            metallicMap = (Texture2D)EditorGUILayout.ObjectField("Metallic Map", metallicMap, typeof(Texture2D), false);
            metallicMapChannel = (Channel)EditorGUILayout.EnumPopup("Input Channel", metallicMapChannel);
            GUI.enabled = true;
            metallicUniform = EditorGUILayout.Slider(metallicUniformContent, metallicUniform, DEFAULT_UNIFORM_VALUE, 1.0f);
            GUILayout.Box("Output Channel: Red", EditorStyles.helpBox);
            EditorGUILayout.Separator();
            GUI.enabled = occlusionUniform < 0.0f;
            occlusionMap = (Texture2D)EditorGUILayout.ObjectField("Occlusion Map", occlusionMap, typeof(Texture2D), false);
            occlusionMapChannel = (Channel)EditorGUILayout.EnumPopup("Input Channel", occlusionMapChannel);
            GUI.enabled = true;
            occlusionUniform = EditorGUILayout.Slider(occlusionUniformContent, occlusionUniform, DEFAULT_UNIFORM_VALUE, 1.0f);
            GUILayout.Box("Output Channel: Green", EditorStyles.helpBox);
            EditorGUILayout.Separator();
            GUI.enabled = emissionUniform < 0.0f;
            emissionMap = (Texture2D)EditorGUILayout.ObjectField("Emission Map", emissionMap, typeof(Texture2D), false);
            emissionMapChannel = (Channel)EditorGUILayout.EnumPopup("Input Channel", emissionMapChannel);
            GUI.enabled = true;
            emissionUniform = EditorGUILayout.Slider(emissionUniformContent, emissionUniform, DEFAULT_UNIFORM_VALUE, 1.0f);
            GUILayout.Box("Output Channel: Blue", EditorStyles.helpBox);
            EditorGUILayout.Separator();
            GUI.enabled = smoothnessUniform < 0.0f;
            smoothnessMap = (Texture2D)EditorGUILayout.ObjectField("Smoothness Map", smoothnessMap, typeof(Texture2D), false);
            smoothnessMapChannel = (Channel)EditorGUILayout.EnumPopup("Input Channel", smoothnessMapChannel);
            GUI.enabled = true;
            smoothnessUniform = EditorGUILayout.Slider(smoothnessUniformContent, smoothnessUniform, DEFAULT_UNIFORM_VALUE, 1.0f);
            GUILayout.Box("Output Channel: Alpha", EditorStyles.helpBox);
            EditorGUILayout.Separator();

            standardMaterial = (Material)EditorGUILayout.ObjectField("Standard Material", standardMaterial, typeof(Material), false);

            GUI.enabled = standardMaterial != null && IsUnityStandardMaterial(standardMaterial);

            if (GUILayout.Button("Auto populate from Standard Material"))
            {
                AutoPopulate();
            }

            GUI.enabled = CanSave();

            EditorGUILayout.Separator();

            GUILayout.Label("Export", EditorStyles.boldLabel);

            if (GUILayout.Button("Save Channel Map"))
            {
                Save();
            }

            GUILayout.Box("Metallic (Red), Occlusion (Green), Emission (Blue), Smoothness (Alpha)", EditorStyles.helpBox);
        }

        private void AutoPopulate()
        {
            metallicUniform = DEFAULT_UNIFORM_VALUE;
            occlusionUniform = DEFAULT_UNIFORM_VALUE;
            emissionUniform = DEFAULT_UNIFORM_VALUE;
            smoothnessUniform = DEFAULT_UNIFORM_VALUE;

            occlusionMap = (Texture2D)standardMaterial.GetTexture(OcclusionMap);
            occlusionMapChannel = occlusionMap != null ? Channel.Green : occlusionMapChannel;
            emissionMap = (Texture2D)standardMaterial.GetTexture(EmissionMap);
            emissionMapChannel = emissionMap != null ? Channel.RGBAverage : emissionMapChannel;

            switch (standardMaterial.shader.name)
            {
                case StandardShaderName:
                    metallicMap = (Texture2D)standardMaterial.GetTexture(MetallicGlossMap);
                    metallicMapChannel = metallicMap != null ? Channel.Red : metallicMapChannel;
                    smoothnessMap = ((int)standardMaterial.GetFloat(SmoothnessTextureChannel) == 0)
                            ? metallicMap
                            : (Texture2D)standardMaterial.GetTexture(MainTex);
                    smoothnessMapChannel = smoothnessMap != null ? Channel.Alpha : smoothnessMapChannel;
                    break;
                case StandardRoughnessShaderName:
                    metallicMap = (Texture2D)standardMaterial.GetTexture(MetallicGlossMap);
                    metallicMapChannel = metallicMap != null ? Channel.Red : metallicMapChannel;
                    smoothnessMap = (Texture2D)standardMaterial.GetTexture(SpecGlossMap);
                    smoothnessMapChannel = smoothnessMap != null ? Channel.Red : smoothnessMapChannel;
                    break;
                default:
                    smoothnessMap = ((int)standardMaterial.GetFloat(SmoothnessTextureChannel) == 0)
                            ? (Texture2D)standardMaterial.GetTexture(SpecGlossMap)
                            : (Texture2D)standardMaterial.GetTexture(MainTex);

                    smoothnessMapChannel = smoothnessMap != null ? Channel.Alpha : smoothnessMapChannel;
                    break;
            }
        }

        private void Save()
        {
            var textures = new Texture[] { metallicMap, occlusionMap, emissionMap, smoothnessMap };
            CalculateChannelMapSize(textures, out int width, out int height);
            var channelMap = new Texture2D(width, height);

            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            // Use the GPU to pack the various texture maps into a single texture.
            var channelPacker = new Material(Shader.Find("Hidden/ChannelPacker"));

            channelPacker.SetTexture(MetallicMap, metallicMap);
            channelPacker.SetInt(MetallicMapChannel, (int)metallicMapChannel);
            channelPacker.SetFloat(MetallicUniform, metallicUniform);
            channelPacker.SetTexture(OcclusionMap, occlusionMap);
            channelPacker.SetInt(OcclusionMapChannel, (int)occlusionMapChannel);
            channelPacker.SetFloat(OcclusionUniform, occlusionUniform);
            channelPacker.SetTexture(EmissionMap, emissionMap);
            channelPacker.SetInt(EmissionMapChannel, (int)emissionMapChannel);
            channelPacker.SetFloat(EmissionUniform, emissionUniform);
            channelPacker.SetTexture(SmoothnessMap, smoothnessMap);
            channelPacker.SetInt(SmoothnessMapChannel, (int)smoothnessMapChannel);
            channelPacker.SetFloat(SmoothnessUniform, smoothnessUniform);
            Graphics.Blit(null, renderTexture, channelPacker);
            DestroyImmediate(channelPacker);

            // Save the last render texture to a texture.
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;
            channelMap.ReadPixels(new Rect(0.0f, 0.0f, width, height), 0, 0);
            channelMap.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);

            // Save the texture to disk.
            string filename = string.Format("{0}{1}.png", GetChannelMapName(textures), "_Channel");
            string path = EditorUtility.SaveFilePanel("Save Channel Map", "", filename, "png");

            if (path.Length == 0) { return; }

            byte[] pngData = channelMap.EncodeToPNG();

            if (pngData != null)
            {
                File.WriteAllBytes(path, pngData);
                Debug.LogFormat("Saved channel map to: {0}", path);
            }
        }

        private bool CanSave()
        {
            return metallicMap != null || occlusionMap != null || emissionMap != null || smoothnessMap != null ||
                   metallicUniform >= 0.0f || occlusionUniform >= 0.0f || emissionUniform >= 0.0f || smoothnessUniform >= 0.0f;
        }

        private static bool IsUnityStandardMaterial(Material material)
        {
            if (material != null)
            {
                if (material.shader.name == StandardShaderName ||
                    material.shader.name == StandardRoughnessShaderName ||
                    material.shader.name == StandardSpecularShaderName)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetChannelMapName(Texture[] textures)
        {
            // Use the first named texture as the channel map name.
            foreach (Texture texture in textures)
            {
                if (texture != null && !string.IsNullOrEmpty(texture.name))
                {
                    return texture.name;
                }
            }

            return string.Empty;
        }

        private static void CalculateChannelMapSize(Texture[] textures, out int width, out int height)
        {
            width = 1;
            height = 1;

            // Find the max extents of all texture maps.
            foreach (Texture texture in textures)
            {
                width = texture != null ? Mathf.Max(texture.width, width) : width;
                height = texture != null ? Mathf.Max(texture.height, height) : height;
            }
        }
    }
}
