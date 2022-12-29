using Azure.Messaging.ServiceBus;
using DataAccess.DAO;
using Entities.Domain.DTO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MessageManager
{
    public interface IBusLogic
    {
        /// <summary>
        /// metodo encargado de obtener la instancia de Services Bus Client de los services Worker 
        /// </summary>
        /// <param name="instanceFromWorkerService"></param>
        void SetInstance(ServiceBusClient instanceFromWorkerService);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageToServiceBus">El Vuelo nuevo registrado en la base de datos que tiene que ser enviado al Service Bus</param>
        /// <returns></returns>
        Task<bool> SendMessageAsync(object messageToServiceBus);

        /// <summary>
        /// Metodo encargado de obtener los registros de la tabla y enviarlos a azure service bus
        /// </summary>
        /// <returns></returns>
        Task GetNewRecords();

        /// <summary>
        /// metodo encargado de obtener los mensajes que se encuentran activos en la cola de AzureServiceBus
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task GetQueue(CancellationToken cancellationToken);
    }
    public class BusLogic : IBusLogic
    {
        private ServiceBusClient _serviceBusClient;
        private readonly ILogger<BusLogic> _logger;

        public BusLogic(ILogger<BusLogic> logger)
        {
            this._logger = logger;
        }

        public void SetInstance(ServiceBusClient instanceFromWorkerService)
        {
            if (this._serviceBusClient is null)
            {
                this._serviceBusClient = instanceFromWorkerService;
            }
        }

        public async Task<bool> SendMessageAsync(object messageToServiceBus)
        {
            if (this._serviceBusClient is not null)
            {
                ServiceBusSender sender = this._serviceBusClient.CreateSender("notificaciones");//queue name
                var body = System.Text.Json.JsonSerializer.Serialize(messageToServiceBus);
                var serviceBusMessage = new ServiceBusMessage(body);
                await sender.SendMessageAsync(serviceBusMessage);
                return true;
            }
            return false;
        }

        public async Task GetNewRecords()
        {
            List<VueloDTO> lista = AzureDAO.GetNewRecordsFromDatabase().Result;

            if (lista is not null && lista.Count > 0)
            {
                foreach (var item in lista)
                {
                    this._logger.LogInformation($"VUELO NUMERO #{item.NumeroVuelo} REGISTRADO EN EL DIA {DateTime.Now.ToShortDateString()}");
                    await SendMessageAsync(item);
                }
            }
            else
            {
                this._logger.LogInformation("SIN VUELOS PENDIENTES");
            }
        }

        public async Task GetQueue(CancellationToken cancellationToken)
        {
            ServiceBusReceiver receiver = this._serviceBusClient.CreateReceiver("notificaciones");
            ServiceBusReceivedMessage message;
            while ((message = await receiver.ReceiveMessageAsync(TimeSpan.FromMilliseconds(1000), cancellationToken)) is not null)
            {
                var jsonString = message.Body.ToString();
                VueloDTO messageToModel = JsonConvert.DeserializeObject<VueloDTO>(jsonString)!;
                if (await AzureDAO.SetRecordsFromServiceBus(messageToModel))
                {
                    await receiver.CompleteMessageAsync(message);
                    this._logger.LogInformation($"VUELO PENDIENTE DE REGISTRO, NUMERO DE VUELO: ${messageToModel.NumeroVuelo} DESDE AZURE SERVICE BUS REGISTRANDO... {DateTime.Now.ToShortDateString()}");
                }
            }
            if (message is null)
            {
                this._logger.LogInformation($"SIN VUELOS PENDIENTES, DEDSDE AZURE SERVICE BUS");
            }
        }
    }
}