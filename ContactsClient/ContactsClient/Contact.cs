﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ContactsClient
{
   public  class Contact
    {
        public String id { get; set; }
        public String Name { get; set; }
        public String phoneNumbers { get; set; }   
    }
}


