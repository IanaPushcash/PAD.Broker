using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lab1;
using Newtonsoft.Json;

namespace Reciever
{
	class Reciever 
	{
		const int port = 8888;
		const string address = "127.0.0.1";
		private NetworkStream stream;
		private TcpClient client;
		//public static bool WillDie = false;

		private string name;

		public Reciever(string name)
		{
			this.name = name;
			client = new TcpClient(address, port);
			stream = client.GetStream();
		}

		public void Send(Message msg)
		{
			try
			{
				byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
				// отправка сообщения
				stream.Write(data, 0, data.Length);

				while (true)
				{
					if (stream.DataAvailable)
					{
						// получаем ответ
						data = new byte[1000000]; // буфер для получаемых данных
						StringBuilder builder = new StringBuilder();
						int bytes = 0;
						do
						{
							bytes = stream.Read(data, 0, data.Length);
							builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
						} while (stream.DataAvailable);
						stream.Flush();
						var message = builder.ToString();
						Console.WriteLine("Was got {0}", message);
					}
					Thread.Sleep(100);
					//if (WillDie)
					//{
					//	willDie();
					//	break;
					//}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		//public void Dispose()
		//{
		//	Send(new Message() {Name = name, IsSender = true, Msg = "", TypeMsg = "die"});
		//}

		//~Reciever()
		//{
		//	byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(new Message() { Name = name, IsSender = true, Msg = "", TypeMsg = "die" }));
		//	// отправка сообщения
		//	stream.Write(data, 0, data.Length);
		//	client?.Close();
		//}

		//private void willDie()
		//{
		//	byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(new Message() { Name = name, IsSender = true, Msg = "", TypeMsg = "willdie" }));
		//	// отправка сообщения
		//	stream.Write(data, 0, data.Length);
		//	WillDie = false;
		//}
	}
}
