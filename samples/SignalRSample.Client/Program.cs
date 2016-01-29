using System;

namespace SignalRSample.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var textWriter = Console.Out;
            var client = new Client(textWriter);

            client.RunAsync("http://localhost:5000/").Wait();

            Console.ReadKey();
        }
    }
}
