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
        Stack<Bitmap> BackUp = new Stack<Bitmap>();

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
            BackUp.Clear();
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
            {
                BackUp.Push(image);
                image = new_img;
            }
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

        private void glassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new glass();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPeg Image| *.jpg | Bitmap Image | *.bmp | Portable network graphics| *.png | All files (*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image.Save(dialog.FileName);
            }

        }

        private void grayWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new gray_world(image);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void filtersToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new median();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void medianToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Filters filter = new median();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void shiftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new shift();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void elongationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new lin_elongation(image);
            backgroundWorker1.RunWorkerAsync(filter);

        }

        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new dilation();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void erosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new erosion();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void openingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new opening();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void closingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new closing();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void topHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new top_hat();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BackUp.Count > 0) backupToolStripMenuItem.Enabled = true;
            else backupToolStripMenuItem.Enabled = false;
        }

        private void backupToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            image = BackUp.Pop();
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }
    }
}
