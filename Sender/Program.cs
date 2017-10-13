﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lab1;
using Newtonsoft.Json;

namespace Sender
{
	class Program
	{
		private static Sender sender;
		static void Main(string[] args)
		{
			handler = new ConsoleEventDelegate(ConsoleEventCallback);
			SetConsoleCtrlHandler(handler, true);
			Console.WriteLine("Enter sender name: ");
			var sName = Console.ReadLine();
			Console.WriteLine("Enter msg type: ");
			var msgType = Console.ReadLine();
			Console.WriteLine("Enter start i: ");
			int i;
			try
			{
				i = Convert.ToInt32(Console.ReadLine());
			}
			catch
			{
				i = 0;
			}
			Task task = new Task(() =>
			{
				sender = new Sender(sName);
				while (true)
				{
					var start = DateTime.Now;
					if (msgType == "die")
					{
						sender.Send(new Message() {IsSender = true, Msg = "", Name = sName, TypeMsg = "willdie"});
						Environment.Exit(0);
					}
					else
					{
						var msg = new Message() {IsSender = true, Msg = i + "", Name = sName, TypeMsg = msgType};
						sender.Send(msg);
						i++;
						Console.WriteLine("Sended " + JsonConvert.SerializeObject(msg));
					}
					while (start.AddSeconds(10) > DateTime.Now)
					{
					}
					;
				}

			});
			task.Start();
			while (true)
			{
				Console.WriteLine("Enter msg type: ");
				msgType = Console.ReadLine();
			}
		}

		static bool ConsoleEventCallback(int eventType)
		{
			if (eventType == 2)
			{
				sender.Dispose();
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
