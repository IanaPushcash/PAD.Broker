using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lab1
{
	class Broker
	{
		private static readonly Broker instance = new Broker();

		private Broker()
		{
			QueueDictionary = new Dictionary<string, Queue<Message>>();
		}

		public static Broker GetInstance()
		{
			return instance;
		}
		public Dictionary<string, Queue<Message>> QueueDictionary { get; set; }

		public void AddMsg(Message msg)
		{
			if (!QueueDictionary.ContainsKey(msg.Name))
				QueueDictionary.Add(msg.Name, new Queue<Message>());
			QueueDictionary[msg.Name].Enqueue(msg);
		}

		public void GetAnswerMsg(Message msg, NetworkStream stream)
		{
			var retMsg = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
			stream.Write(retMsg, 0, retMsg.Length);
		}

		public void ProcessingMsg(Message msg, NetworkStream stream)
		{
			if (msg.TypeMsg != "die" && msg.TypeMsg != "willdie")
			{
				if (msg.IsSender)
					AddMsg(msg);
				else
				{
					if (!QueueDictionary.ContainsKey(msg.TypeMsg))
					{
						GetAnswerMsg(new Message() {IsSender = false, Msg = "Non-existend type msg", Name = "Server", TypeMsg = "Error"}, stream );
						return;
					}
					if (QueueDictionary[msg.TypeMsg].Count == 0)
					{
						GetAnswerMsg(new Message() { IsSender = false, Msg = "Queue is empty", Name = "Server", TypeMsg = "Error" }, stream);
						return;
					}
					GetAnswerMsg(QueueDictionary[msg.TypeMsg].Dequeue(), stream);
				}
			}
		}
	}
}
