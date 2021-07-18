using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectPaymentCardConsumer
{
    public class RabbitmqConsumer : IDisposable
    {
        private const string ExchangeName = "";
        private const string RpcQueueName = "rpc_queue";

        private const string connectionString = "amqp://guest:guest@localhost:5672/";
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private Random _rnd;

        public void CreateConnection()
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();

        }

        public void ProcessMessages()
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(RpcQueueName, true, false, false, null);

            _channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(RpcQueueName, false, consumer);
            _rnd = new Random();
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                string response = null;
                if (e == null) return;
                var props = e.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                Console.WriteLine("----------------------------------------------------------");

                try
                {
                    response = MakePayment(e);
                    Console.WriteLine("Correlation ID = {0}", props.CorrelationId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" ERROR : " + ex.Message);
                    response = "";
                }
                finally
                {
                    if (response != null)
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        _channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
                    }
                    _channel.BasicAck(e.DeliveryTag, false);
                }

                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine("");
            }
            catch (Exception)
            {
                _channel.BasicReject(e.DeliveryTag, true);
            }
        }

        private string MakePayment(BasicDeliverEventArgs e)
        {
            var content = Encoding.UTF8.GetString(e.Body.ToArray());

            var message = JsonConvert.DeserializeObject<CardPayment>(content);

            var response = _rnd.Next(1000, 100000000).ToString(CultureInfo.InvariantCulture);
            Console.WriteLine("Payment -  {0} : £{1} : Auth Code <{2}> ", message.CardNumber, message.Amount, response);

            return response;
        }
    }
}
