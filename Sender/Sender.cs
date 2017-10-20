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
		private NetworkStream stream;
		private TcpClient client = null;

		public string name { get; set; }
		public Sender(string name)
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
				
				
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			//finally
			//{
			//	client.Close();
			//}
		}

		public void Wait()
		{
			try
			{
				while (true)
				{
					if (stream.DataAvailable)
					{
						// получаем ответ
						byte[] data = new byte[1000000]; // буфер для получаемых данных
						StringBuilder builder = new StringBuilder();
						int bytes = 0;
						do
						{
							bytes = stream.Read(data, 0, data.Length);
							builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
						} while (stream.DataAvailable);
						stream.Flush();
						//name = name + "~" + builder.ToString();
					}
				}
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
