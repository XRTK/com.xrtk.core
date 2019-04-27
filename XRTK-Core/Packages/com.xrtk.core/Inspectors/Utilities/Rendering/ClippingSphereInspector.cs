// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;
using XRTK.Utilities.Rendering;

namespace XRTK.Inspectors
{
    [CustomEditor(typeof(ClippingSphere))]
    public class ClippingSphereEditor : Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var primitive = target as ClippingSphere;
            Debug.Assert(primitive != null);
            return new Bounds(primitive.transform.position, Vector3.one * primitive.Radius);
        }
    }
}