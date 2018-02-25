using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace filters
{
    public partial class Form1 : Form
    {
        Bitmap image;
 
        public Form1()
        {
            InitializeComponent();
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new invert_filter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void dottedToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files (*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap new_img = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = new_img;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void blackWhiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new black_and_white();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new sepia();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void brightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new brightness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void sharpnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new sharpness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new sobel();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void gaussToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void blurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new blur();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void horisonttalWavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new horiz_waves();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void verticalWavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new vert_waves();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
