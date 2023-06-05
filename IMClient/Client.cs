using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using IMDll;

namespace IMClient
{
    
    internal class Client
    {
        private Socket clientSocket;
        private bool serverConnectionStatus = false;
        
        public event EventHandler<ConnectEventArgs> RefreshConnectStatus = null;
        public event EventHandler<ReceiveEventArgs> DataReceive = null;

        public Client( ) 
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }




        public void Connect(string host, int port)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                serverConnectionStatus = false;
                while (true)
                {
                    if (Tool.IsConnected(clientSocket))
                    {
                        if (!serverConnectionStatus)
                        {
                            serverConnectionStatus = true;
                            this.RefreshConnectStatus(this, new ConnectEventArgs
                            {
                                Connected = true
                            });
                            ReceiveData receiveState = new ReceiveData(clientSocket, 1024);
                            clientSocket.BeginReceive(receiveState.Buffer, 0, 1024, SocketFlags.None, ReceiveCallback, receiveState);
                        }
                    }
                    else
                    {
                        if (serverConnectionStatus)
                        {
                            serverConnectionStatus = false;
                            this.RefreshConnectStatus(this, new ConnectEventArgs
                            {
                                Connected = false
                            });
                        }
                        try
                        {
                            clientSocket.Connect(host, port);
                        }
                        catch (SocketException)
                        {
                        }
                        catch (InvalidOperationException)
                        {
                            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        }
                    }
                }
            });
        }



        public bool SendMessage(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                try
                {
                    // 将消息转换为字节数组并发送给服务器
                    byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
                    clientSocket.Send(messageBytes);
                    return true;

                }
                catch (Exception ex) when (ex is ObjectDisposedException || ex is SocketException) { }
                {
                    return false;
                }
            }
            return false;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            ReceiveData receiveState = ar.AsyncState as ReceiveData;
            int len;
            try
            {
                len = receiveState.Socket.EndReceive(ar);
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is SocketException)
            {
                return;
            }
            ProcessReceivedData(receiveState.Socket, receiveState.Buffer, len);
            receiveState.Socket.BeginReceive(receiveState.Buffer, 0, 1024, SocketFlags.None, ReceiveCallback, receiveState);
        }

        private void ProcessReceivedData(Socket socket, byte[] buffer, int len)
        {
            this.DataReceive(this, new ReceiveEventArgs
            {
                Text = Encoding.UTF8.GetString(buffer, 0, len)
            });
        }


    }

    
}
