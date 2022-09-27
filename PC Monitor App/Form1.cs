using System.IO.Ports;
using OpenHardwareMonitor.Hardware;

namespace PC_Monitor_App
{
    public partial class Form1 : Form
    {
        private SerialPort port = new SerialPort();
        static string data;
        Computer c = new()
        {
            GPUEnabled = true,
            CPUEnabled = true
        };
        float value1, value2;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            c.Open();
        }

        private void Init()
        {
            try
            {
                notifyIcon1.Visible = false;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Handshake = Handshake.None;
                port.RtsEnable = true;
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    comboBox1.Items.Add(port);
                }
                port.BaudRate = 9600;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                port.Write("DIS*");
                port.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            label2.Text = "Disconnected";
            timer1.Enabled = false;
            statusStrip1.Text = "Connect to Arduino...";
            data = "";
        }
        private void Timer1_Tick(object Sender, EventArgs e)
        {
            Status();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!port.IsOpen)
                {
                    port.PortName = comboBox1.Text;
                    port.Open();
                    timer1.Interval = Convert.ToInt32(comboBox2.Text);
                    timer1.Enabled = true;
                    statusStrip1.Text = "Sending data...";
                    label2.Text = "Connected";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                try
                {
                    notifyIcon1.ShowBalloonTip(500, "Arduino", statusStrip1.Text, ToolTipIcon.Info);

                }
                catch (Exception ex)
                {

                }
                this.Hide();
            }


        }
            
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {

            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Status()
        {
            foreach (var hardwadre in c.Hardware)
            {
                if (hardwadre.HardwareType == HardwareType.GpuNvidia ||
                    hardwadre.HardwareType == HardwareType.GpuAti)
                {
                    hardwadre.Update();
                    foreach (var sensor in hardwadre.Sensors)
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            value1 = sensor.Value.GetValueOrDefault();
                        }
                }
                if (hardwadre.HardwareType == HardwareType.CPU)
                {
                    hardwadre.Update();
                    foreach (var sensor in hardwadre.Sensors)
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            value2 = sensor.Value.GetValueOrDefault();
                        }
                }
            }
            try
            {
                port.Write(value1 + "*" + value2 + "#");
            }
            catch (Exception ex)
            {
                timer1.Stop();
                MessageBox.Show(ex.Message);
                statusStrip1.Text = "Arduino's not responding...";
            }
        }
    }
}