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

namespace NetChat
{
    public partial class FrmCli : Form
    {
        FrmSvr MySvr;
        static FrmCli MyCli;

        public FrmCli(FrmSvr pf)
        {
            InitializeComponent();
            MySvr = pf;
            MyCli = this;
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
            lbl.Location = new Point(x - lbl.Width, lblPosY);

            // 스크롤 맨 밑으로 내리기
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            panel1.PerformLayout();

            // 상대창에 대화표시
            // MySvr.putMsg(textBox1.Text);
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

        Socket mySocket;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            // create the socket
            Socket sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            // bind the listening socke to the port
            IPAddress hostIP = IPAddress.Parse(textBox2.Text);
            IPEndPoint ep = new IPEndPoint(hostIP, 5142);
            StateObject state = new StateObject();

            sock.Connect(ep);
            mySocket = sock;
            state.workSocket = sock;

            // 버튼 숨기기
            btnConnect.Hide();
            btnReceive.Hide();

            sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object
            StateObject state = (StateObject)ar.AsyncState;
            Socket sock = state.workSocket;

            try
            {
                // Read data from the client socket
                int bytesRead = sock.EndReceive(ar);
                MyCli.mySocket = sock;

                if (bytesRead > 0)
                {
                    MyCli.putMsg(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
            } catch(Exception e)
            {
                MyCli.putMsg("Error: Server disconnected!");
            }

        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            byte[] bytes = new byte[256];
            mySocket.Receive(bytes);
            putMsg(Encoding.UTF8.GetString(bytes));
        }

        private void FrmCli_Load(object sender, EventArgs e)
        {

        }
    }
}
