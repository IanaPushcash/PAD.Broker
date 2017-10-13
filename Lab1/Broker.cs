using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Lab1
{
	class Broker
	{
		private static readonly Broker instance = new Broker();
		private static ListDictionary queueDictionary;
		public static List<Client> Subscribers { get; set; }
		//public static Dictionary<Client, DateTime> Publishers { get; set; }
		private Broker()
		{
			queueDictionary = new ListDictionary();
			Subscribers = new List<Client>();
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

					//if (!QueueDictionary.ContainsKey(msg.TypeMsg))
					//{
					//	GetAnswerMsg(new Message() {IsSender = false, Msg = "Non-existend type msg", Name = "Server", TypeMsg = "Error"}, stream );
					//	return;
					//}
					//if (QueueDictionary[msg.TypeMsg].Count == 0)
					//{
					//	GetAnswerMsg(new Message() { IsSender = false, Msg = "Queue is empty", Name = "Server", TypeMsg = "Error" }, stream);
					//	return;
					//}
					//Message m = new Message();
					//QueueDictionary[msg.TypeMsg].TryDequeue(out m);
					//while (true)
					//{
						queueDictionary.SendMessageSub(msg, stream);
						//GetAnswerMsg(new Message() { Msg = "Hello" }, stream);
					//}
				}
					
			}
			else if (msg.TypeMsg == "willdie")
				queueDictionary.SendWillDieMessage(msg, stream);
			else if (msg.TypeMsg == "die")
			{
				queueDictionary.SendDieMessage(msg, stream);
			}
		}

		
		public static void Dispose()
		{
			queueDictionary.Dispose();
		}
	}
}
