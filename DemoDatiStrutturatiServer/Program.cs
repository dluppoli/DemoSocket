using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DemoDatiStrutturatiServer
{
    public class CustomMessage
    {
        public int key;
        public string value;
    }

    public class NetworkPacket
    {
        public List<byte> header;
        public List<byte> buffer;

        public byte[] getBytes
        {
            get
            {
                return header.Concat(buffer).ToArray();
            }
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var address = new IPEndPoint(IPAddress.Loopback, 8082);
            TcpListener serverSocket = new TcpListener(address);
            serverSocket.Start();

            using (TcpClient clientSocket = await serverSocket.AcceptTcpClientAsync())
            {
                using(NetworkStream stream = clientSocket.GetStream())
                {
                    //Ricezione
                    byte[] header = new byte[4];
                    await stream.ReadAsync(header, 0, header.Length);

                    int bufferSize = IPAddress.NetworkToHostOrder( BitConverter.ToInt32( header, 0) );
                    byte[] buffer = new byte[bufferSize];
                    await stream.ReadAsync(buffer, 0, buffer.Length);

                    CustomMessage message = Decode(buffer);

                    Console.WriteLine($"{message.key} - {message.value}");

                    //Invio
                    message.value = "Echo di :" + message.value;
                    message.key++;

                    NetworkPacket packet = Encode(message);
                    await stream.WriteAsync(packet.getBytes,0, packet.getBytes.Length);
                }
            }
            Console.ReadLine();
        }

        static NetworkPacket Encode(CustomMessage message)
        { 
            var xs = new XmlSerializer(typeof(CustomMessage));
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            xs.Serialize(sw, message);

            byte[] buffer = Encoding.ASCII.GetBytes(sb.ToString());
            byte[] header = BitConverter.GetBytes( IPAddress.HostToNetworkOrder(buffer.Length));

            return new NetworkPacket()
            {
                header = new List<byte>(header),
                buffer = new List<byte>(buffer)
            };
        }

        static CustomMessage Decode(NetworkPacket packet)
        {
            return Decode(packet.buffer.ToArray());
        }
        static CustomMessage Decode(byte[] buffer)
        {
            string message = Encoding.ASCII.GetString(buffer);
            var xs = new XmlSerializer(typeof(CustomMessage));
            var sr = new StringReader(message);
            return (CustomMessage) xs.Deserialize(sr);
        }
    }
}
