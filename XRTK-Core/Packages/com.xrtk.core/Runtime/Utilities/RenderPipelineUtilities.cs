// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.Rendering;
using XRTK.Extensions;
using RenderPipeline = XRTK.Definitions.Utilities.RenderPipeline;

namespace XRTK.Utilities
{
    public static class RenderPipelineUtilities
    {
        private const string urpAssetTypeName = "UniversalRenderPipelineAsset";
        private const string hdrpAssetTypeName = "HDRenderPipelineAsset";

        /// <summary>
        /// Gets the <see cref="RenderPipeline"/> currently in use and active.
        /// </summary>
        /// <returns></returns>
        public static RenderPipeline GetActiveRenderingPipeline()
        {
            var renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
            if (renderPipelineAsset.IsNull())
            {
                return RenderPipeline.Legacy;
            }

            switch (renderPipelineAsset.GetType().Name)
            {
                case urpAssetTypeName:
                    return RenderPipeline.UniversalRenderPipeline;
                case hdrpAssetTypeName:
                    return RenderPipeline.HighDefinitionRenderPipeline;
            }

            return RenderPipeline.Custom;
        }
    }
}