﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.SignalR.Client.Hubs
{
    public class HubProgressUpdate
    {
        /// <summary>
        /// The callback identifier
        /// </summary>
        [JsonProperty("I")]
        public string Id { get; set; }

        /// <summary>
        /// The progress value
        /// </summary>
        [JsonProperty("D")]
        public JToken Data { get; set; }
    }
}