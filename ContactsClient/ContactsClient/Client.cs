using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ContactsClient
{
    public class Client
    {
        #region variables
        private TcpClient m_client;
        private Socket m_clientSocket;
        private int m_m_receiveDataSize = 20000;
        byte[] m_receiveData;
        byte[] m_sendData;
        bool m_readContact = false;
        string m_json = "";
        #endregion
        #region properties
        public static string IpAddress { get; set; }
        public static int Port { get; set; }
        #endregion
        #region events
        public delegate void ConnectHandler(bool success);
        public event ConnectHandler Connected;
        public delegate void ContactsAddedHandler(List<Contact> contacts);
        public event ContactsAddedHandler ContactsAdded;
        public delegate void ContactUptadedHandler(string id, string name, string phone);
        public event ContactUptadedHandler ContactUptade;
        public delegate void ContactDeletedHandler(string id);
        public event ContactDeletedHandler ContactDelete;
        #endregion
        #region functions
        public void GetContacts()
        {
            while (!m_client.Connected) { }
            WriteData(Utilities.getAllContactString());
        }
        public void Connect()
        {
            m_client = new TcpClient();
            m_client.BeginConnect(IpAddress, Port, ConnectionComplete, m_client);
        }
        void ReceiveData()
        {
            m_receiveData = new byte[m_m_receiveDataSize];
            m_clientSocket.BeginReceive(m_receiveData, 0, m_m_receiveDataSize, SocketFlags.None, new AsyncCallback(ReceiveComplete), m_clientSocket);
        }
        void InspectData(string data)
        {
            if (m_readContact)
            {
                if (data.Equals(ServerCommandType.EndContact.ToString()))
                {
                    ContactsAdded((List<Contact>)JsonConvert.DeserializeObject(m_json, typeof(List<Contact>)));
                    m_readContact = false;
                }
                else
                    m_json = String.Concat(m_json, data);
            }
            else
            {
                string part1 = data.Split(Utilities.splitChar)[0];
                if (part1.Equals(ServerCommandType.Update.ToString()))
                    ContactUptade(data.Split(Utilities.splitChar)[1], data.Split(Utilities.splitChar)[2], data.Split(Utilities.splitChar)[3]);
                else if (part1.Equals(ServerCommandType.Delete.ToString()))
                    ContactDelete(data.Split(Utilities.splitChar)[1]);

            }
            if (data.Equals(ServerCommandType.StartContact.ToString()))
            {
                m_readContact = true;
            }
        }
        public void WriteData(string data)
        {
            try
            {
                m_sendData = Encoding.UTF8.GetBytes(data);
                m_clientSocket.BeginSend(m_sendData, 0, m_sendData.Length, SocketFlags.None, new AsyncCallback(WriteComplete), m_clientSocket);
            }
            catch (Exception ex)
            {

                Console.WriteLine("WriteData:\n" + ex.Message);
            }

        }
        #endregion
        #region callbacks
        void ConnectionComplete(IAsyncResult result)
        {
            try
            {
                TcpClient Client = (TcpClient)result.AsyncState;
                Client.EndConnect(result);
                m_clientSocket = Client.Client;
                ReceiveData();
                Connected(true);
            }
            catch (Exception)
            {
                Connected(false);
            }

        }
        void ReceiveComplete(IAsyncResult result)
        {
            try
            {
                Socket m_client = (Socket)result.AsyncState;
                int dataCount = m_client.EndReceive(result);
                if (dataCount > 0)
                    InspectData(Encoding.UTF8.GetString(m_receiveData, 0, dataCount));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.InnerException);
                ReceiveData();
            }
            ReceiveData();
        }
        void WriteComplete(IAsyncResult result)
        {
            Socket m_client = (Socket)result.AsyncState;
            m_client.EndSend(result);
        }
        #endregion
    }
}
