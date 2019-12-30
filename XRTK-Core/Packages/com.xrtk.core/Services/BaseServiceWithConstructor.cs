// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Interfaces;

namespace XRTK.Services
{
    /// <summary>
    /// Base <see cref="IMixedRealityService"/> with a constructor override.
    /// </summary>
    public abstract class BaseServiceWithConstructor : BaseService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="priority">The priority of the service.</param>
        protected BaseServiceWithConstructor(string name = "", uint priority = 10)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = GetType().Name;
            }

            this.name = name;
            this.priority = priority;
        }

        private readonly string name;

        /// <inheritdoc />
        public override string Name => name;

        private readonly uint priority;

        /// <inheritdoc />
        public override uint Priority => priority;
    }
}