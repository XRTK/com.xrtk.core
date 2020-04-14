// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions;
using XRTK.Interfaces;

namespace XRTK.Services
{
    /// <summary>
    /// The base data provider implements <see cref="IMixedRealityDataProvider"/> and provides default properties for all data providers.
    /// </summary>
    public abstract class BaseDataProvider : BaseServiceWithConstructor, IMixedRealityDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="priority">The priority of the service.</param>
        /// <param name="profile">The optional <see cref="BaseMixedRealityProfile"/> for the data provider.</param>
        /// <param name="parentService">The <see cref="IMixedRealityService"/> that this <see cref="IMixedRealityDataProvider"/> is assigned to.</param>
        protected BaseDataProvider(string name, uint priority, BaseMixedRealityProfile profile, IMixedRealityService parentService) : base(name, priority)
        {
            ParentService = parentService ?? throw new ArgumentNullException($"{nameof(parentService)} cannot be null");
        }

        /// <inheritdoc />
        public IMixedRealityService ParentService { get; }

        private uint priority;

        /// <inheritdoc />
        public override uint Priority
        {
            get => priority + ParentService.Priority;
            protected set => priority = value;
        }
    }
}