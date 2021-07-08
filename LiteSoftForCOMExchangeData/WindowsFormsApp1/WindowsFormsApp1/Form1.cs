using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Management;
using System.Threading;



namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SerialPort activePort = new SerialPort();
        SerialClass SerialPorts = new SerialClass();
        public Form1()
        {
            InitializeComponent();
            autoconnect();
        }



        private void автоподключениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoconnect();
        }

        // обновляем состояние при поиске портов
        private void timer1_Tick(object sender, EventArgs e)
        {

            toolStripProgressBar1.Value = (((SerialPorts.ComActive+1)*100)/(SerialPorts.ComCounts+1));
            if (SerialPorts.updated == true)
            {
                try
                {
                    for (int i = 0; i < SerialPorts.Ports.Count; i++)
                    {
                        if (i == SerialPorts.ComActive && SerialPorts.NumberActualPort > 0)
                        {
                            toolStripComboBox1.Items.Add(SerialPorts.Ports[i] + " Ready");
                        }
                        else
                        {
                            toolStripComboBox1.Items.Add(SerialPorts.Ports[i]);
                        }
                    }
                }
                catch { }
                if (SerialPorts.NumberActualPort==1)
                {
                    activePort.PortName=SerialPorts.Ports[SerialPorts.ComActive];
                    define.ComSetupPreferences(activePort);
                    activePort.DataReceived += new SerialDataReceivedEventHandler(SerialPorts.DataReceivedHandler);
                    activePort.Open();
                    toolStripComboBox1.SelectedIndex=SerialPorts.ComActive;
                }
                SerialPorts.updated = false;
                timer1.Enabled = false;
                toolStripProgressBar1.Value = 0;
            }
        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            autoconnect();
        }

        private void autoconnect()
        {
            if (SerialPorts.updated == false && timer1.Enabled == false)                                // если процесс автоподключения не активен
            {
                activePort.Close();                                                                     // закрываем активный порт
                toolStripComboBox1.Items.Clear();                                                       // очищаем список ранее найденных портов
                toolStripComboBox1.Text = "";                                                           // очищаем поле отображения текущего порта
                Thread threadUpdatePorts = new Thread(new ThreadStart(SerialPorts.updatePorts));        // формируем отдельный поток поиска портов
                timer1.Enabled = true;                                                                  // запускаем таймер, он покажет нам процес поиска и результат
                threadUpdatePorts.Start();                                                              // запускаем поток
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            activePort.Close();
        }

        // отключение от порта / подключение к порту
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (activePort.IsOpen)                                                          
            {
                activePort.Close();
                toolStripComboBox1.Text = "";
            } else 
            if (SerialPorts.NumberActualPort == 1)
            {
                try
                {
                    activePort.PortName = SerialPorts.Ports[SerialPorts.ComActive];
                    activePort.Open();
                    toolStripComboBox1.SelectedIndex = SerialPorts.ComActive;
                }
                catch
                {

                }
            }
        }

        // изменение состояния регистра указывающее состояние 13-го порта для данного устройства шаг по шагу
        private void button1_Click(object sender, EventArgs e)
        {
            if (SerialPorts.ReadIntParam(activePort, 0) == 1)
                 button1.Text = "" + SerialPorts.WriteIntParam(activePort, 0, 0);
            else button1.Text = "" + SerialPorts.WriteIntParam(activePort, 0, 1);

        }

        // контроль порта в реальном времени
        private void timerCheckConnect_Tick(object sender, EventArgs e)
        {
            if (activePort.IsOpen)
            {
                toolStripButton2.BackColor = Color.FromArgb(00, 200, 00);
                toolStripComboBox1.SelectedIndex=SerialPorts.ComActive;
                toolStripComboBox1.Text = SerialPorts.Ports[SerialPorts.ComActive] + " Ready";
            }
            else
                toolStripButton2.BackColor = Color.FromArgb(255, 150, 150);
            int val = SerialPorts.ReadIntParam(activePort, 0);
            if (val!=-1) button1.Text = "" + SerialPorts.ReadIntParam(activePort, 0);
            else         button1.Text = "";
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
