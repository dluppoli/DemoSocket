using DemoFinalLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DemoFinaleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var address = new IPEndPoint(IPAddress.Loopback, 8081);
            using (var clientSocket = new TcpClient())
            {
                await clientSocket.ConnectAsync(address.Address, address.Port);

                using (var stream = clientSocket.GetStream())
                {
                    while (true)
                    {
                        Console.WriteLine("1 - Calcolo Fattoriale");
                        Console.WriteLine("2 - Calcolo Eratostene");
                        Console.WriteLine("9 - Uscita");
                        Console.Write("Inserire la scelta: ");

                        string scelta = Console.ReadLine().Trim();

                        if (scelta == "1")
                        {
                            Console.Write("Inserire il numero di cui calcolare il fattoriale: ");
                            int n = int.Parse(Console.ReadLine());

                            ServerCommand command = new ServerCommand(int.Parse(scelta), n);
                            NetworkPacket<ServerCommand> packet = new NetworkPacket<ServerCommand>(command);
                            await stream.WriteAsync(packet.GetBytes, 0, packet.GetBytes.Length);

                            var response = await NetworkPacket<int>.ReadFromStream(stream);
                            Console.WriteLine(response.Decode());
                        }
                        else if (scelta == "2")
                        {
                            Console.Write("Calcolare i numeri primi <= ");
                            int n = int.Parse(Console.ReadLine());

                            ServerCommand command = new ServerCommand(int.Parse(scelta), n);
                            NetworkPacket<ServerCommand> packet = new NetworkPacket<ServerCommand>(command);
                            await stream.WriteAsync(packet.GetBytes, 0, packet.GetBytes.Length);

                            var response = await NetworkPacket<bool[]>.ReadFromStream(stream);
                            bool[] results = response.Decode();
                            for (int i = 0; i<results.Length;i++)
                            {
                                if( results[i]==true)
                                    Console.WriteLine(i);
                            }
                        }
                        else if (scelta == "9")
                            break;
                        else
                            Console.WriteLine("Scelta errata");

                        Console.WriteLine("Premi un tasto per continuare");
                        Console.ReadLine();
                        Console.Clear();
                    }
                }
            }
        }
    }
}
