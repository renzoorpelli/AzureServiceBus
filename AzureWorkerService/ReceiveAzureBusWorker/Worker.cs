using Azure.Messaging.ServiceBus;
using MessageManager;

namespace ReceiveAzureBusWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusLogic _busLogic;
        private readonly ServiceBusClient _serviceBusClient;

        public Worker(ILogger<Worker> logger, IBusLogic busLogic, ServiceBusClient serviceBusClient)
        {
            _logger = logger;
            this._busLogic = busLogic;
            this._serviceBusClient = serviceBusClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PeriodicTimer timer = new(TimeSpan.FromMilliseconds(Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["timer"]!)));
            this._busLogic.SetInstance(this._serviceBusClient);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await this._busLogic.GetQueue(stoppingToken);
            }
        }
    }
}