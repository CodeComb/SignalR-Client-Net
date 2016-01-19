// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.SignalR.NetClient.Infrastructure;

namespace Microsoft.AspNet.SignalR.NetClient.Hubs
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