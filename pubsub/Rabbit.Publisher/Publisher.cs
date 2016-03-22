using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Rabbit.Publisher
{
    public class Publisher
    {
        private string exchange = "testexchange";

        public void Run(string hostName)
        {
            var connectionFactory = new ConnectionFactory {HostName = hostName, AutomaticRecoveryEnabled = true, 
                NetworkRecoveryInterval = TimeSpan.FromMilliseconds(100)};
            
            var connection = connectionFactory.CreateConnection();

            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, "direct");

            PublishMessages(channel);

            Shutdown();
        }

        private void PublishMessages(IModel channel)
        {
            var routingkey = "foo";
            var i = 0;
            while (++i > 0)
            {
                var published = false;
                while (!published)
                {
                    try
                    {
                        var message = string.Format("Message {0}", i);

                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange, routingkey, null, body);
                        //channel.WaitForConfirmsOrDie();
                        Console.Write(" [x] Sending '{0}':'{1}'...", routingkey, message);
                        Console.WriteLine("Confirmed!");
                        published = true;
                    } catch (AlreadyClosedException e)
                    {
                        Console.WriteLine(" Publishing failed, retrying...");
                        Thread.Sleep(30);
                    }
                    Thread.Sleep(300);
                }
            }
        }

        private void Shutdown()
        {
            Console.WriteLine("  Shutting down");
        }

    }
}