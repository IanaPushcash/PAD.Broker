using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
			handler = new ConsoleEventDelegate(ConsoleEventCallback);
			SetConsoleCtrlHandler(handler, true);
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
					Thread.Sleep(100);
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

		static bool ConsoleEventCallback(int eventType)
		{
			if (eventType == 2)
			{
				Broker.Dispose();
			}
			return false;
		}
		static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
		// Pinvoke
		private delegate bool ConsoleEventDelegate(int eventType);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
	}
}
