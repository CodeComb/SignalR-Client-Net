// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Net.Http;

#if !NETFX_CORE && !PORTABLE && !DOTNET5_4
using System.Security.Cryptography.X509Certificates;
#endif

namespace Microsoft.AspNet.SignalR.NetClient.Http
{
#if !NETFX_CORE && !PORTABLE && !__ANDROID__ && !IOS && !DOTNET5_4
    public class DefaultHttpHandler : WebRequestHandler
#else
    public class DefaultHttpHandler : HttpClientHandler
#endif
    {
        private readonly IConnection _connection;

        public DefaultHttpHandler(IConnection connection)
        {
            if (connection != null)
            {
                _connection = connection;
            }
            else
            {
                throw new ArgumentNullException("connection");
            }

            Credentials = _connection.Credentials;
#if PORTABLE
            if (this.SupportsPreAuthenticate())
            {
                PreAuthenticate = true;
            }
#elif NET45 || DOTNET5_4
            PreAuthenticate = true;
#endif

            if (_connection.CookieContainer != null)
            {
                CookieContainer = _connection.CookieContainer;
            }

#if !PORTABLE
            if (_connection.Proxy != null)
            {
                Proxy = _connection.Proxy;
            }
#endif

#if NET45
            foreach (X509Certificate cert in _connection.Certificates)
            {
                ClientCertificates.Add(cert);
            }
#endif
        }
    }
}
