using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IMDll;

namespace IMClient
{
    public partial class Form1 : Form
    {
        private Client client;
        private delegate void ProcessMsgReceive(string msg);
        private event ProcessMsgReceive eventReceive;

        private const string ServerNotRunningText = "服务器未连接";
        private const string ServerRunningText = "服务器已连接";
        private const string ServerReconnectingText = "服务器已断开，正在重新连接";
        public Form1()
        {
            InitializeComponent();
            client = new Client();
            client.RefreshConnectStatus += Client_RefreshConnectStatus;
            client.DataReceive += Client_DataReceive;
        }

        private void Client_DataReceive(object sender, ReceiveEventArgs e)
        {
            ReceiveEventArgs re = e;
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.AppendText(re.Text + "\r\n");
            });
        }

        private void Client_RefreshConnectStatus(object sender, ConnectEventArgs e)
        {
            if (e.Connected)
            {
                Invoke((MethodInvoker)delegate
                {
                    labelServerStatus.Text = ServerRunningText;
                    textMsgInput.Enabled = true;
                    buttonSendMsg.Enabled = true;
                    buttonConnect.Enabled = false;
                    textServerAddr.Enabled = false;
                    progressBar1.Visible = false;
                });
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    progressBar1.Visible = true;
                    progressBar1.Value = 0;
                    progressBar1.Step = 80;
                    progressBar1.PerformStep();
                    labelServerStatus.Text = ServerReconnectingText;
                    textMsgInput.Enabled = false;
                    buttonSendMsg.Enabled = false;
                    buttonConnect.Enabled = false;
                    textServerAddr.Enabled = false;
                });
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string[] serverAddrPort = textServerAddr.Text.Split(':');
            if (serverAddrPort.Length != 2 || IPAddress.Parse(serverAddrPort[0]) == null || Convert.ToInt32(serverAddrPort[1]) == 0)
            {
                MessageBox.Show("Wrong Server Address");
                return;
            }
            try
            {
                client.Connect(serverAddrPort[0], Convert.ToInt32(serverAddrPort[1]));
                textMsgInput.Enabled = false;
                buttonSendMsg.Enabled = false;
                buttonConnect.Enabled = false;
                textServerAddr.Enabled = false;
                this.labelServerStatus.Text = "服务器正在连接";
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                progressBar1.Step = 80;
                progressBar1.PerformStep();
                //this.buttonConnect.Enabled = false;
                //this.textServerAddr.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void buttonSendMsg_Click(object sender, EventArgs e)
        {
            if(textMsgInput.Text != "")
            {
                client.SendMessage(textMsgInput.Text.Trim());
                richTextBox1.AppendText("Me:" + textMsgInput.Text.Trim() + "\r\n");
                textMsgInput.Clear();
            }
            else
            {
                MessageBox.Show("Message can not be empty");
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                buttonSendMsg_Click(sender, e);
                return;
            }
        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            labelServerStatus.Focus();
        }

        private void textMsgInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSendMsg_Click((TextBox)sender, e);
            }
        }
    }
}
              