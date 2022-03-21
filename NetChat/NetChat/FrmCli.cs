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

        private void button1_Click(object sender, EventArgs e)
        {
            int x = label2.Location.X + label2.Width;
            int y = label2.Location.Y;

            label2.Text = textBox1.Text;
            label2.Location = new Point(x - label2.Width, y);

            MySvr.label1.Text = textBox1.Text;
        }
    }
}
