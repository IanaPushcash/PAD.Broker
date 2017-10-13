using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lab1
{
	class ListDictionary:IDisposable
	{
		private ConcurrentDictionary<string, List<Message>> qDictionary;

		private readonly string fileName = "ListDictionary.txt";

		public ListDictionary()
		{
			qDictionary = new ConcurrentDictionary<string, List<Message>>();
			DeserializeQueue(fileName);
		}

		public void AddMessage(Message msg)
		{
			if (!qDictionary.ContainsKey(msg.Name))
				qDictionary.TryAdd(msg.Name, new List<Message>());
			qDictionary[msg.Name].Add(msg);
			SendMessage(ref msg);
		}

		private void TrySendToSubscribers(List<Client> subs)
		{
			throw new NotImplementedException();
		}

		public bool SendMessageSub(Message msg, NetworkStream stream)
		{
			if (!existName(msg.Name) || !existType(msg.Name,msg.TypeMsg))
				GetAnswerMsg(new Message() { IsSender = false, Msg = "Non-existend type msg", Name = "Server", TypeMsg = "Error" }, stream);
			while (true)
			{
				if (msg.Name != "" && msg.TypeMsg != "" && existName(msg.Name) && existType(msg.Name, msg.TypeMsg))
				{
					var mes = qDictionary[msg.Name].Last(m => m.TypeMsg == msg.TypeMsg);
					SendMessage(ref mes);
					continue;
				}
				if (msg.Name != "" && msg.TypeMsg == "" && existName(msg.Name) && qDictionary[msg.Name].Count > 0)
				{
					var mes = qDictionary[msg.Name].Last();
					SendMessage(ref mes);
					continue;
				}
				if (msg.Name == "" && msg.TypeMsg != "" && existType(msg.Name, msg.TypeMsg))
				{
					foreach (var key in qDictionary.Keys)
					{
						if (qDictionary[key].Any(q => q.TypeMsg == msg.TypeMsg))
						{
							var mes = qDictionary[key].Last(q => q.TypeMsg == msg.TypeMsg);
							SendMessage(ref mes);
							continue;
						}
					}
				}
				if (msg.Name == "" && msg.TypeMsg == "" && existName(msg.Name) && existType(msg.Name, msg.TypeMsg))
				{
					foreach (var key in qDictionary.Keys)
					{
						if (qDictionary[key].Any())
						{
							var mes = qDictionary[key].Last();
							SendMessage(ref mes);
							continue;
						}
					}
				}

			}
		}

		public void SendMessage(ref Message msg)
		{
			var r = false;
			foreach (var subscriber in Broker.Subscribers)
			{
				if ((subscriber.TargetAuthor == "" || subscriber.TargetAuthor == msg.Name) &&
				    (subscriber.TargetType == "" || subscriber.TargetType == msg.TypeMsg))
				{
					GetAnswerMsg(msg, subscriber.client.GetStream());
					r = true;
				}
			}
			if (r) qDictionary[msg.Name].Remove(msg);
		}

		private bool existType(string msgName, string msgTypeMsg)
		{
			if (msgTypeMsg == "" && qDictionary.Keys.Count > 0 && qDictionary.Any(l=>l.Value.Count>0)) return true;
			if (msgName != "" && existName(msgName)) return qDictionary[msgName].Any(q => q.TypeMsg == msgTypeMsg);
			if (msgName == "") return existName(msgName);
			foreach (var key in qDictionary.Keys)
			{
				if (qDictionary[key].Any(q => q.TypeMsg == msgTypeMsg))
					return true;
			}
			return false;
		}

		private bool existName(string msgName)
		{
			return qDictionary.Keys.Contains(msgName) || (msgName == "" && qDictionary.Keys.Count>0);
		}

		public void GetAnswerMsg(Message msg, NetworkStream stream)
		{
			var retMsg = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
			stream.Write(retMsg, 0, retMsg.Length);
		}

		public void SerializeQueue(string fileName)
		{
			var str = JsonConvert.SerializeObject(qDictionary);
			File.WriteAllText(fileName, str);

		}

		public void DeserializeQueue(string fileName)
		{
			var str = File.ReadAllText(fileName);
			qDictionary = JsonConvert.DeserializeObject<ConcurrentDictionary<string, List<Message>>>(str);
		}
		
		public void Dispose()
		{
			SerializeQueue(fileName);
		}
	}
}
