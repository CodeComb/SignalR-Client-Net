﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;
using Moq;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client.Infrastructure;
using Microsoft.AspNetCore.Testing.xunit;

namespace Microsoft.AspNetCore.SignalR.Client.Tests.Infrastructure
{
    public class TaskQueueMonitorTests
    {
        private static readonly string _expectedErrorMessage =
            string.Format(CultureInfo.CurrentCulture,
                          Resources.Error_PossibleDeadlockDetected,
                          Timeout.InfiniteTimeSpan.TotalSeconds);

        [Fact]
        public void ErrorsAreTriggeredForLongRunningTasks()
        {
            VerifyErrorCount(Times.Once(), monitor =>
            {
                monitor.TaskStarted();

                monitor.Beat();
                monitor.Beat();
            });
        }

        [Fact]
        public void ErrorsAreNotTriggeredMultipleTimesForTheSameTask()
        {
            VerifyErrorCount(Times.Once(), monitor =>
            {
                monitor.TaskStarted();

                monitor.Beat();
                monitor.Beat();
                monitor.Beat();
            });
        }

        [Fact]
        public void MultipleErrorsAreTriggeredForMultipleLongRunningTasks()
        {
            VerifyErrorCount(Times.Exactly(2), monitor =>
            {
                monitor.TaskStarted();

                monitor.Beat();
                monitor.Beat();

                monitor.TaskCompleted();
                monitor.TaskStarted();

                monitor.Beat();
                monitor.Beat();
            });
        }

        [Fact]
        public void ErrorsAreNotTriggeredBeforeATaskStarts()
        {
            VerifyErrorCount(Times.Never(), monitor =>
            {
                monitor.Beat();
                monitor.Beat();
            });
        }

        [Fact]
        public void ErrorsAreNotTriggeredForShortRunningTasks()
        {
            VerifyErrorCount(Times.Never(), monitor =>
            {
                monitor.TaskStarted();

                monitor.Beat();

                monitor.TaskCompleted();
                monitor.TaskStarted();
                monitor.TaskCompleted();
                monitor.TaskStarted();

                monitor.Beat();

                monitor.TaskCompleted();

                monitor.Beat();
                monitor.Beat();
            });
        }

        [Fact(Skip = "Bug CI test (DNXCORE50)")]
        public void ErrorsAreTriggeredByTimer()
        {
            var mockConnection = new Mock<IConnection>();
            var wh = new ManualResetEventSlim();

            mockConnection.Setup(c => c.OnError(It.IsAny<SlowCallbackException>())).Callback(wh.Set);

            using (var monitor = new TaskQueueMonitor(mockConnection.Object, TimeSpan.FromMilliseconds(100)))
            {
                monitor.TaskStarted();
                Assert.True(wh.Wait(TimeSpan.FromMilliseconds(500)));
            };
        }

        [Fact]
        public void ErrorsAreNotTriggeredByTimerAfterDisposal()
        {
            var mockConnection = new Mock<IConnection>();
            var wh = new ManualResetEventSlim();

            mockConnection.Setup(c => c.OnError(It.IsAny<SlowCallbackException>())).Callback(wh.Set);

            using (var monitor = new TaskQueueMonitor(mockConnection.Object, TimeSpan.FromMilliseconds(100)))
            {
                monitor.TaskStarted();
            };

            Assert.False(wh.Wait(TimeSpan.FromMilliseconds(500)));
        }

        private static void VerifyErrorCount(Times count, Action<TaskQueueMonitor> test)
        {
            var connection = Mock.Of<IConnection>();

            using (var monitor = new TaskQueueMonitor(connection, Timeout.InfiniteTimeSpan))
            {
                test(monitor);
            }

            Mock.Get(connection)
                .Verify(c => c.OnError(It.Is<SlowCallbackException>(e => e.Message == _expectedErrorMessage)), count);
        }
    }
}
