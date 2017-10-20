using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading;

namespace Lab1
{
	class Broker
	{
		private static readonly Broker instance = new Broker();
		private static ListDictionary queueDictionary;
		public static List<Client> Subscribers { get; set; }
		public static List<Client> Publishers { get; set; }
		//public static Dictionary<Client, DateTime> Publishers { get; set; }
		private Broker()
		{
			queueDictionary = new ListDictionary();
			Subscribers = new List<Client>();
			Publishers = new List<Client>();

			Task checkPublishers = new Task(() =>
			{
				while (true)
				{
					if (Publishers.Count > 0)
						for (int i = 0; i < Publishers.Count; i++)
						{
							try
							{
								var retMsg = Encoding.Unicode.GetBytes("Check");
								//stream.Write(retMsg, 0, retMsg.Length);
								Publishers[i].client.GetStream().Write(retMsg, 0, retMsg.Length);
							}
							catch
							{
								ListDictionary.SendDieMessage(new Message() {Name = Publishers[i].ClientName});
								Publishers.Remove(Publishers[i--]);
							}
						}
					Thread.Sleep(15000);
				}

			});
			checkPublishers.Start();
			//Publishers = new Dictionary<Client, DateTime>();
		}

		public static Broker GetInstance()
		{
			return instance;
		}

		public void ProcessingMsg(Message msg, NetworkStream stream)
		{
			if (msg.TypeMsg != "die" && msg.TypeMsg != "willdie")
			{
				if (msg.IsSender)
					queueDictionary.AddMessage(msg);
				else
				{
					
						queueDictionary.SendMessageSub(msg, stream);
				}
					
			}
			else if (msg.TypeMsg == "willdie")
				queueDictionary.SendWillDieMessage(msg);
			else if (msg.TypeMsg == "die")
			{
				ListDictionary.SendDieMessage(msg);
			}
		}

		
		public static void Dispose()
		{
			queueDictionary.Dispose();
		}
	}
}
