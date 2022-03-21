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

        private void button1_Click(object sender, EventArgs e)
        {
            int x = label2.Location.X + label2.Width;
            int y = label2.Location.Y;

            label2.Text = textBox1.Text;
            label2.Location = new Point(x - label2.Width, y);
            MyCli.label1.Text = textBox1.Text;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
