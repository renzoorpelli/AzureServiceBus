using Azure.Messaging.ServiceBus;
using MessageManager;

namespace SendAzureBusWorker.Middleware
{
    public static class IoC
    {
        public static IServiceCollection AddDependency(this IServiceCollection services, string connectionString)
        {
            
            services.AddSingleton(sv => new ServiceBusClient(connectionString, new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            }));

            services.AddSingleton<IBusLogic, BusLogic>();

            return services;
        }
    }
}
