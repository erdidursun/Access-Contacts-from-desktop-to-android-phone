using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContactsClient
{
    public partial class Contacts : Form
    {
        public List<Contact> contacts = new List<Contact>();
        public List<Contact> tmp = new List<Contact>();
        delegate void AddContactHandler(Control c);
        delegate void RemoveContactsHandler();
        RemoveContactsHandler RemoveContacts;
        Thread t;
        AddContactHandler AddContact;
       
        public Contacts()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.GetContacts();           
        }

        void client_Connected(bool success)
        {
            if (success)
            {
                this.Text = "Connection Has Been Established ";
                Client.GetContacts();

            }
            else
            {
                this.Text = "Connection Failed. ";
            }
        }
        void ShowContact()
        {
            int y = 2;
            this.Text = "Contacts Loading....";
            this.Invoke(RemoveContacts);           
            foreach (var c in tmp)
            {
                ContactInterface cI = new ContactInterface();
                if (c != null)
                {
                    cI.name = c.Name;
                    cI.Id = c.id;
                    cI.Phone = c.phoneNumbers;
                    cI.Location = new Point(1, y);
                    y += cI.Height + 2;
                    this.Invoke(AddContact, cI);
                }
            }
            tmp = contacts;
             MessageBox.Show("Contacts Complete.");
             this.Close();
        }
        void Remove()
        {
            panel2.Visible = true;
            panel2.Controls.Clear();
            groupBox3.Visible = true;
            groupBox2.Enabled = true;
        }
        void Add(Control c)
        {
            panel2.Controls.Add(c);
            panel2.Refresh();
        }
        void StartThread()
        {
            if (t != null)
            {
                t.Abort();
                t.DisableComObjectEagerCleanup();

            }
            t = new Thread(new ThreadStart(ShowContact));
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();
        }
        void ContactAdded(List<Contact> contacts)
        {     
            tmp = this.contacts = contacts.OrderBy(p => p.Name).ToList<Contact>();
            StartThread();
        }
        void ContactDelete(string id)
        {
            int index = contacts.FindIndex(p => p.id.Equals(id));
            string name = contacts[index].Name;
            contacts.RemoveAt(contacts.FindIndex(p => p.id.Equals(id)));
            MessageBox.Show(String.Format("{0} is Deleted", name), "Delete Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            StartThread();

        }
        void ContactUptade(string id, string name, string phone)
        {
            contacts[contacts.FindIndex(p => p.id == id)] = new Contact() { id = id, Name = name, phoneNumbers = phone };
            MessageBox.Show(String.Format("{0} is Updated", name), "Update Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            StartThread();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            AddContact = new AddContactHandler(Add);
            RemoveContacts = new RemoveContactsHandler(Remove);
            CheckForIllegalCrossThreadCalls = false;
            Client.Connect();
            Client.Connected += client_Connected;
            Client.ContactsAdded += ContactAdded;
            Client.ContactDelete += ContactDelete;
            Client.ContactUptade += ContactUptade;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (comboBox1.SelectedIndex == 0)
            {
                tmp = tmp.Where<Contact>(p => tmp.Where(t => t.Name == p.Name).Count() > 1).ToList();
                if (tmp.Count == 0)
                {
                    MessageBox.Show("No records were found matching");
                }
                else
                StartThread();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                tmp = tmp.Where<Contact>(p => tmp.Where(t => t.phoneNumbers == p.phoneNumbers).Count() > 1).ToList();
                if (tmp.Count == 0)
                {
                    MessageBox.Show("No records were found matching");
                }
                else
                StartThread();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                ShowContact();
            else
            {
                tmp = tmp.Where(p => p.Name.ToLower().Contains(textBox1.Text.ToLower()) || p.phoneNumbers.Contains(textBox1.Text)).ToList<Contact>();
                StartThread();
            }

        }

    }
}
