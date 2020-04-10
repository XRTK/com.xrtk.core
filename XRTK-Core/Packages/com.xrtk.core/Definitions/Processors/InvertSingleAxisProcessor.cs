// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.Devices
{
    [CreateAssetMenu(fileName = "InvertSingleAxisProcessor", menuName = "Mixed Reality Toolkit/Input Processors/InvertSingleAxisProcessor", order = 0)]
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