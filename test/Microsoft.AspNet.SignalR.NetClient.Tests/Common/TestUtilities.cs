// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.SignalR.NetClient.Infrastructure;
using System;
using Xunit;

namespace Microsoft.AspNet.SignalR.NetClient.Tests.Common
{
    public static class TestUtilities
    {
        public static void AssertAggregateException(Action action, string message)
        {
            TestUtilities.AssertUnwrappedMessage<AggregateException>(action, message);
        }

        public static void AssertAggregateException<T>(Action action, string message)
        {
            TestUtilities.AssertUnwrappedException<AggregateException>(action, message, typeof(T));
        }

        public static void AssertUnwrappedMessage<T>(Action action, string message) where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                Assert.Equal(ex.Unwrap().Message, message);
            }
        }

        public static void AssertUnwrappedException<T>(Action action, string message, Type expectedExceptionType) where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                Exception unwrappedException = ex.Unwrap();

                Assert.IsType(expectedExceptionType, unwrappedException);
                Assert.Equal(message, unwrappedException.Message);
            }
        }

        public static void AssertUnwrappedException<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Exception unwrappedException = ex.Unwrap();

                Assert.IsType(typeof(T), unwrappedException);
            }
        }
    }
}

