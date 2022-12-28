using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageManager;

namespace ReceiveAzureBusWorker.Middlewares
{
    internal static class IoC
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
