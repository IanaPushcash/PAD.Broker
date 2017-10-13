using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Lab1;
using Newtonsoft.Json;

namespace Sender
{
	class Sender : IDisposable
	{
		const int port = 8888;
		const string address = "127.0.0.1";
		

		private string name;
		public Sender(string name)
		{
			this.name = name;
		}

		public void Send(Message msg)
		{
			TcpClient client = null;
			try
			{
				client = new TcpClient(address, port);
				NetworkStream stream = client.GetStream();

				byte[] data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
				// отправка сообщения
				stream.Write(data, 0, data.Length);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				client.Close();
			}
		}

		

		public void Dispose()
		{
			Send(new Message() {Name = name, IsSender = true, Msg = "", TypeMsg = "die"});
		}

		~Sender()
		{
			Send(new Message() { Name = name, IsSender = true, Msg = "", TypeMsg = "die" });
		}
	}
}
