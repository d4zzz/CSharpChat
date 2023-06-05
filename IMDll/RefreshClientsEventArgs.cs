using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDll
{
    public class RefreshClientsEventArgs : EventArgs
    {
        public string[] Clients { get; set; } = default;
    }
}
