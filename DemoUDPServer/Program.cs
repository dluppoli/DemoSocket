using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoUDPServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var address = new IPEndPoint(IPAddress.Loopback, 8081);
            using (UdpClient serverSocket = new UdpClient(address))
            {
                while (true)
                {
                    var result = await serverSocket.ReceiveAsync();
                    byte[] buffer = result.Buffer;

                    string message = Encoding.ASCII.GetString(buffer);

                    Thread.Sleep(3000);

                    message = "Echo di : " + message;

                    byte[] responseBuffer = Encoding.ASCII.GetBytes(message);
                    await serverSocket.SendAsync(responseBuffer, responseBuffer.Length,result.RemoteEndPoint);
                }
            }
        }
    }
}
