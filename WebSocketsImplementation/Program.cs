using SkyTec.Networking.Websocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketsImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            var Server = new Server(8080);
            Server.Start();

            Server.onConnection += delegate(object sender, ConnectionEventArgs e)
            {
                Connection Client = e.Connection;
                Console.WriteLine("Connected to " + Client.GetHashCode());

                Client.onMessage += delegate(object sender2, MessageEventArgs e2)
                {
                    Console.WriteLine(e2.Message + " (from " + Client.GetHashCode() + ")");
                };

            };

            Console.WriteLine("Press Enter to leave.");
            Console.ReadLine();
        }
    }
}
