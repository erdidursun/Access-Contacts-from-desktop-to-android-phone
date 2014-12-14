using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ContactsClient
{
    enum ClientCommandType {
        getAllContact,
        updateContact,
        deleteContact        
    }
    enum ServerCommandType { 
        StartContact,
        EndContact,
        Update,
        Delete
    }
    class Utilities
    {
        public static char splitChar = '$';
        private static string CommandFormat = String.Concat("{0}",splitChar,"{1}", Environment.NewLine);
        public static String updateContactString(Contact contact)
        {
            return String.Format(CommandFormat, ClientCommandType.updateContact, JsonConvert.SerializeObject(contact));
        }
        public static String getAllContactString()
        {
            return String.Format(CommandFormat, ClientCommandType.getAllContact, " ");
        }
        public static String deleteContactString(string id)
        {
            return String.Format(CommandFormat, ClientCommandType.deleteContact, id);
        }
    }
}
