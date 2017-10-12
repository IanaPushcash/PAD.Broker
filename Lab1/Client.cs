using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lab1
{
	class Client
	{
		public TcpClient client;
		public bool IsSender = false;
		public Client(TcpClient tcpClient)
		{
			client = tcpClient;
		}

		public void Process()
		{
			NetworkStream stream = null;
			try
			{
				stream = client.GetStream();
				byte[] data = new byte[1000000]; // буфер для получаемых данных
				// получаем сообщение
				StringBuilder builder = new StringBuilder();
				int bytes = 0;
				do
				{
					bytes = stream.Read(data, 0, data.Length);
					builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
				} while (stream.DataAvailable);

				Message msg = JsonConvert.DeserializeObject<Message>(builder.ToString());
				Broker.GetInstance().ProcessingMsg(msg, stream);
				if (msg.IsSender) IsSender = true;
				//	while (true)
				//	{
				//		stream.
				//	}
				//	Broker.GetInstance().AddMsg(msg);
				//else Broker.GetInstance().GetAnswerMsg(msg, stream);
				// отправляем обратно сообщение в верхнем регистре
				//message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();
				//data = Encoding.Unicode.GetBytes(message);
				//stream.Write(data, 0, data.Length);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				if (stream != null)
					stream.Close();
				if (client != null)
					client.Close();
			}
		}
	}
}
