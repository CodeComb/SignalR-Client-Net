﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client.Http;
using Microsoft.AspNetCore.SignalR.Client.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.SignalR.Client.Transports
{
    public abstract class ClientTransportBase : IClientTransport
    {
        private readonly IHttpClient _httpClient;
        private readonly string _transportName;
        private readonly TransportAbortHandler _abortHandler;
        private bool _finished = false;

        private TransportInitializationHandler _initializationHandler;

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in the Dispose method.")]
        protected ClientTransportBase(IHttpClient httpClient, string transportName)
            : this(httpClient, transportName, new TransportHelper(), new TransportAbortHandler(httpClient, transportName))
        {
        }

        internal ClientTransportBase(IHttpClient httpClient, string transportName, TransportHelper transportHelper, TransportAbortHandler abortHandler)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (string.IsNullOrWhiteSpace(transportName))
            {
                throw new ArgumentNullException(nameof(transportName));
            }

            Debug.Assert(transportHelper != null, "transportHelper is null");
            Debug.Assert(abortHandler != null, "abortHandler is null");

            _httpClient = httpClient;
            _transportName = transportName;
            TransportHelper = transportHelper;
            _abortHandler = abortHandler;
        }

        protected IHttpClient HttpClient => _httpClient;

        protected TransportHelper TransportHelper { get; }

        protected TransportAbortHandler AbortHandler => _abortHandler;

        /// <summary>
        /// Gets transport name.
        /// </summary>
        public string Name => _transportName;

        public abstract bool SupportsKeepAlive { get; }

        public Task<NegotiationResponse> Negotiate(IConnection connection, string connectionData)
        {
            if (_finished)
            {
                throw new InvalidOperationException(Resources.Error_TransportCannotBeReused);
            }

            return TransportHelper.GetNegotiationResponse(HttpClient, connection, connectionData);
        }

        public Task Start(IConnection connection, string connectionData, CancellationToken disconnectToken)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _initializationHandler = new TransportInitializationHandler(HttpClient, connection, connectionData, Name, disconnectToken, TransportHelper);
            _initializationHandler.OnFailure += OnStartFailed;

            OnStart(connection, connectionData, disconnectToken);

            return _initializationHandler.Task;
        }

        protected abstract void OnStart(IConnection connection, string connectionData, CancellationToken disconnectToken);

        protected abstract void OnStartFailed();

        // internal for testing
        protected internal void TransportFailed(Exception ex)
        {
            // will be no-op if handler already finished (either succeeded or failed)
            if (ex == null)
            {
                _initializationHandler.Fail();
            }
            else
            {
                _initializationHandler.Fail(ex);
            }
        }

        public abstract Task Send(IConnection connection, string data, string connectionData);

        public virtual void Abort(IConnection connection, TimeSpan timeout, string connectionData)
        {
            _finished = true;
            AbortHandler.Abort(connection, timeout, connectionData);
        }

        public abstract void LostConnection(IConnection connection);

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The client receives the exception in the OnError callback.")]
        // virtual to allow mocking
        protected internal virtual bool ProcessResponse(IConnection connection, string response)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (_initializationHandler == null)
            {
                throw new InvalidOperationException(Resources.Error_ProcessResponseBeforeStart);
            }

            connection.MarkLastMessage();

            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            var shouldReconnect = false;

            try
            {
                var result = connection.JsonDeserializeObject<JObject>(response);

                if (!result.HasValues)
                {
                    return false;
                }

                if (result["I"] != null)
                {
                    connection.OnReceived(result);
                    return false;
                }

                shouldReconnect = (int?)result["T"] == 1;

                var groupsToken = result["G"];
                if (groupsToken != null)
                {
                    connection.GroupsToken = (string)groupsToken;
                }

                var messages = result["M"] as JArray;
                if (messages != null)
                {
                    connection.MessageId = (string)result["C"];

                    foreach (JToken message in (IEnumerable<JToken>)messages)
                    {
                        connection.OnReceived(message);
                    }

                    if ((int?)result["S"] == 1)
                    {
                        _initializationHandler.InitReceived();
                    }
                }
            }
            catch (Exception ex)
            {
                connection.OnError(ex);
            }

            return shouldReconnect;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _finished = true;
                _abortHandler.Dispose();
            }
        }
    }
}
