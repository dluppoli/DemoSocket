using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DemoFinalLib
{
    public class NetworkPacket<T>
    {
        public List<byte> Header;
        public List<byte> Data;

        public byte[] GetBytes
        {
            get
            {
                return Header.Concat(Data).ToArray();
            }
        }

        public NetworkPacket(byte[] header, byte[] data)
        {
            Header = header.ToList();
            Data = data.ToList();
        }

        public NetworkPacket(T data)
        {
            var xs = new XmlSerializer(typeof(T));
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            xs.Serialize(sw, data);

            Data = Encoding.ASCII.GetBytes(sb.ToString()).ToList();
            Header = BitConverter.GetBytes( IPAddress.HostToNetworkOrder(Data.Count)).ToList();
        }

        public T Decode()
        { 
            var str = Encoding.UTF8.GetString(Data.ToArray());
            var sr = new StringReader(str);
            var xs = new XmlSerializer(typeof(T));

            return (T)xs.Deserialize(sr);
        }

        static public async Task<NetworkPacket<T>> ReadFromStream(NetworkStream stream)
        {
            byte[] header = new byte[4];
            await stream.ReadAsync(header, 0, header.Length);

            int dataBufferSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));
            byte[] dataBuffer = new byte[dataBufferSize];
            await stream.ReadAsync(dataBuffer, 0, dataBuffer.Length);

            return new NetworkPacket<T>(header, dataBuffer);
            
        }
    }
}
