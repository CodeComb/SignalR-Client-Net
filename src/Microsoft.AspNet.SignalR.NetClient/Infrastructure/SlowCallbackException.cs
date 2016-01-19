// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;

namespace Microsoft.AspNet.SignalR.NetClient.Infrastructure
{
#if NET45
    [Serializable]
#endif
    public class SlowCallbackException : Exception
    {
        public SlowCallbackException() { }
        public SlowCallbackException(string message) : base(message) { }
        public SlowCallbackException(string message, Exception inner) : base(message, inner) { }

#if NET45
        protected SlowCallbackException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
#endif
    }
}
