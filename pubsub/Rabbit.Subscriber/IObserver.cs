using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQ.Util;

namespace ReliableConsumer
{
	public interface IObserver 
	{
        void NotifyDeath(IModel channel);
	}

}
