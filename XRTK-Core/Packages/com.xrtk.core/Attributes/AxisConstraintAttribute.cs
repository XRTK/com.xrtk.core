// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.Utilities;

namespace XRTK.Attributes
{
    public sealed class AxisConstraintAttribute : PropertyAttribute
    {
        public AxisType AxisConstraint { get; }

        public AxisConstraintAttribute(AxisType axisConstraint)
        {
            AxisConstraint = axisConstraint;
        }
    }
}