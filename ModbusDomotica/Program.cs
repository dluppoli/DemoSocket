using NModbus;
using NModbus.Serial;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusDomotica
{
    internal class Program
    {
        private static double target = 21.5;
        private static bool appRunning = true;

        static void Main(string[] args)
        {
            var t = new Thread(RefreshUI);
            t.Start();
            
            string scelta = "";
            while(scelta!="E")
            { 
                scelta = Console.ReadLine();
                if (scelta == "+") target += 0.5;
                if (scelta == "-") target -= 0.5;
            }
            appRunning = false;
        }

        static async void RefreshUI() {
            using (var port = new SerialPort("COM2"))
            {
                port.BaudRate = 9600;
                port.Parity = Parity.Even;
                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Open();

                var factory = new ModbusFactory();
                var bus = factory.CreateRtuMaster(port);


                var vecchiaTempTarget = -100.0;
                var vecchiaTemperatura = -100.0;

                while (appRunning)
                {
                    //Lettura temperatura
                    ushort[] valori = await bus.ReadHoldingRegistersAsync(1, 0, 2);
                    double temperatura = valori[0] / 10.0;
                    ushort apertura = valori[1];

                    //Determinazione % apertura
                    ushort nuovaApertura = 0;
                    if (temperatura < target) nuovaApertura = 25;
                    if (temperatura < target - 0.5) nuovaApertura = 50;
                    if (temperatura < target - 1) nuovaApertura = 75;
                    if (temperatura < target - 1.5) nuovaApertura = 100;

                    //Scrittura % apertura
                    if (nuovaApertura != apertura)
                        await bus.WriteSingleRegisterAsync(1, 1, nuovaApertura);

                    //Stampa UI
                    if (nuovaApertura != apertura ||
                        vecchiaTemperatura != temperatura ||
                        vecchiaTempTarget != target)
                    {
                        Console.Clear();
                        Console.WriteLine($"Temperatura Target: {target}");
                        Console.WriteLine($"Temperatura Attuale: {temperatura}");
                        Console.WriteLine($"% Apertura: {nuovaApertura}");
                        Console.WriteLine();
                        Console.WriteLine("Opzioni: +  -  E");
                    }

                    vecchiaTempTarget = target;
                    vecchiaTemperatura = temperatura;

                    //Aspetto 30 secondi
                    Thread.Sleep(500);
                }
            }
        }
    }
}
