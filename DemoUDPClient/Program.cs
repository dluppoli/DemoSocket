using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DemoUDPClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var address = new IPEndPoint(IPAddress.Loopback, 8081);
            using(var clientSocket = new UdpClient() )
            {
                clientSocket.Connect(address);

                while (true)
                {
                    Console.WriteLine("Inserire il messaggio (EXIT per uscire): ");
                    string message = Console.ReadLine();
                    if (message == "EXIT") break;

                    byte[] buffer = Encoding.ASCII.GetBytes(message);
                    await clientSocket.SendAsync(buffer, buffer.Length);

                    var response = await clientSocket.ReceiveAsync();
                    message = Encoding.ASCII.GetString(response.Buffer);
                    Console.WriteLine(message);
                }
            }
        }
    }
}
