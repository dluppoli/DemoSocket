using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DemoDatiStrutturatiClient
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
            using (var clientSocket = new TcpClient())
            {
                await clientSocket.ConnectAsync(address.Address, address.Port);
            
                using(var stream = clientSocket.GetStream())
                {
                    //Invio
                    Console.Write("Inserire la chiave: ");
                    int key = int.Parse(Console.ReadLine());

                    Console.Write("Inserire il valore: ");
                    string value = Console.ReadLine();

                    CustomMessage message = new CustomMessage() { key = key, value = value };
                    NetworkPacket packet = Encode(message);
                    await stream.WriteAsync(packet.getBytes, 0, packet.getBytes.Length);

                    //Ricezione
                    byte[] header = new byte[4];
                    await stream.ReadAsync(header, 0, header.Length);

                    int bufferSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));
                    byte[] buffer = new byte[bufferSize];
                    await stream.ReadAsync(buffer, 0, buffer.Length);

                    message = Decode(buffer);

                    Console.WriteLine($"Ricevuto: {message.key} - {message.value}");
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
            byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(buffer.Length));

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
            return (CustomMessage)xs.Deserialize(sr);
        }
    }
}
