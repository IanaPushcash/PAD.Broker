using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lab1
{
	class QueueDictionary:IDisposable
	{
		private ConcurrentDictionary<string, ConcurrentQueue<Message>> qDictionary;

		private readonly string fileName = "QueueDictionary.txt";

		public QueueDictionary()
		{
			qDictionary = new ConcurrentDictionary<string, ConcurrentQueue<Message>>();
			DeserializeQueue(fileName);
		}

		public bool AddMessage(Message msg)
		{
			var ret = true;
			if (!qDictionary.ContainsKey(msg.Name))
				ret = qDictionary.TryAdd(msg.Name, new ConcurrentQueue<Message>());
			if (ret)
				qDictionary[msg.Name].Enqueue(msg);
			return ret;
		}

		public bool SendMessage()
		{
			return false;
		}

		public void SerializeQueue(string fileName)
		{
			var str = JsonConvert.SerializeObject(qDictionary);
			File.WriteAllText(fileName, str);

		}

		public void DeserializeQueue(string fileName)
		{
			var str = File.ReadAllText(fileName);
			qDictionary = JsonConvert.DeserializeObject<ConcurrentDictionary<string, ConcurrentQueue<Message>>>(str);
		}
		
		public void Dispose()
		{
			SerializeQueue(fileName);
		}
	}
}
