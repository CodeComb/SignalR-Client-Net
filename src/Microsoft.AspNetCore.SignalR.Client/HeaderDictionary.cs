﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.AspNetCore.SignalR.Client
{
    public class HeaderDictionary : IDictionary<string, string>
    {
        private readonly IDictionary<string, string> _dictionary = new Dictionary<string, string>();
        private readonly IConnection _connection;

        public HeaderDictionary(IConnection connection)
        {
            _connection = connection;
        }

        public void Add(string key, string value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<string> Values => _dictionary.Values;

        public string this[string key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _connection.State != ConnectionState.Disconnected || _dictionary.IsReadOnly;

        public bool Remove(KeyValuePair<string, string> item)
        {
            return _dictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
