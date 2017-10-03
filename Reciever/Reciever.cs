using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Lab1;
using Newtonsoft.Json;

namespace Reciever
{
	class Reciever: IDisposable
	{
		const int port = 8888;
		const string address = "127.0.0.1";


		private string name;
		public Reciever(string name)
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


				// получаем ответ
				data = new byte[1000000]; // буфер для получаемых данных
				StringBuilder builder = new StringBuilder();
				int bytes = 0;
				do
				{
					bytes = stream.Read(data, 0, data.Length);
					builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
				}
				while (stream.DataAvailable);

				var message = builder.ToString();
				Console.WriteLine("Was got {0}", message);
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
			Send(new Message() { Name = name, IsSender = true, Msg = "", TypeMsg = "die" });
		}
		
	}
}
