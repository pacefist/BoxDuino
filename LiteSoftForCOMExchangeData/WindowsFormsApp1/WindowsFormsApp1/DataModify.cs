using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class DataModify
    {
        // формирование ModBus RTU запроса на отправку одного двухбайтового значения на заданный адрес заданный канал
        public static string DataGenerateForSendByte (byte chanel, int adress, int data)
        {
            String result = "";
            result = result + (char)chanel + (char)6 + 
                (char)(adress >> 8) + (char)(adress & 0xFF) + 
                (char)(data >> 8) + (char)(data & 0xFF);
            int crc1 = CRC.CRC_calc(result);
            result += (char)(crc1 & 0xFF);
            result += (char)(crc1 >> 8);
            result = ConvertTo1251(result);
            return result;
        }
        // формирование ModBus RTU запроса на получение одного двухбайтового значения с заданного адреса заданного канала
        public static string DataGenerateForGetByte(byte chanel, int adress)
        {
            String result = "";
            result = result + (char)chanel + (char)3 + 
                (char)(adress >> 8) + (char)(adress & 0xFF) + 
                (char)0 + (char)1;
            int crc1 = CRC.CRC_calc(result);
            result += (char)(crc1 & 0xFF);
            result += (char)(crc1 >> 8);
            result = ConvertTo1251(result);
            return result;
        }

        // конвертация данных в формат CP-1251 для адекватной работы с символами, код которых имеет значение более 127
        public static string ConvertTo1251(string inputString)
        {
            string s = "";
            byte[] symbol1 = { 0x00 };
            for (int i = 0; i < inputString.Length; i++)
            {
                symbol1[0] = (byte)inputString[i];
                s = s + Encoding.GetEncoding(1251).GetString((symbol1));
            }
            return s;
        }

        // функция из строки кодов символов в HEX формирует и отдает строку символов 
        public static string convertToString(string inputString)
        {
            string s = "";
            char a = (char)0;
            for (int i = 2; i < inputString.Length; i++)
            {
                if (inputString[i] > 47 && inputString[i] < 58) a = Convert.ToChar(inputString[i] - 48);
                if (inputString[i] > 64 && inputString[i] < 71) a = Convert.ToChar(inputString[i] - 55);
                a = Convert.ToChar((int)a * 16);
                i++;
                if (inputString[i] > 47 && inputString[i] < 58) a = Convert.ToChar(a + (inputString[i] - 48));
                if (inputString[i] > 64 && inputString[i] < 71) a = Convert.ToChar(a + (inputString[i] - 55));
                s = s + a;
            }
            return s;
        }

        // функция из строки в кодировке CP-1251 отдает строку кодов символов в HEX
        public static string Convert1251ToHEX(string inputString)
        {
            string s = "";
            string asciiBytes = inputString;

            for (int i = 0; i < inputString.Length; i++)
            {
                if ((asciiBytes[i] >> 4) < 10) s = s + (char)((asciiBytes[i] >> 4) + 48);
                if ((asciiBytes[i] >> 4) >= 10) s = s + (char)((asciiBytes[i] >> 4) + 55);
                if ((asciiBytes[i] & 0x0F) < 10) s = s + (char)((asciiBytes[i] & 0x0F) + 48);
                if ((asciiBytes[i] & 0x0F) >= 10) s = s + (char)((asciiBytes[i] & 0x0F) + 55);
                s = s + " ";
            }
            return s;
        }

        // функция из строки полученной из порта формирует строку кодов символов в HEX
        public static string ConvertToHEX(string inputString)
        {
            string s = "";
            byte[] asciiBytes = Encoding.GetEncoding(1251).GetBytes(inputString);

            for (int i = 0; i < inputString.Length; i++)
            {
                if ((asciiBytes[i] >> 4) < 10) s = s + (char)((asciiBytes[i] >> 4) + 48);
                if ((asciiBytes[i] >> 4) >= 10) s = s + (char)((asciiBytes[i] >> 4) + 55);
                if ((asciiBytes[i] & 0x0F) < 10) s = s + (char)((asciiBytes[i] & 0x0F) + 48);
                if ((asciiBytes[i] & 0x0F) >= 10) s = s + (char)((asciiBytes[i] & 0x0F) + 55);
                s = s + " ";
            }
            return s;
        }


    }
}