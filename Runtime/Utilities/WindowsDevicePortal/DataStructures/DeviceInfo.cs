﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using XRTK.Utilities.WebRequestRest;

namespace XRTK.Utilities.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class DeviceInfo
    {
        // These fields are public to be serialized by the Unity Json Serializer Utility.
        #region Json Serialized Fields

        /// <summary>
        /// The IP Address of the device.
        /// </summary>
        public string IP;

        /// <summary>
        /// The user name of the device.
        /// </summary>
        public string User;

        /// <summary>
        /// The password for the device.
        /// </summary>
        public string Password;

        /// <summary>
        /// The machine name of the device.
        /// </summary>
        public string MachineName;

        #endregion Json Serialized Fields

        // These fields are public but NonSerialized because we don't want them serialized by the
        // Json Utility, but we also don't want their values overwritten when deserialization happens.
        #region Json Overwritten Fields

        /// <summary>
        /// The current CSRF Token for the device.
        /// </summary>
        [NonSerialized]
        public string CsrfToken;

        private Dictionary<string, string> authorization;

        #endregion Json Overwritten Fields

        // Properties are not serialized by the Unity JSON serializer, but become null whenever deserialized.
        #region Properties

        /// <summary>
        /// The current authorization for the device.
        /// </summary>
        public Dictionary<string, string> Authorization => authorization ?? (authorization = new Dictionary<string, string> { { "Authorization", Rest.GetBasicAuthentication(User, Password) } });

        /// <summary>
        /// The last known battery state of the device.
        /// </summary>
        public BatteryInfo BatteryInfo { get; set; }

        /// <summary>
        /// The last known power state of the device.
        /// </summary>
        public PowerStateInfo PowerState { get; set; }

        #endregion Properties

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="machineName"></param>
        public DeviceInfo(string ip, string user, string password, string machineName = "")
        {
            IP = ip;
            User = user;
            Password = password;
            MachineName = string.IsNullOrEmpty(machineName) ? ip : machineName;
            CsrfToken = string.Empty;
        }
    }
}
