// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Services;
using System;

namespace XRTK.Examples.Demos.CustomExtensionServices
{
    /// <summary>
    /// The implementation of your <see cref="IDemoCustomExtensionService"/>
    /// </summary>
    public class DemoCustomExtensionService : BaseExtensionService, IDemoCustomExtensionService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public DemoCustomExtensionService(string name, uint priority, DemoCustomExtensionServiceProfile profile)
                : base(name, priority, profile)
        {
            if (profile == null)
            {
                throw new Exception($"{GetType().Name} expects a {nameof(DemoCustomExtensionServiceProfile)}");
            }

            // In the constructor, you should set any configuration data from your profile here.
            MyCustomData = profile.MyCustomStringData;
        }

        /// <inheritdoc />
        public string MyCustomData { get; }

        /// <inheritdoc />
        public void MyCustomMethod() { }
    }
}