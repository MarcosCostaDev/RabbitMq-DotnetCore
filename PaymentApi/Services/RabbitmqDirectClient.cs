using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentApi.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentApi.Services
{

    public interface IRabbitmqDirectClient
    {
        string MakePayment(CardPayment cardPayment);
    }
    public class RabbitmqDirectClient : IRabbitmqDirectClient, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly DefaultBasicConsumer _consumer;

        public RabbitmqDirectClient(IConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(configuration.GetConnectionString("rabbitmq"))
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _replyQueueName = _channel.QueueDeclare("rpc_reply", true, false, false, null);
           // _channel.BasicQos(0, 10, false);
            _consumer = new DefaultBasicConsumer(_channel);
            _channel.DefaultConsumer = _consumer;
        }


        public void Dispose()
        {
           
        }

        public string MakePayment(CardPayment cardPayment)
        {
            var corrId = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = _replyQueueName;
            props.CorrelationId = corrId;
            var serialized = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(cardPayment));
            _channel.BasicPublish("", "rpc_queue", props, serialized);

           

            while(true)
            {
                var ea = _channel.BasicGet(_replyQueueName, false);

                if (ea?.BasicProperties.CorrelationId != corrId) continue;

                var authCode = Encoding.UTF8.GetString(ea.Body.ToArray());
                _channel.BasicAck(ea.DeliveryTag, true);
                return authCode;
            }

            
        }

    }
}
