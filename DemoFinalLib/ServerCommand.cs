using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFinalLib
{
    public class ServerCommand
    {
        public int Command;
        public int Value;

        public ServerCommand() 
        {  }

        public ServerCommand(int command, int value) : this()
        {
            Command = command;
            Value = value;  
        }
    }
}
