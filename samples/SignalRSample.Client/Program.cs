// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace SignalRSample.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var textWriter = Console.Out;
            var client = new Client(textWriter);

            client.RunAsync("http://localhost:5001/").Wait();

            Console.ReadKey();
        }
    }
}
