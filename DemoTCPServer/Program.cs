using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DemoTCPServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Creare la socket
            var address = new IPEndPoint(IPAddress.Loopback, 8080);
            //Socket serverSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //Bindind della socket verso IP e Porta
            //serverSocket.Bind(address);
            //Mettere in ascolto il server
            //serverSocket.Listen(100);
            TcpListener serverSocket = new TcpListener(address);
            serverSocket.Start();


            //Accettare la connessione
            while (true)
            {
                using (TcpClient clientSocket = await serverSocket.AcceptTcpClientAsync())
                {
                    //Generare lo stream di comunicazione
                    using (NetworkStream stream = clientSocket.GetStream() )
                    {
                        while (true)
                        {
                            try
                            {
                                //Ricevere dati
                                byte[] buffer = new byte[1024];
                                await stream.ReadAsync(buffer, 0, buffer.Length);

                                //Elabora i dati
                                string message = Encoding.ASCII.GetString(buffer);
                                Console.WriteLine("Ho ricevuto " + message);

                                //Spedisce dati di risposta
                                message = "Echo di: " + message;
                                byte[] responseBuffer = Encoding.ASCII.GetBytes(message);
                                await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                            }
                            catch (Exception ex)
                            {   
                                Console.WriteLine(ex.ToString());
                                break;
                            }
                        }
                        //Chiusura della socket e dello stream
                    }
                }
            }
//            Console.WriteLine("Esecuzione terminata");
//            Console.ReadLine();
        }
    }
}
