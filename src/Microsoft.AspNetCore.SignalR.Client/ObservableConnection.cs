﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.SignalR.Client.Infrastructure;

namespace Microsoft.AspNetCore.SignalR.Client
{
    public class ObservableConnection<T> : IObservable<T>
    {
        private readonly Connection _connection;
        private readonly Func<string, T> _convert;

        public ObservableConnection(Connection connection, Func<string, T> convert)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (convert == null)
            {
                throw new ArgumentNullException(nameof(convert));
            }

            _convert = convert;
            _connection = connection;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Action<string> received = data =>
            {
                observer.OnNext(_convert(data));
            };

            Action closed = () =>
            {
                observer.OnCompleted();
            };

            Action<Exception> error = ex =>
            {
                observer.OnError(ex);
            };

            _connection.Received += received;
            _connection.Closed += closed;
            _connection.Error += error;

            return new DisposableAction(() =>
            {
                _connection.Received -= received;
                _connection.Closed -= closed;
                _connection.Error -= error;
            });
        }
    }
}
