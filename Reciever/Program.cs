using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
			Reciever reciever;
			Task task = new Task(() =>
			{
				while (true)
				{
					reciever = new Reciever(sName);

					//while (true)
					//{
					//	Console.WriteLine("Enter msg type: ");
					//	msgType = Console.ReadLine();
					//var start = DateTime.Now;
					//if (msgType == "die")
					//{
					//	reciever.Send(new Message() {IsSender = false, Msg = "", Name = sName, TypeMsg = "willdie"});
					//	//break;
					//}
					//else
					//{
					reciever.Send(new Message() {IsSender = false, Msg = "", Name = sName, TypeMsg = msgType} );
					//}
					//while (start.AddSeconds(10) > DateTime.Now) ;
					//}
					//if (msgType == "die") Environment.Exit(0);
					Thread.Sleep(100);
				}
			});
			task.Start();
			//task.Wait();
			while (true)
			{
				//Console.WriteLine("Enter msg type: ");
				//msgType = 
					Console.ReadLine();
				//task.;
				//task.Start();
				//if (msgType == "die")
				//	Reciever.WillDie = true;
			}
		}
	}
}
