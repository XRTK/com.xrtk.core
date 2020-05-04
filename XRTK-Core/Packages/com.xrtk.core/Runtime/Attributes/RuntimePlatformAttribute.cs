using System;
using UnityEngine;
using XRTK.Interfaces;

namespace XRTK.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RuntimePlatformAttribute : PropertyAttribute
    {
        public Type Platform { get; }

        public RuntimePlatformAttribute(Type platformType)
        {
            if (typeof(IMixedRealityPlatform).IsAssignableFrom(platformType))
            {
                Platform = platformType;
            }
            else
            {
                throw new ArgumentException($"{nameof(platformType)} must implement {nameof(IMixedRealityPlatform)}");
            }
        }
    }
}
