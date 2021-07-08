using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    //класс подсчета CRC смотри статью https://zen.yandex.ru/media/id/60e6a16dc39351282891d79d/crc-16-kod-dlia-modbus-na-c-60e6a1843b8bbd3052227c0a
    class CRC
    {
        public static int CRC_calc(string datas)
        {
            int k = 0;
            int length_command = datas.Length;
            int i;
            int crc_result = 0xFFFF;
            while (length_command > 0)
            {
                length_command--;
                crc_result ^= (ushort)datas[k];
                k++;
                for (i = 0; i < 8; i++)
                {
                    if ((crc_result & 0x01) == 1)
                        crc_result = (crc_result >> 1) ^ 0xA001;
                    else crc_result = (crc_result >> 1);
                }
            }
            return crc_result;
        }
        public static int CRC_calc_check(byte[] data)
        {
            string datas ="";
            for (int n = 0; n < (data.Length - 2); n++) datas = datas + (char)data[n];
            return CRC_calc(datas);
        }
        public static int CRC_calc_check(string data)
        {
            string datas = "";
            for (int n = 0; n < (data.Length - 2); n++) datas = datas + (char)data[n];
            return CRC_calc(datas);
        }
    }
}