// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using XRTK.Interfaces;

namespace XRTK.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Attempts to get the name of the first configuration?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurations"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool TryGetConfigurationName<T>(this IMixedRealityServiceConfiguration[] configurations, out string name) where T : IMixedRealityService
        {
            name = string.Empty;

            for (int i = 0; i < configurations.Length; i++)
            {
                if (typeof(T).IsAssignableFrom(configurations[i].InstancedType.Type))
                {
                    name = configurations[i].Name;
                    return true;
                }
            }

            return false;
        }
    }
}