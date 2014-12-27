using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContactsClient
{
    public partial class ContactInterface : UserControl
    {
        public String Id { get; set; }
        public String name { get { return label1.Text; } set { label1.Text = value; } }
        public String Phone { get { return label2.Text; } set { label2.Text = value; } }
        public ContactInterface()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ContactUpdateForm cuf = new ContactUpdateForm();
            cuf.SetName(name);
            cuf.SetPhone(Phone);
            ContactUpdateForm.Id = Id;
            cuf.ShowDialog();
            this.label1.Text = ContactUpdateForm.name;
            this.label2.Text = ContactUpdateForm.Phone;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure?", "Delete Operation", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Yes)
            {
              Contacts.m_client.WriteData(Utilities.deleteContactString(Id));                
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

      
    }
}
