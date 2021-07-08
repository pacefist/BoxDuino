using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace WindowsFormsApp1
{
    // данный класс организует обмен данными с портом COM
    class SerialClass
    {
        public List<String> Ports           = new List<String> { }; // список доступных портов
        public bool updated = false;                                // состояние окончания процесса поиска и обновления списка портов
        public int ComCounts = 0;                                   // количество найденных портов
        public int ComActive = 0;                                   // порт на котором найдено устройство
        public int NumberActualPort = 0;                            // количество портов на которых найдено наше устройство
        string indata = "";                                         // в данную перемнную будем читать данные приходящие в порт
        public void updatePorts()
        {
            int i = 0;
            String[] ports = SerialPort.GetPortNames();             // получаем список портов
            ComCounts = ports.Length;                               //просто сохраняем количество портов
            NumberActualPort = 0;                                   // обнуляем количество портов с устройством
            updated = false;                                        // порты еще ищутся и проверяются
            Ports.Clear();                                          // очищаем лист портов
            try
            {
                while (ports[i] != null)                            // проверяем каждый порт в цикле
                {
                    SerialPort mySerialPort = new SerialPort(ports[i]);                                        // подключаемся к порту
                    string EnterWord="";                                                                       // переменная для строки отправки
                    EnterWord = DataModify.DataGenerateForGetByte(1, 101);                                     // формируем запрос на первый канал в регистр 101
                    define.ComSetupPreferences(mySerialPort);                                                  // указываем настройки порта
                    mySerialPort.DataReceived += new SerialDataReceivedEventHandler (DataReceivedHandler);     // указываем обработчик события по приему данных
                    try
                    {
                        mySerialPort.Open();                                                                    // открываем порт
                        indata = "";                                                                            // очищаем буфер приема
                        if (mySerialPort.IsOpen)                                                                // если порт открылся
                        {
                            mySerialPort.Write(EnterWord);                                                      // отправка в порт запроса содержимого регистра 101
                            DateTime foo = DateTime.Now;                                                        // берем текущее время
                            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();                          // переводим в формат unixTime
                            while ((unixTime + 2) > ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds())         // ждем 2 секунды
                            {
                            }
                            byte[] asciiBytes = Encoding.GetEncoding(1251).GetBytes(indata);                    // конвертируем принятые данные в кодировку CP-1251
                            int crc = CRC.CRC_calc_check(asciiBytes);                                           // вычисляем CRC
                            if (indata.Length > 5)                                                              // проверяем корректность полученных данных
                                if (DataCheck.CheckAdressOk(1, indata) && DataCheck.CheckFunctionOk(3, indata) &&
                                    crc == (asciiBytes[asciiBytes.Length - 1] * 256 + asciiBytes[asciiBytes.Length - 2]) &&
                                    (asciiBytes[3] * 256 + asciiBytes[4]) == 140)                               // если данные корректны 
                                { NumberActualPort++; ComActive = i; }                                          // сохраняем информацию о найденонм устройстве
                            mySerialPort.Close();                                                               // закрываем порт
                            Ports.Add(ports[i]);                                                                // добавляем новую запись в порт
                        }
                    }
                    catch {
                        Ports.Add(ports[i]);                                                                    // действие в случае, если ошибка открытия порта
                        mySerialPort.Close();
                    }
                    i++;
                }
            }
            catch { }
            updated = true;                                                                                     //поиск портов завершен
        }



        // чтение одного двухбайтового числового значения с устройства из заданного регистра 
        public int ReadIntParam(SerialPort port, int adress)
        {
            if (port.IsOpen)
            {
                string data = "";
                data = DataModify.DataGenerateForGetByte(1, adress);
                indata = "";
                port.Write(data);
                DateTime foo = DateTime.Now;
                long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
                while ((unixTime + 2) > ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds())
                { if (indata.Length >= 7) break; }
                byte[] asciiBytes = Encoding.GetEncoding(1251).GetBytes(indata);
                if (CRC.CRC_calc_check(asciiBytes) == (asciiBytes[asciiBytes.Length - 1] * 256 + asciiBytes[asciiBytes.Length - 2]))
                    return asciiBytes[4];
            }
            return -1;
        }

        //запись в заданный регистр одного двухбайтового числа
        public int WriteIntParam(SerialPort port, int adress, int value)
        {
            if (port.IsOpen)
            {
                string data = "";
                data = DataModify.DataGenerateForSendByte(1, adress, value);
                indata = "";
                port.Write(data);
                DateTime foo = DateTime.Now;
                long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
                while ((unixTime + 2) > ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds())
                { if (indata.Length >= 8) break; }
                byte[] asciiBytes = Encoding.GetEncoding(1251).GetBytes(indata);
                if (CRC.CRC_calc_check(asciiBytes) == (asciiBytes[asciiBytes.Length - 1] * 256 + asciiBytes[asciiBytes.Length - 2]))
                    return asciiBytes[5];
            }
            return -1;
        }



        // функция события получения данных из порта
        public void DataReceivedHandler(
                object sender,
                SerialDataReceivedEventArgs e) 
        {
            SerialPort sp = (SerialPort)sender;
            indata = indata+(sp.ReadExisting());
        }
        
    }

}
