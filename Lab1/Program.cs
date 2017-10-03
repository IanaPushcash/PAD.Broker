using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1
{
	class Program
	{
		const int port = 8888;
		static TcpListener listener;
		private static Thread _serverThread;

		static void Main(string[] args)
		{
			_serverThread = new Thread(startServer) {IsBackground = true};
			_serverThread.Start();
			Console.ReadLine();
		}

		private static void startServer()
		{
			try
			{
				listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
				listener.Start();
				Console.WriteLine("Ожидание подключений...");

				while (true)
				{
					TcpClient client = listener.AcceptTcpClient();
					Client clientObject = new Client(client);

					// создаем новый поток для обслуживания нового клиента
					Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
					clientThread.Start();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				if (listener != null)
					listener.Stop();
			}

		}
	}
}
