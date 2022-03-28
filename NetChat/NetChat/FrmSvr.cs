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
    public partial class FrmSvr : Form
    {
        FrmCli MyCli;
        public FrmSvr()
        {
            InitializeComponent();
        }

        private void FrmSvr_Load(object sender, EventArgs e)
        {
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

            // 상대창에 대화표시
            // MyCli.putMsg(textBox1.Text);
            byte[] msg = Encoding.UTF8.GetBytes(textBox1.Text);
            mySocket.Send(msg);
        }
        public void putMsg(string msg)
        {
            // 표준라벨 작성(Hide) + 라벨 클론
            Label lbl = lblB.Clone();
            int x = lbl.Location.X;

            // 라벨 추가
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

        Socket listenSocket, mySocket;
        private void btnListen_Click(object sender, EventArgs e)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            IPAddress hostIP = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(hostIP, 5142);
            sock.Bind(ep);
            listenSocket = sock;

            listenSocket.Listen(5);
            btnListen.Hide();

        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            byte[] bytes = new byte[256];
            mySocket.Receive(bytes);
            putMsg(Encoding.UTF8.GetString(bytes));
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            Socket sock = listenSocket.Accept();
            mySocket = sock;
            comboBox1.Text = sock.RemoteEndPoint.ToString();
        }
    }
}
