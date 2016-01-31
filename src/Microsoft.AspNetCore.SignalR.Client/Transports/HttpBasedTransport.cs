// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client.Http;
using Microsoft.AspNetCore.SignalR.Client.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.SignalR.Client.Transports
{
    public abstract class HttpBasedTransport : ClientTransportBase
    {
        protected HttpBasedTransport(IHttpClient httpClient, string transportName)
            : base(httpClient, transportName)
        { }

        public override Task Send(IConnection connection, string data, string connectionData)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            string url = UrlBuilder.BuildSend(connection, Name, connectionData);

            var postData = new Dictionary<string, string> { { "data", data } };

            return TaskAsyncHelper.Then(TaskAsyncHelper.Then(HttpClient.Post(url, connection.PrepareRequest, postData, isLongRunning: false), response => response.ReadAsString()), raw =>
                {
                    if (!string.IsNullOrEmpty(raw))
                    {
                        connection.Trace(TraceLevels.Messages, "OnMessage({0})", raw);

                        connection.OnReceived(connection.JsonDeserializeObject<JObject>(raw));
                    }
                })
                .Catch(connection.OnError, connection);
        }
    }
}
