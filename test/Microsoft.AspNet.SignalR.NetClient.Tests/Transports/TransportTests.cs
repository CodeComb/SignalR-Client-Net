// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Microsoft.AspNet.SignalR.NetClient.Transports;
using Moq;
using Xunit;

namespace Microsoft.AspNet.SignalR.NetClient.Tests.Transports
{
    public class TransportTests
    {
        [Fact]
        public void VerifyLastActiveSetsLastErrorIfConnectionExpired()
        {
            var mockConnection = new Mock<IConnection>();

            mockConnection.Setup(c => c.LastActiveAt).Returns(new DateTime(1));
            mockConnection.Setup(c => c.ReconnectWindow).Returns(new TimeSpan(42));

            var connection = mockConnection.Object;

            Assert.False(TransportHelper.VerifyLastActive(connection));

            var expectedMessage =
                string.Format(CultureInfo.CurrentCulture, Resources.Error_ReconnectWindowTimeout,
                    connection.LastActiveAt, connection.ReconnectWindow);

            mockConnection.Verify(c => c.Stop(It.Is<TimeoutException>(e => e.Message == expectedMessage)));
        }
    }
}
