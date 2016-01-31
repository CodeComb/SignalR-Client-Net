// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SignalR.Client.Http
{
    public static class HttpClientExtensions
    {
        public static Task<IResponse> Post(this IHttpClient client, string url, Action<IRequest> prepareRequest, bool isLongRunning)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (prepareRequest == null)
            {
                throw new ArgumentNullException(nameof(prepareRequest));
            }

            return client.Post(url, prepareRequest, postData: null, isLongRunning: isLongRunning);
        }
    }
}
