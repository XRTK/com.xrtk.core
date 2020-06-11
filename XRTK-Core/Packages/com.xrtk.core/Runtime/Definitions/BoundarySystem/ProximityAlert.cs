// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Definitions.BoundarySystem
{
    public enum ProximityAlert
    {
        /// <summary>
        /// The tracked object is safely within the boundary.
        /// </summary>
        Clear = 0,
        /// <summary>
        /// The tracked object's bounds have touching the boundary.
        /// </summary>
        Touch,
        /// <summary>
        /// The tracked object has crossed outside the boundary.
        /// </summary>
        Exit,
        /// <summary>
        /// The tracked object has crossed inside the boundary.
        /// </summary>
        Enter,
    }
}