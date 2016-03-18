using System;
using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    class Producer
    {
        public static void Main(string[] args)
        {
            var loadBalancerIp = args[0];
            var queueName = "clustertest";
            var factory = new ConnectionFactory() { HostName = loadBalancerIp };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var i = 0;
                while(++i>0)
                {
                    var message = string.Format("Message {0}", i);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true);

                    channel.BasicPublish(exchange: "",
                        routingKey: queueName,
                        basicProperties: properties,
                        body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
                    
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
