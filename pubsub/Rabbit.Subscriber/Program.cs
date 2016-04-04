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

        string _queueName;

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

            _queueName = "foo";
            channel.QueueDelete(_queueName);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Subscribe(channel, shouldCrash: true);


            Console.ReadLine();
        }

        public void NotifyDeath(IModel channel)
        {
            Subscribe(channel, false);
        }

        public void Subscribe(IModel channel, bool shouldCrash)
        {
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(_queueName, _exchange, "foo");
            var basicConsumer = new BasicConsumer(channel, shouldCrash, this);
            channel.BasicConsume(_queueName, false, basicConsumer);
        }
    }
}
