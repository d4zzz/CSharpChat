using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace IMDll
{
    public class Message
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }

        public DateTime MsgTime { get; set; }

        public Message() { }
    }
}
