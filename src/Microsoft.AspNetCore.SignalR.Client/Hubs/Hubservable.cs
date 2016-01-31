﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR.Client.Infrastructure;

namespace Microsoft.AspNetCore.SignalR.Client.Hubs
{
    /// <summary>
    /// <see cref="T:System.IObservable{object[]}"/> implementation of a hub event.
    /// </summary>
    public class Hubservable : IObservable<IList<JToken>>
    {
        private readonly string _eventName;
        private readonly IHubProxy _proxy;

        public Hubservable(IHubProxy proxy, string eventName)
        {
            _proxy = proxy;
            _eventName = eventName;
        }

        public IDisposable Subscribe(IObserver<IList<JToken>> observer)
        {
            var subscription = _proxy.Subscribe(_eventName);
            subscription.Received += observer.OnNext;

            return new DisposableAction(() =>
            {
                subscription.Received -= observer.OnNext;
            });
        }
    }
}