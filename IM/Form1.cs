using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IMDll;

namespace IMServer
{
    public partial class IMServer : Form
    {
        private string serverNotRunning = "服务器状态：未启动";
        private string serverRunning = "服务器状态：已启动";
        private Server server;
        public IMServer()
        {
            InitializeComponent();
            server = new Server();
            server.eventRefreshClients += Server_eventRefreshClients;
        }

        private void Server_eventRefreshClients(object sender, EventArgs e)
        {
            RefreshClientsEventArgs re = (RefreshClientsEventArgs)e;
            if (lstClients.InvokeRequired)
            {
                lstClients.Invoke((MethodInvoker)delegate
                {
                    lstClients.Items.Clear();
                    ListBox.ObjectCollection items = lstClients.Items;
                    object[] clients = re.Clients;
                    items.AddRange(clients);
                });
            }
        }





        private void buttonStartServer_Click(object sender, EventArgs e)
        {
            if(numericUpDownPort.Validate())
            {
                
                server.Start("0.0.0.0",(int)numericUpDownPort.Value);
                numericUpDownPort.Enabled = false;
                buttonStartServer.Enabled = false;
                labelServerStatus.Text = serverRunning;
            }
            else
            {
                MessageBox.Show("端口号错误");
            }
        }


       
    }
}
