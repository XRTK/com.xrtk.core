// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Definitions.Devices
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Input System/Processors/Invert Single Axis", fileName = "InvertSingleAxisProcessor", order = (int)CreateProfileMenuItemIndices.InputProcessors)]
    public class InvertSingleAxisProcessor : InputProcessor<float>
    {
        [SerializeField]
        private bool invert = true;

        public bool Invert
        {
            get => invert;
            set => invert = value;
        }

        /// <inheritdoc />
        public override void Process(ref float value)
        {
            value *= -1f;
        }
    }
}