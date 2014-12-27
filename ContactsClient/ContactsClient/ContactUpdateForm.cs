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
    public partial class ContactUpdateForm : Form
    {
        public ContactUpdateForm()
        {
            InitializeComponent();
        }
        public void SetName(string name)
        {
            textBox1.Text = name;
            this.Text = name;
        }
        public static string Id { get; set; }
        public static string name { get; set; }
        public static string Phone { get; set;  }
        public void SetPhone(string phone)
        {
            textBox2.Text = phone;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Contacts.m_client.WriteData(Utilities.updateContactString(new Contact { id = Id, Name = textBox1.Text, phoneNumbers = textBox2.Text }));
            name = textBox1.Text;
            Phone = textBox2.Text;
            this.Close();
        }

        private void ContactUpdateForm_Load(object sender, EventArgs e)
        {
           
        }
    }
}
