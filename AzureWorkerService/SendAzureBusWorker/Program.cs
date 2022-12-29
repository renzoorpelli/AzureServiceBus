using SendAzureBusWorker;
using SendAzureBusWorker.Middleware;
using Serilog;

Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .WriteTo.File(@"C:\Users\Renzo\Desktop\wkReceiverLogFile.txt")
               .CreateLogger();


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
