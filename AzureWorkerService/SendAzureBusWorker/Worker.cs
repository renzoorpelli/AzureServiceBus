using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MessageManager;
using Azure.Messaging.ServiceBus;

namespace SendAzureBusWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusLogic _busLogic;
        private readonly ServiceBusClient _serviceBusClient;

        public Worker(ILogger<Worker> logger, IBusLogic busLogic, ServiceBusClient serviceBusClient)
        {
            this._logger = logger;
            this._busLogic = busLogic;
            this._serviceBusClient = serviceBusClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PeriodicTimer timer = new(TimeSpan.FromMilliseconds(Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["timer"]!)));
            this._busLogic.SetInstance(this._serviceBusClient);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                this._logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await this._busLogic.GetNewRecords();
            }
        }
    }
}
