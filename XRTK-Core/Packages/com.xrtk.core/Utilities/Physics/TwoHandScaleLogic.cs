// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace XRTK.Utilities.Physics
{
    /// <summary>
    /// Implements a scale logic that will scale an object based on the 
    /// ratio of the distance between hands.
    /// 
    /// <code>
    /// object_scale = start_object_scale * curr_hand_dist / start_hand_dist
    /// </code>
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    public class TwoHandScaleLogic
    {
        private Vector3 startObjectScale;
        private float startHandDistanceMeters;

        /// <summary>
        /// Initialize system with source info from controllers/hands
        /// </summary>
        /// <param name="handsPressedMap">Dictionary that maps inputSources to states</param>
        /// <param name="manipulationRoot">Transform of gameObject to be manipulated</param>
        public virtual void Setup(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot)
        {
            startHandDistanceMeters = GetMinDistanceBetweenHands(handsPressedMap);
            startObjectScale = manipulationRoot.transform.localScale;
        }

        /// <summary>
        /// Update the current scale based on the distance between the hands.
        /// </summary>
        /// <param name="handsPressedMap"></param>
        /// <returns>a <see cref="Vector3"/> describing the new Scale of the <see cref="GameObject"/> being manipulated</returns>
        public virtual Vector3 UpdateMap(Dictionary<uint, Vector3> handsPressedMap)
        {
            return startObjectScale * (GetMinDistanceBetweenHands(handsPressedMap) / startHandDistanceMeters);
        }

        private static float GetMinDistanceBetweenHands(Dictionary<uint, Vector3> handsPressedMap)
        {
            var result = float.MaxValue;
            // TODO cache the array and only update it if the count changes.
            var handLocations = new Vector3[handsPressedMap.Values.Count];
            handsPressedMap.Values.CopyTo(handLocations, 0);

            for (int i = 0; i < handLocations.Length; i++)
            {
                for (int j = i + 1; j < handLocations.Length; j++)
                {
                    var distance = Vector3.Distance(handLocations[i], handLocations[j]);

                    if (distance < result)
                    {
                        result = distance;
                    }
                }
            }

            return result;
        }
    }
}
