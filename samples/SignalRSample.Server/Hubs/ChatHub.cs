using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.Extensions.Logging;

namespace SignalRSample.Server.Hubs
{
    [HubName("ChatHub")]
    public class Chat : Hub
    {
        private readonly ILogger _logger;

        public Chat(ILogger<Chat> logger)
        {
            _logger = logger;
        }

        public void Send(string message)
        {
            _logger.LogInformation($"Message '{message}' is sent to the client with the ID: {Context?.ConnectionId}");
            Clients.All.send(message);
        }

        public override Task OnConnected()
        {
            _logger.LogInformation($"Connected client with ID: {Context?.ConnectionId}");
            return base.OnConnected();
        }
    }
}