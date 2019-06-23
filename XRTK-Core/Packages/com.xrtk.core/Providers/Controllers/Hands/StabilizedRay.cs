// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.Providers.Controllers.Hands
{
    /// <summary>
    /// A ray whose position and direction are stabilized in a way similar to how gaze stabilization
    /// works in HoloLens.
    /// 
    /// The ray uses Anatolie Gavrulic's "DynamicExpDecay" filter to stabilize the ray
    /// this filter adjusts its smoothing factor based on the velocity of the filtered object
    /// 
    /// The formula is
    ///  Y_smooted += ∆𝑌_𝑟
    ///  where
    /// 〖∆𝑌_𝑟=∆𝑌∗〖0.5〗^(∆𝑌/〖Halflife〗)
    /// 
    /// In code, LERP(signal, oldValue, POW(0.5, ABS(signal – oldValue) / hl)
    /// </summary>
    internal class StabilizedRay
    {
        public float HalfLifePosition { get; } = 0.1f;

        public float HalfLifeDirection { get; } = 0.1f;

        public Vector3 StabilizedPosition { get; private set; }

        public Vector3 StabilizedDirection { get; private set; }

        public Quaternion StabilizedRotation => Mathf.Abs(StabilizedDirection.magnitude) < 1e-5f ?
            Quaternion.identity : Quaternion.LookRotation(StabilizedDirection);

        private bool isInitialized = false;

        /// <summary>
        /// HalfLife closer to zero means lerp closer to one
        /// </summary>
        /// <param name="halfLife"></param>
        public StabilizedRay(float halfLife)
        {
            HalfLifePosition = halfLife;
            HalfLifeDirection = halfLife;
        }

        public StabilizedRay(float halfLifePos, float halfLifeDir)
        {
            HalfLifePosition = halfLifePos;
            HalfLifeDirection = halfLifeDir;
        }

        public void AddSample(Ray ray)
        {
            if (!isInitialized)
            {
                StabilizedPosition = ray.origin;
                StabilizedDirection = ray.direction.normalized;
                isInitialized = true;
            }
            else
            {
                StabilizedPosition = DynamicExpDecay(StabilizedPosition, ray.origin, HalfLifePosition);
                StabilizedDirection = DynamicExpDecay(StabilizedDirection, ray.direction.normalized, HalfLifeDirection);
            }
        }


        public static float DynamicExpCoefficient(float hLife, float delta)
        {
            if (hLife == 0)
            {
                return 1;
            }

            return 1.0f - Mathf.Pow(0.5f, delta / hLife);
        }

        public static Vector3 DynamicExpDecay(Vector3 from, Vector3 to, float hLife)
        {
            return Vector3.Lerp(from, to, DynamicExpCoefficient(hLife, Vector3.Distance(to, from)));
        }
    }
}