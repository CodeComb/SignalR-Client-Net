﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Microsoft.AspNetCore.SignalR.Client.Infrastructure
{
    [SuppressMessage("Microsoft.Design", "CA1032:Add constructor", Justification = "Silverlight does not support Serialization")]
    [SuppressMessage("Microsoft.Usage", "CA2237:Add serializable", Justification = "Silverlight does not support Serialization")]
    public class HttpClientException : Exception
    {
        public HttpResponseMessage Response { get; private set; }

        public HttpClientException()
            : base()
        {
        }

        public HttpClientException(string message)
            : base(message)
        {
        }

        public HttpClientException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public HttpClientException(HttpResponseMessage responseMessage)
            : this(GetExceptionMessage(responseMessage))
        {
            Response = responseMessage;
        }

        private static string GetExceptionMessage(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException("responseMessage");
            }

            return responseMessage.ToString();
        }
    }
}