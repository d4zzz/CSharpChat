using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IMDll
{
    public class Tool
    {
        public static  bool IsConnected(Socket socket)
        {
            bool result = false;
            bool blocking = socket.Blocking;
            try
            {
                byte[] bytes = new byte[0];
                socket.Blocking = false;
                socket.Send(bytes, 0, SocketFlags.None);
                return true;
            }
            catch (SocketException ex)
            {
                if (ex.NativeErrorCode.Equals(10035))
                    result = true;
            }
            finally
            {
                socket.Blocking = blocking;
            }
            return result;
        }
    }
}
