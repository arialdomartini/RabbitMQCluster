using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Util;

namespace ReliableConsumer
{
    public class MainClass
    {
        static string _exchange;

        public static void Main(string[] args)
        {
            var hostName = args[0];

            new MainClass().Run(hostName);
        }

        public void Run(string hostName)
        {

            var connectionFactory = new ConnectionFactory {HostName = hostName, AutomaticRecoveryEnabled = true, 
                NetworkRecoveryInterval = TimeSpan.FromMilliseconds(100)};

            var connection = connectionFactory.CreateConnection();

            var channel = connection.CreateModel();

            _exchange = "testexchange";
            channel.ExchangeDeclare(_exchange, "direct");
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Subscribe(channel);


            Console.ReadLine();
        }

        public void Subscribe(IModel channel)
        {
            const string queueName = "foo";
            channel.QueueDelete(queueName);
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queueName, _exchange, "foo");
            var basicConsumer = new BasicConsumer(channel, true, this);
            channel.BasicConsume(queueName, false, basicConsumer);
        }
    }
}
