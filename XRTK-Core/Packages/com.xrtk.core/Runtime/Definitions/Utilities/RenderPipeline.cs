// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.Utilities
{
    /// <summary>
    /// Available render pipelines for use in Unity applications.
    /// </summary>
    public enum RenderPipeline
    {
        /// <summary>
        /// The legacy "built-in" render pipeline of Unity that was deprecated
        /// when scriptable render pipelines were introduced.
        /// </summary>
        Legacy = 0,
        /// <summary>
        /// The universal render pipeline, formerly also known as lightweight render pipeline.
        /// </summary>
        UniversalRenderPipeline,
        /// <summary>
        /// The high definition pipeline.
        /// </summary>
        HighDefinitionRenderPipeline,
        /// <summary>
        /// A customized scriptable render pipeline.
        /// </summary>
        Custom
    }
}