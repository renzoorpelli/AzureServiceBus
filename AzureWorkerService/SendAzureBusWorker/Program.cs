using System.Configuration;
using System;
using Azure.Messaging.ServiceBus;
using SendAzureBusWorker.Middleware;
using SendAzureBusWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AzureBussConnection"].ConnectionString;
        
        //agrego servicios y conexion al service bus
        IoC.AddDependency(services, connectionString);

        services.AddHostedService<Worker>();

    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
