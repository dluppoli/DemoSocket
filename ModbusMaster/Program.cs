using NModbus;
using NModbus.Serial;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusMaster
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Configurazione della porta seriale (e apertura)
            using (SerialPort port = new SerialPort("COM2"))
            {
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.Even;
                port.StopBits = StopBits.One;

                port.Open();

                //Creare il ModbusFactory
                var factory = new ModbusFactory();

                //Creare (e configurare) il bus di comunicazione
                var bus = factory.CreateRtuMaster(port);

                //Inviare operazioni agli slave
                var coils = await bus.ReadCoilsAsync(1, 0, 2);
                foreach(var coil in coils) Console.WriteLine(coil);

                await bus.WriteSingleCoilAsync(1,2,true);

                double temperaturaTarget = 21.5;
                await bus.WriteSingleRegisterAsync(1, 0, (ushort)(temperaturaTarget * 10));

                Console.ReadLine();
            }
        }
    }
}
