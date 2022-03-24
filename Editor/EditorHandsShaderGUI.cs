﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace XRTK.Editor
{
    public class EditorHandsShaderGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            foreach (MaterialProperty materialProperty in properties)
            {
                if (materialProperty.flags != MaterialProperty.PropFlags.PerRendererData)
                {
                    materialEditor.ShaderProperty(materialProperty, materialProperty.displayName);
                }
            }
        }
    }
}