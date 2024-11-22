using DemoFinalLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DemoFinaleServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var address = new IPEndPoint(IPAddress.Loopback, 8081);
            var serverSocket = new TcpListener(address);
            serverSocket.Start();

            while (true)
            {
                var clientSocket = await serverSocket.AcceptTcpClientAsync();
                MakeWork(clientSocket);
            }
        }

        public static async Task MakeWork(TcpClient clientSocket)
        {
#pragma warning disable CS4014 
            Task.Run(async () =>
            {
                using (var stream = clientSocket.GetStream())
                {
                    while (true)
                    {
                        try
                        {
                            var packet = await NetworkPacket<ServerCommand>.ReadFromStream(stream);
                            var command = packet.Decode();
                            if (command.Command == 1)
                            {
                                int fattoriale = 1;
                                for (int i = 2; i <= command.Value; i++)
                                    fattoriale *= i;

                                NetworkPacket<int> result = new NetworkPacket<int>(fattoriale);
                                await stream.WriteAsync(result.GetBytes,0,result.GetBytes.Length);
                            }
                            else if (command.Command == 2)
                            {
                                bool[] numeri = new bool[command.Value+1];
                                for(int i = 0; i < numeri.Length; i++)
                                    numeri[i] = true;

                                numeri[0] = false;
                                numeri[1] = false;

                                for(int i=2; i<= Math.Sqrt(command.Value); i++)
                                {
                                    for (int j = 2 * i; j <= command.Value; j += i)
                                        numeri[j] = false;
                                }

                                NetworkPacket<bool[]> result = new NetworkPacket<bool[]>(numeri);
                                await stream.WriteAsync(result.GetBytes, 0, result.GetBytes.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                }
                clientSocket.Dispose();
            });
#pragma warning restore CS4014 
        }
    }
}
