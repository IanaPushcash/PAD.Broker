using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1;

namespace Reciever
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Enter reciever name: ");
			var sName = Console.ReadLine();
			Console.WriteLine("Enter msg type: ");
			var msgType = Console.ReadLine();
			Task task = new Task(() =>
			{
				using (Reciever reciever = new Reciever(sName))
				{
					while (true)
					{

						var start = DateTime.Now;
						if (msgType == "die")
						{
							reciever.Send(new Message() { IsSender = false, Msg = "", Name = sName, TypeMsg = "willdie" });
							break;
						}
						else
						{
							reciever.Send(new Message() { IsSender = false, Msg = "", Name = sName, TypeMsg = msgType });
						}
						while (start.AddSeconds(10) > DateTime.Now) ;
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
