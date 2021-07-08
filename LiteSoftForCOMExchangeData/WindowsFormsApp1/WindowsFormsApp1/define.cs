using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace WindowsFormsApp1
{
    class define
    {
        //настройки порта COM
        public static void ComSetupPreferences(SerialPort Port)
        {
            Port.BaudRate = 115200;
            Port.Parity = Parity.None;
            Port.StopBits = StopBits.One;
            Port.DataBits = 8;
            Port.Handshake = Handshake.None;
            Port.Encoding = Encoding.GetEncoding(1251);
        }
    }
}
