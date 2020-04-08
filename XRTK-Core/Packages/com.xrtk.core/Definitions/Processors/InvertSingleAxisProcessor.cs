// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Definitions.Devices
{
    public class InvertSingleAxisProcessor : InputProcessor<float>
    {
        [SerializeField]
        private bool invert = false;

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