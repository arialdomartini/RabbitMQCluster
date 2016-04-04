using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Client.Framing.Impl;
using System.Threading;

namespace ReliableConsumer
{
    public class BasicConsumer : DefaultBasicConsumer
	{
        readonly IModel _channel;
        readonly bool _crash = true;

        MainClass _manager;

        public BasicConsumer(IModel channel, bool crash, MainClass manager) : base (channel)
        {
            _manager = manager;
            _crash = crash;
            _channel = channel;
        }


        public override void HandleBasicCancel(string consumerTag)
        {
            Console.WriteLine("HandleBasicCancel");
        }

        public override void HandleBasicCancelOk(string consumerTag)
        {
            base.HandleBasicCancelOk(consumerTag);
            Console.Write("BasicCancelOk. Sleeping...");
            Thread.Sleep(1000);
            Console.WriteLine("Restarted!");

            _manager.Subscribe(_channel, shouldCrash: false);
        }

        public override void HandleBasicConsumeOk(string consumerTag)
        {
            base.HandleBasicConsumeOk(consumerTag);
            Console.WriteLine("BasicConsumeOk");

        }


        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            try
            {
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("BasicConsumer: received '{0}':'{1}'", routingKey, message);
                if(_crash == true && int.Parse(message) % 10 == 0)
                {
                    throw new Exception("An unhandled exception");
                }
                _channel.BasicAck(deliveryTag, false);
                    
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                _channel.BasicCancel(ConsumerTag);
                _channel.BasicReject(deliveryTag, true);

                return;
            }
        }
	}

}
