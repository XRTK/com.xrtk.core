// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.Utilities.WebRequestRest
{
    /// <summary>
    /// Response to a REST Call.
    /// </summary>
    public struct Response
    {
        /// <summary>
        /// Was the REST call successful?
        /// </summary>
        public bool Successful { get; }

        /// <summary>
        /// Response body from the resource.
        /// </summary>
        public string ResponseBody { get; }

        /// <summary>
        /// Response data from the resource.
        /// </summary>
        public byte[] ResponseData { get; }

        /// <summary>
        /// Response code from the resource.
        /// </summary>
        public long ResponseCode { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="successful">Was the REST call successful?</param>
        /// <param name="responseBody">Response body from the resource.</param>
        /// <param name="responseData">Response data from the resource.</param>
        /// <param name="responseCode">Response code from the resource.</param>
        public Response(bool successful, string responseBody, byte[] responseData, long responseCode)
        {
            Successful = successful;
            ResponseBody = responseBody;
            ResponseData = responseData;
            ResponseCode = responseCode;
        }
    }
}
