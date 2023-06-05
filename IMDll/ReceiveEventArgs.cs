using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IMDll
{
    public class ReceiveEventArgs:EventArgs
    {
        public string Text { get; set; } = string.Empty;
    }
}
