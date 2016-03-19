using System;
using RabbitMQ.Client;
using System.Text;
using System.Threading;

namespace Producer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var loadBalancerIp = args[0];

            var producer = new Producer(new ChannelFactory(loadBalancerIp));
            producer.Run();
        }
    }

    class ChannelFactory
    {
        readonly string _serverIp;
        readonly ConnectionFactory _connectioFactory;
        IConnection _connection;
        IModel _channel;

        public ChannelFactory(string serverIp)
        {
            _serverIp = serverIp;
            _connectioFactory = new ConnectionFactory() { HostName = _serverIp };

        }

        public IModel GetChannel()
        {
            _connection = _connectioFactory.CreateConnection();
            _channel = _connection.CreateModel();
            return _channel;
        }
    }

    class Producer
    {
        readonly string queueName = "clustertest";
        ChannelFactory _channelFactory;

        public Producer(ChannelFactory channelFactory)
        {
            this._channelFactory = channelFactory;
        }

        IModel Setup()
        {
            var channel = _channelFactory.GetChannel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            return channel;
        }

        void PublishMessages(IModel channel)
        {
            var i = 0;
            while (++i > 0)
            {
                var message = string.Format("Message {0}", i);
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.SetPersistent(true);
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                Thread.Sleep(300);
            }
        }

        public void Run()
        {
            var channel = Setup();
            PublishMessages(channel);
        }
    }
}
