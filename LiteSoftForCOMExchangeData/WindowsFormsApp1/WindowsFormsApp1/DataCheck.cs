using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
	class DataCheck
	{
        //функция проверки адреса устройства в ответе
        public static Boolean CheckAdressOk(byte adress, string data)
        {
            if (adress == data[0]) return true; else return false;
        }
        //функция проверки номера функции в ответе
        public static Boolean CheckFunctionOk(byte function, string data)
        {
            if (function == data[1]) return true; else return false;
        }
    }
}
