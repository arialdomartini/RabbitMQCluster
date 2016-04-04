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

            var queueName = "foo";
            channel.QueueDelete(queueName);
            channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            channel.QueueBind(queueName, exchange, "foo");

            //var eventingConsumer = new EventingConsumer(channel);
            //channel.BasicConsume(queueName, false, eventingConsumer);


            var basicConsumer = new BasicConsumer(channel, crash: true);
            channel.BasicConsume(queueName, false, basicConsumer);
            //basicConsumer.ConsumerTag = tag;

            Console.ReadLine();
        }

    }
}
