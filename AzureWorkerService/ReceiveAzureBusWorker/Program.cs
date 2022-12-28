using ReceiveAzureBusWorker;
using ReceiveAzureBusWorker.Middlewares;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AzureBussConnection"].ConnectionString;

        IoC.AddDependency(services, connectionString);
        services.AddHostedService<Worker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
