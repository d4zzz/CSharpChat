using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IMDll
{
    public class ReceiveData
    {
        public Socket Socket { get; set; }

        public byte[] Buffer { get; set; }

        public ReceiveData(Socket socket, int bufferSize)
        {
            Socket = socket;
            this.Buffer = new byte[bufferSize];
        }
    }
}
