// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client.Http;
using Microsoft.AspNetCore.SignalR.Client.Infrastructure;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.SignalR.Client.Transports
{
    public class TransportHelper
    {
        // virtual to allow mocking
        public virtual Task<NegotiationResponse> GetNegotiationResponse(IHttpClient httpClient, IConnection connection, string connectionData)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException("httpClient");
            }

            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            var negotiateUrl = UrlBuilder.BuildNegotiate(connection, connectionData);

            httpClient.Initialize(connection);

            return TaskAsyncHelper.Then(TaskAsyncHelper.Then(httpClient.Get(negotiateUrl, connection.PrepareRequest, isLongRunning: false), response => response.ReadAsString()), raw =>
                            {
                                if (String.IsNullOrEmpty(raw))
                                {
                                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ServerNegotiationFailed));
                                }

                                return JsonConvert.DeserializeObject<NegotiationResponse>(raw);
                            });
        }

        // virtual to allow mocking
        public virtual Task<string> GetStartResponse(IHttpClient httpClient, IConnection connection, string connectionData, string transport)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException("httpClient");
            }

            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            var startUrl = UrlBuilder.BuildStart(connection, transport, connectionData);

            httpClient.Initialize(connection);

            return TaskAsyncHelper.Then(httpClient.Get(startUrl, connection.PrepareRequest, isLongRunning: false), response => response.ReadAsString());
        }


        public static bool VerifyLastActive(IConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            // Ensure that we have not exceeded the reconnect window
            if (DateTime.UtcNow - connection.LastActiveAt >= connection.ReconnectWindow)
            {
                connection.Trace(TraceLevels.Events, "There has not been an active server connection for an extended period of time. Stopping connection.");
                connection.Stop(new TimeoutException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ReconnectWindowTimeout,
                    connection.LastActiveAt, connection.ReconnectWindow)));

                return false;
            }

            return true;
        }
    }
}
