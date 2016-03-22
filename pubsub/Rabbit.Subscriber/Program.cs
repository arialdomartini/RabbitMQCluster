using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ReliableConsumer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var hostName = args[0];
         
            var connectionFactory = new ConnectionFactory {HostName = hostName, AutomaticRecoveryEnabled = true, 
                NetworkRecoveryInterval = TimeSpan.FromMilliseconds(100)};

            var connection = connectionFactory.CreateConnection();

            var channel = connection.CreateModel();

            var exchange = "testexchange";
            channel.ExchangeDeclare(exchange, "direct");

            var queueName = channel.QueueDeclare().QueueName;

            channel.ModelShutdown += (sender, eventArgs) =>
            {
                Console.WriteLine("  >>>>>>>> Model shutdown!");

            };

            channel.QueueBind(queueName, exchange, "foo");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                ConsumeMessage(channel, ea);
            };
            channel.BasicConsume(queueName, false, consumer);    



            Console.ReadLine();
        }

        static void ConsumeMessage(IModel channel, BasicDeliverEventArgs ea)
        {
            try
            {

                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                Console.WriteLine("Reliable consumer: received '{0}':'{1}'", routingKey, message);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            }
            catch (Exception e)
            {
                channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                Console.WriteLine("     Exception in reliable consumer");
            }
        }
    }
}
