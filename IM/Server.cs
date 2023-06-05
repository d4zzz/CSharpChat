using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IMDll;


namespace IMServer
{

    public class Server
    {
        private Socket serverSocket;
        private List<Socket> clientSockets;

        public event EventHandler<EventArgs> eventRefreshClients;
        public Server()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSockets = new List<Socket>();
        }

        public bool Start(string host, int port)
        {
            if (IPAddress.Parse(host) == null)
                return false;
            try
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse(host), port));
                serverSocket.Listen(10);
                IAsyncResult ar = serverSocket.BeginAccept(CallbackAccept, null);
                ThreadPool.QueueUserWorkItem(ProcessThreaed);
                return true;
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


        private void ProcessThreaed(object o)
        {
            bool clientChange = false;
            while (true)
            {
                    
                for (int i =0;i< clientSockets.Count;i++)
                {
                    if (!Tool.IsConnected(clientSockets[i]))
                    {
                        try
                        {
                            clientSockets[i].Shutdown(SocketShutdown.Both);
                        }
                        finally
                        {
                            clientSockets[i].Close();
                        }
                        clientSockets.Remove(clientSockets[i]);
                        clientChange = true;
                        
                    }
                }
                if (clientChange)
                {
                    RefreshClientList();
                    clientChange = false;
                }
            }
        }

        private void CallbackAccept(IAsyncResult ar)
        {
            Socket socket = serverSocket.EndAccept(ar);
            ReceiveData receiveData = new ReceiveData(socket, 1024);
            socket.BeginReceive(receiveData.Buffer, 0, 1024, SocketFlags.None, CallbackRcv, receiveData);
            clientSockets.Add(socket);
            RefreshClientList();
            serverSocket.BeginAccept(CallbackAccept, null);
        }



        private void CallbackRcv(IAsyncResult ar)
        {
            ReceiveData receiveData = ar.AsyncState as ReceiveData;
            int len;
            try
            {
                len = receiveData.Socket.EndReceive(ar);
            }
            catch (Exception ex) when (ex is ObjectDisposedException ||ex is SocketException)
            {
                return;
            }
            ProcessMsg(receiveData.Socket, receiveData.Buffer,len);
            receiveData.Socket.BeginReceive(receiveData.Buffer, 0, 1024, SocketFlags.None, CallbackRcv, receiveData);
        }

        private void ProcessMsg(Socket socket, byte[] buffer, int len)
        {
            foreach (var client in clientSockets)
            {
                if(client != socket)
                {
                    byte[] ip = Encoding.UTF8.GetBytes($"[{socket.RemoteEndPoint.ToString()}] ");
                    byte[] newData = new byte[ip.Length + buffer.Length];
                    ip.CopyTo(newData, 0);
                    buffer.CopyTo(newData, ip.Length);
                    client.Send(newData, ip.Length+len, SocketFlags.None);

                }
            }
        }

        private void RefreshClientList()
        {
            IEnumerable<string> source;
            source = clientSockets.Select(x => x.RemoteEndPoint.ToString());
            // 客户端列表刷新事件
            this.eventRefreshClients(this, new RefreshClientsEventArgs
            {
                Clients = source.ToArray()
            });
        }

        
    }
}
