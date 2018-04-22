using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tomogram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int min_value = 0;
        int TF_value = 1;
        Bin bin = new Bin();
        int currentLayer;
        View view = new View();
        bool loaded = false;
        bool needReload = false;
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.ReadBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                //trackBar1.Maximum = Bin.Z - 1;
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if(loaded)
            {
                view.DrawQuads(currentLayer);
                glControl1.SwapBuffers();
            }
        }
    }
}
