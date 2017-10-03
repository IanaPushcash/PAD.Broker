using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lab1;
using Newtonsoft.Json;

namespace Sender
{
	class Program
	{
		private static ManualResetEvent _stopper = new ManualResetEvent(false);

		static void Main(string[] args)
		{
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
				using (Sender sender = new Sender(sName))
				{
					while (true)
					{
						var start = DateTime.Now;
						if (msgType == "die")
						{
							sender.Send(new Message() {IsSender = true, Msg = "", Name = sName, TypeMsg = "willdie"});
							break;
						}
						else
						{
							var msg = new Message() {IsSender = true, Msg = i + "", Name = sName, TypeMsg = msgType};
							sender.Send(msg);
							i++;
							Console.WriteLine("Sended " + JsonConvert.SerializeObject(msg));
						}
						while (start.AddSeconds(10) > DateTime.Now) {};
					}
				}
			});
			task.Start();
			while (true)
			{
				Console.WriteLine("Enter msg type: ");
				msgType = Console.ReadLine();
			}
		}

		

	}
}
