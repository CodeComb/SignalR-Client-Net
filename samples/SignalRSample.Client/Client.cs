using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client.Infrastructure;

namespace SignalRSample.Client
{
    public class Client
    {
        private readonly TextWriter _textWriter;

        public Client(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        public async Task RunAsync(string url)
        {
            try
            {
                var hubConnection = new HubConnection(url);
                var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");

                chatHubProxy.On<string>("send", message => _textWriter.Write($"Received server: '{message}'"));

                await hubConnection.Start();
                _textWriter.WriteLine($"Connected to server: {url}");

                _textWriter.WriteLine("Sent server: 'Hello World!'");
                await chatHubProxy.Invoke("Send", "Hello World!");
            }
            catch (HttpClientException httpClientException)
            {
                _textWriter.WriteLine($"HttpClientException: {httpClientException.Response}");
            }
            catch (Exception exception)
            {
                _textWriter.WriteLine($"Exception: {exception}");
            }
        }
    }
}