using Azure.Messaging.ServiceBus;
using MessageManager;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ReceiveAzureBusWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusLogic _busLogic;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker(ILogger<Worker> logger, IBusLogic busLogic, ServiceBusClient serviceBusClient, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            this._busLogic = busLogic;
            this._serviceBusClient = serviceBusClient;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PeriodicTimer timer = new(TimeSpan.FromMilliseconds(Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["timer"]!)));
            this._busLogic.SetInstance(this._serviceBusClient);

            try
            {
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await this._busLogic.GetQueue(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);
                this._logger.LogInformation(ex.Message);
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
            
        }
    }
}