using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IMDll
{
    public class User
    {
        public int Id { get; set; }
        public IPAddress Address { get; set; }
        public string Name { get; set; }

        User() { }
    }
}
