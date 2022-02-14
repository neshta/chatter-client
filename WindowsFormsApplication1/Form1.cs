using System;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Net.Sockets;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        static Form2 f2 = new Form2();
        static Form3 f3 = new Form3();
        static Form4 f4 = new Form4();
        string Server;
        string Port;
        string Name;
        int opened = 0;
        int connected = 0;

        TcpClient _tcpСlient = new TcpClient();
        NetworkStream ns;
        bool _stopNetwork;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Form5 f5 = new Form5();
            f5.Show();
            f5.progressBar1.Value = 25;
            groupBox1.Visible = false;
            panel2.Visible = false;
            f5.progressBar1.Value = 60;
            RegistryKey chatter = Registry.CurrentUser.OpenSubKey("ChatteR\\Settings");
            if (chatter != null)
            {
                Server = (string)Registry.GetValue("HKEY_CURRENT_USER\\ChatteR\\Settings", "Server", "No");
                Port = (string)Registry.GetValue("HKEY_CURRENT_USER\\ChatteR\\Settings", "Port", "No");
            }
            f5.progressBar1.Value = 70;
            Connect();
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            f5.progressBar1.Value = 80;
            f5.progressBar1.Value = 90;
            f5.progressBar1.Value = 100;
            f5.Hide();
            f2.ShowDialog();
            Name = f2.returnName();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void авторизацияToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f3.ShowDialog();
        }

        private void выходToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            /*
            switch (opened)
            {
                case 0:
                    {
                        System.Drawing.Bitmap image = WindowsFormsApplication1.Properties.Resources.img_Hide;
                        pictureBox2.Image = image;
                        groupBox1.Visible = true;
                        panel2.Visible = true;
                        opened = 1;
                        while (this.Size.Width != 671)
                        {
                            this.Size = new Size(this.Size.Width + 1, this.Size.Height);
                            if((int)(this.Location.X - 183) >= 0)
                            this.Location = new Point(this.Location.X - 1, this.Location.Y);
                        }
                        break;
                    }
                case 1:
                    {
                        System.Drawing.Bitmap image = WindowsFormsApplication1.Properties.Resources.img_Show;
                        pictureBox2.Image = image;
                        groupBox1.Visible = false;
                        panel2.Visible = false;
                        opened = 0;
                        while (this.Size.Width != 488)
                        {
                            this.Size = new Size(this.Size.Width - 1, this.Size.Height);
                            if ((int)(this.Location.X - 183) >= 0)
                            this.Location = new Point(this.Location.X + 1, this.Location.Y);
                        }
                        break;
                    }
            }
            */
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Equals("")) { }
            else //try 
            //{ 
                SendMessage(); 
            //}
                //catch
                //{
                    //MessageBox.Show("Не удалось установить подключение к серверу.\nВероятно сервер недоступен или отсутствует доступ к интернету.\nПроверьте доступность сервера и наличие интернета и попробуйте снова.", "Нет соединения с сервером.");
                //}
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseClient();
        }

        void Connect()
        {
            try
            {
                _tcpСlient.Connect(Server, int.Parse(Port));
                ns = _tcpСlient.GetStream();
                Thread th = new Thread(ReceiveRun);
                th.Start();
                this.Text = "ChatteR - v1.0 [Подключен]";
            }
            catch(Exception ex)
            {
                MessageBox.Show("Не удалось установить подключение к серверу.\nВероятно сервер недоступен или отсутствует доступ к интернету.\nПроверьте доступность сервера и наличие интернета и попробуйте снова.", "Нет соединения с сервером.");
            }
        }

        void CloseClient()
        {
            try
            {
                SendSpecialMessage("Bye");
            }
            catch { }
            if (ns != null) ns.Close();
            if (_tcpСlient != null) _tcpСlient.Close();
            ns = null;
            _tcpСlient = null;
            _tcpСlient = new TcpClient();

            _stopNetwork = true;

            this.Text = "ChatteR - v1.0 [Отключен]";
        }

        void SendMessage()
        {
            if (ns != null)
            {
                byte[] buffer = Encoding.Unicode.GetBytes(Name + ": " + richTextBox1.Text);
                ns.Write(buffer, 0, buffer.Length);
                richTextBox1.Text = "";
            }
        }

        void SendSpecialMessage(string s)
        {
            if (ns != null && _stopNetwork == false)
            {
                byte[] buffer = Encoding.Unicode.GetBytes(s);
                ns.Write(buffer, 0, buffer.Length);
            }
        }

        void ReceiveRun()
        {
            while (true)
            {
                try
                {
                    string s = null;
                    while (ns.DataAvailable == true)
                    {
                        byte[] buffer = new byte[_tcpСlient.Available];

                        ns.Read(buffer, 0, buffer.Length);
                        s += Encoding.Unicode.GetString(buffer);
                    }

                    if (s != null)
                    {
                        if (s.Equals("ByeAll"))
                        {
                            CloseClient();
                        }
                        else
                        {
                            ShowReceiveMessage(s);
                            s = String.Empty;
                        }
                    }
                    Thread.Sleep(100);
                }
                catch
                {
                }

                if (_stopNetwork == true) break;

            }
        }

        delegate void UpdateReceiveDisplayDelegate(string message);
        void ShowReceiveMessage(string message)
        {
            if (richTextBox2.InvokeRequired == true)
            {
                UpdateReceiveDisplayDelegate sm = new UpdateReceiveDisplayDelegate(ShowReceiveMessage);
                Invoke(sm, new object[] { message });
            }
            else
            {
                richTextBox2.AppendText("\n" + message);
            }
        }

        private void настройкиToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            f4.ShowDialog();
            Server = f4.returnServer();
            Port = f4.returnPort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1_Click(sender, e);
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            f2.StartPosition = FormStartPosition.CenterParent;
            f2.ShowDialog();
            Name = f2.returnName();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            f3.ShowDialog();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            f4.ShowDialog();
            Server = f4.returnServer();
            Port = f4.returnPort();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try
            {
                CloseClient();
            }
            catch { }
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            try
            {
                CloseClient();
                Connect();
            }
            catch
            {
            }
        }
    }
}
