using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DemoTCPClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Creo una nuova socket
            var address = new IPEndPoint(IPAddress.Loopback, 8080);
            //using (Socket serverSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            using (TcpClient serverSocket = new TcpClient())
            {
                //Collegarsi al server
                //await serverSocket.ConnectAsync(IPAddress.Loopback, 8080);
                await serverSocket.ConnectAsync(address.Address, address.Port);

                //Creare lo stream di comunicazione
                using (var stream = serverSocket.GetStream())
                {
                    //Invia i dati al server
                    while(true)
                    {
                        Console.Write("Inserire il messaggio da inviare (EXIT per uscire: ");
                        string message = Console.ReadLine();
                        if (message == "EXIT") break;

                        byte[] buffer = Encoding.ASCII.GetBytes(message);
                        await stream.WriteAsync(buffer, 0, buffer.Length);

                        //Riceve la risposta dal server
                        byte[] responseBuffer = new byte[1024];
                        await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                        string responseMessage = Encoding.ASCII.GetString(responseBuffer);
                        Console.WriteLine("Il server ha risposto con: " + responseMessage);
                    } 
                    //Chiudere socker e stream
                }
            }
            Console.ReadLine();
        }
    }
}
