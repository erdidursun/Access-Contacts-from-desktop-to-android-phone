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
    public static class Client
    {
        #region variables
        private static TcpClient client;
        private static Socket clientSocket;
        private static int receiveDataSize = 20000;
        static byte[] receiveData;
        static byte[] sendData;
        static bool readContact = false;
        static string json = "";
        #endregion
        #region properties
        public static string IpAddress { get; set; }
        public static int Port { get; set; }
        #endregion
        #region events
        public delegate void ConnectHandler(bool success);
        public static event ConnectHandler Connected;
        public delegate void ContactsAddedHandler(List<Contact> contacts);
        public static event ContactsAddedHandler ContactsAdded;
        public delegate void ContactUptadedHandler(string id, string name, string phone);
        public static event ContactUptadedHandler ContactUptade;
        public delegate void ContactDeletedHandler(string id);
        public static event ContactDeletedHandler ContactDelete;
        #endregion
        #region functions
        public static void GetContacts()
        {
            while (!client.Connected) { }
            WriteData(Utilities.getAllContactString());
        }
        public static void Connect()
        {
            client = new TcpClient();
            client.BeginConnect(IpAddress, Port, ConnectionComplete, client);
        }
        static void ReceiveData()
        {
            receiveData = new byte[receiveDataSize];
            clientSocket.BeginReceive(receiveData, 0, receiveDataSize, SocketFlags.None, new AsyncCallback(ReceiveComplete), clientSocket);
        }
        static void InspectData(string data)
        {
            if (readContact)
            {
                if (data.Equals(ServerCommandType.EndContact.ToString()))
                {
                    ContactsAdded((List<Contact>)JsonConvert.DeserializeObject(json, typeof(List<Contact>)));
                    readContact = false;
                }
                else
                    json = String.Concat(json, data);
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
                readContact = true;
            }
        }
        public static void WriteData(string data)
        {
            try
            {
                sendData = Encoding.UTF8.GetBytes(data);
                clientSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(WriteComplete), clientSocket);
            }
            catch (Exception ex)
            {

                Console.WriteLine("WriteData:\n" + ex.Message);
            }

        }
        #endregion
        #region callbacks
        static void ConnectionComplete(IAsyncResult result)
        {
            try
            {
                TcpClient Client = (TcpClient)result.AsyncState;
                Client.EndConnect(result);
                clientSocket = Client.Client;
                ReceiveData();
                Connected(true);
            }
            catch (Exception)
            {
                Connected(false);
            }

        }
        static void ReceiveComplete(IAsyncResult result)
        {
            try
            {
                Socket client = (Socket)result.AsyncState;
                int dataCount = client.EndReceive(result);
                if (dataCount > 0)
                    InspectData(Encoding.UTF8.GetString(receiveData, 0, dataCount));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.InnerException);
                ReceiveData();
            }
            ReceiveData();
        }
        static void WriteComplete(IAsyncResult result)
        {
            Socket client = (Socket)result.AsyncState;
            client.EndSend(result);
        }
        #endregion
    }
}
