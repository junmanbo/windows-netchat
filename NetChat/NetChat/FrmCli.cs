using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetChat
{
    public partial class FrmCli : Form
    {
        FrmSvr MySvr;
        public FrmCli(FrmSvr pf)
        {
            InitializeComponent();
            MySvr = pf;
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

            // 상대창에 대화표시
            MySvr.putMsg(textBox1.Text);
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
    }
}
