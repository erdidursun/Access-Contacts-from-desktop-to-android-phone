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
        public List<Contact> m_contacts = new List<Contact>();
        public List<Contact> m_tmp = new List<Contact>();
        public static Client m_client;
        delegate void AddContactHandler(Control c);
        delegate void RemoveContactsHandler();
        RemoveContactsHandler RemoveContacts;
        Thread m_thread;
        AddContactHandler AddContact;

        public Contacts()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_client.GetContacts();
        }

        void client_Connected(bool success)
        {
            if (success)
            {
                this.Text = "Connection Has Been Established ";
                m_client.GetContacts();

            }
            else
            {
                MessageBox.Show("Connection Failed. ");
                this.Close();
            }
        }
        void ShowContact()
        {
            int y = 2;
            this.Text = "Contacts Loading....";
            this.Invoke(RemoveContacts);
            foreach (var c in m_tmp)
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
            m_tmp = m_contacts;
            this.Text = "Contacts Complete.";
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
            if (m_thread != null)
                m_thread.Abort();

            m_thread = new Thread(new ThreadStart(ShowContact));
            m_thread.SetApartmentState(ApartmentState.MTA);
            m_thread.Start();
        }
        void ContactAdded(List<Contact> m_contacts)
        {
            m_tmp = this.m_contacts = m_contacts.OrderBy(p => p.Name).ToList<Contact>();
            StartThread();
        }
        void ContactDelete(string id)
        {
            int index = m_contacts.FindIndex(p => p.id.Equals(id));
            string name = m_contacts[index].Name;
            m_contacts.RemoveAt(m_contacts.FindIndex(p => p.id.Equals(id)));
            MessageBox.Show(String.Format("{0} is Deleted", name), "Delete Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            StartThread();

        }
        void ContactUptade(string id, string name, string phone)
        {
            m_contacts[m_contacts.FindIndex(p => p.id == id)] = new Contact() { id = id, Name = name, phoneNumbers = phone };
            MessageBox.Show(String.Format("{0} is Updated", name), "Update Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            StartThread();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_client = new Client();
            AddContact = new AddContactHandler(Add);
            RemoveContacts = new RemoveContactsHandler(Remove);
            CheckForIllegalCrossThreadCalls = false;
            m_client.Connect();
            m_client.Connected += client_Connected;
            m_client.ContactsAdded += ContactAdded;
            m_client.ContactDelete += ContactDelete;
            m_client.ContactUptade += ContactUptade;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox1.SelectedIndex == 0)
            {
                m_tmp = m_tmp.Where<Contact>(p => m_tmp.Where(t => t.Name == p.Name).Count() > 1).ToList();
                if (m_tmp.Count == 0)
                {
                    MessageBox.Show("No records were found matching");
                }
                else
                    StartThread();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                m_tmp = m_tmp.Where<Contact>(p => m_tmp.Where(t => t.phoneNumbers == p.phoneNumbers).Count() > 1).ToList();
                if (m_tmp.Count == 0)
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
                m_tmp = m_tmp.Where(p => p.Name.ToLower().Contains(textBox1.Text.ToLower()) || p.phoneNumbers.Contains(textBox1.Text)).ToList<Contact>();
                StartThread();
            }

        }

    }
}
