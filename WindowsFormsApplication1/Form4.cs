using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WindowsFormsApplication1
{
    public partial class Form4 : Form
    {
        string server;
        string port;

        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistryKey ch = Registry.CurrentUser.CreateSubKey("ChatteR\\Settings");
            ch.SetValue("Server", textBox1.Text);
            ch.SetValue("Port", textBox2.Text);
            ch.Close();
            server = textBox1.Text;
            port = textBox2.Text;
        }

        public string returnServer()
        {
            return server;
        }

        public string returnPort()
        {
            return port;
        }
    }
}
