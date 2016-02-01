// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.AspNetCore.SignalR.Client.Http
{
#if !DOTNET5_4
    public class DefaultHttpHandler : WebRequestHandler
#else
    public class DefaultHttpHandler : HttpClientHandler
#endif
    {
        private readonly IConnection _connection;

        public DefaultHttpHandler(IConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connection = connection;

            Credentials = _connection.Credentials;

            PreAuthenticate = true;

            if (_connection.CookieContainer != null)
            {
                CookieContainer = _connection.CookieContainer;
            }

            if (_connection.Proxy != null)
            {
                Proxy = _connection.Proxy;
            }

#if NET451
            foreach (X509Certificate cert in _connection.Certificates)
            {
                ClientCertificates.Add(cert);
            }
#endif
        }
    }
}
