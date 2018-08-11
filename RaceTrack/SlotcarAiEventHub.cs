using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace RaceTrack
{
    public class SlotcarAiEventHub : IDisposable
    {
        private EventHubClient eventHubClient;
        private const string EventHubConnectionString = "Endpoint=sb://slotcar-ai-events.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rpZPZJ56BYshGXRS2jGdsn+sV2ceYMyMOhLQWAuSGsM=";
        private const string EventHubName = "race-track";

        public SlotcarAiEventHub()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            Console.WriteLine("Connected to EventHub");
        }

        public void SendMessage(string message)
        {
            try
            {
                Console.WriteLine($"Sending message: { message }");
                eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
            }
            catch (Exception exception)
            {
                Console.WriteLine($" { DateTime.Now } > Exception: { exception.Message }");
            }
        }

        public void Dispose()
        {
            eventHubClient.CloseAsync();
        }
    }
}