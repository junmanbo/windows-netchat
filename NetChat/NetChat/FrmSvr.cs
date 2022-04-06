using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace NetChat
{
    public partial class FrmSvr : Form
    {
        FrmCli MyCli;
        static FrmSvr MySvr;

        public FrmSvr()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void FrmSvr_Load(object sender, EventArgs e)
        {
            MySvr = this;
            MyCli = new FrmCli(this);
            MyCli.Show();
            MyCli.Location = new Point(this.Location.X + this.Width, this.Location.Y);
        }

        int lblPosY = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            // 표준라벨 작성(Hide) + 라벨 클론
            Label lbl = lblY.Clone();
            int x = lbl.Location.X + lbl.Width;

            // 라벨 추가
            panel1.Controls.Add(lbl);

            // 대화 표시
            lbl.Text = textBox1.Text;
            lbl.Visible = true;

            // 라벨 위치 지정
            lblPosY += lbl.Height;
            lbl.Location = new Point(x-lbl.Width, lblPosY);

            // 스크롤 맨 밑으로 내리기
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            panel1.PerformLayout();

            // 상대창에 대화표시
            // MyCli.putMsg(textBox1.Text);
            byte[] msg = Encoding.UTF8.GetBytes(textBox1.Text);
            mySocket.Send(msg);
        }
        public void putMsg(string msg)
        {
            // 표준라벨 작성(Hide) + 라벨 클론
            Label lbl = lblB;
            Invoke((MethodInvoker)delegate { lbl= lblB.Clone(); });
            int x = lbl.Location.X;

            // 라벨 추가
            panel1.VerticalScroll.Value = 0;
            panel1.Controls.Add(lbl);

            // 대화 표시
            lbl.Text = msg;
            lbl.Visible = true;

            // 라벨 위치 지정
            lblPosY += lbl.Height;
            lbl.Location = new Point(x, lblPosY);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string ip = "127.0.0.1";
            foreach (var ipa in host.AddressList)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = ipa.ToString();
                    if (ip.Split('.')[0] == "192")
                        return ip;
                }
            }
            return ip;
        }

        Socket listenSocket, mySocket;

        private void btnListen_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(StartListening);
            // Create the socket
            Socket sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            // Bind the listening socket to the port
            IPAddress hostIP = IPAddress.Parse(PublicIPAddress());
            //IPAddress hostIP = IPAddress.Parse("192.168.219.110");
            comboBox1.Text = hostIP.ToString();

            hostIP = IPAddress.Parse(GetLocalIPAddress());
            this.Text = hostIP.ToString();

            IPEndPoint ep = new IPEndPoint(hostIP, 5142);
            sock.Bind(ep);
            listenSocket = sock;

            // Start listening
            //listenSocket.Listen(5);
            btnListen.Hide();

            // 콤보박스에 ip주소 표기
            //sock.BeginAccept(new AsyncCallback(AcceptCallback), sock);

            t.Start(sock);

            // 버튼 숨기기
            btnAccept.Hide();
            btnReceive.Hide();
        }

        // Thread signal
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static void StartListening(object data)
        {
            Socket sock = (Socket)data;

            // start listening
            sock.Listen(5);
            try
            {
                while (true)
                {
                    allDone.Reset();
                    sock.BeginAccept(new AsyncCallback(AcceptCallback), sock);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        List<Socket> MySocketList = new List<Socket>();
        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue
            allDone.Set();

            try
            {
                // Get the socket that handles the client request
                Socket listener = (Socket)ar.AsyncState;
                Socket sock = listener.EndAccept(ar);
                StateObject state = new StateObject();

                MySvr.MySocketList.Add(sock);
                MySvr.comboBox1.Items.Add(sock.RemoteEndPoint);
                MySvr.comboBox1.SelectedIndex = MySvr.comboBox1.Items.Count - 1;
                state.workSocket = sock;
                MySvr.mySocket = sock;
                MySvr.btnSend.Enabled = true;

                sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);

            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object
            StateObject state = (StateObject)ar.AsyncState;
            Socket sock = state.workSocket;
            string msg = sock.RemoteEndPoint.ToString();

            try
            {
                // Read data from the client socket
                int bytesRead = sock.EndReceive(ar);
                MySvr.mySocket = sock;

                if (bytesRead > 0)
                {
                    //msg = msg.Split(':')[0] + ": " + Encoding.UTF8.GetString(state.buffer, 0, bytesRead);
                    msg = Encoding.UTF8.GetString(state.buffer, 0, bytesRead);
                    MySvr.putMsg(msg);
                    sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
            } catch (Exception e)
            {
                MySvr.putMsg("Error: Client disconnected!");
            }
        }

        public static string PublicIPAddress()
        {
            string uri = "http://checkip.dyndns.org/";
            string ip = "";
            var client = new HttpClient();
            string result = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
            try
            {
                ip = result.Split(':')[1].Split('<')[0].Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(result, "Error: IPAddress");
                ip = "127.0.0.1";
            }
            return ip;
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            byte[] bytes = new byte[256];
            mySocket.Receive(bytes);
            putMsg(Encoding.UTF8.GetString(bytes));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            mySocket = MySocketList[cmb.SelectedIndex];

        }

        private void FrmSvr_FormClosing(object sender, FormClosingEventArgs e)
        {
            listenSocket.Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            Socket sock = listenSocket.Accept();
            mySocket = sock;
            comboBox1.Text = sock.RemoteEndPoint.ToString();
        }
    }
}
