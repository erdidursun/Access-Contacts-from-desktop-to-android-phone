using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContactsClient
{
    public partial class Connect : Form
    {    

        public Connect()
        {
            InitializeComponent();
        }

        private void Connect_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.IpAddress = textBox1.Text;
            Client.Port = Convert.ToInt32(textBox2.Text);            
            Contacts f = new Contacts();
            this.Hide();
            f.ShowDialog();
        }
    }
}
