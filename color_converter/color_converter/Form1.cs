using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace color_converter
{
    public partial class Form1 : Form
    {
        color_conv cc = new color_conv();
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string img_path;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "image files|*.jpg; *.png; *.bpm; *.jpeg";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                img_path = dialog.FileName;
                cc.rgb_img = new Bitmap(img_path);
                this.pictureBox1.Image = cc.rgb_img;
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            cc.rgb_img_to_hsv();
            int width = cc.rgb_img.Width;
            int heigh = cc.rgb_img.Height;

            for (int i = 0; i < width; ++i)
                for (int j = 0; j < heigh; ++j)
                {
                    int tmpH = cc.hsv_img[i, j].h + trackBar1.Value;
                    if (tmpH < 0)
                        cc.hsv_img[i, j].h = (ushort)(tmpH + 360);
                    else if (tmpH >= 360)
                        cc.hsv_img[i, j].h = (ushort)(tmpH - 360);
                    else
                        cc.hsv_img[i, j].h = (ushort)tmpH;
                }

            trackBar1.Value = 0;
            cc.hsv_img_to_grb();
            pictureBox1.Image = cc.rgb_img;
            this.Cursor = Cursors.Default;


        }
    }

    public struct HSV_color
    {
        public ushort h;
        public byte s;
        public byte v;

        public HSV_color(ushort _h, byte _s, byte _v) /*: h(_h), s(_s), v(_v)*/
        {
            h = _h;
            s = _s;
            v = _v;
        }


    }

    public class color_conv
    {
        public Bitmap rgb_img;
        public HSV_color[,] hsv_img;

        public HSV_color rgb_to_hsv(Color source)
        {
            float eps = 0.00001f;
            ushort hue = 0;
            byte saturation = 0, value = 0;
            float R_ = source.R / 255f;
            float G_ = source.G / 255f;
            float B_ = source.B / 255f;
            float Cmax = Math.Max(R_, Math.Max(G_, B_));
            float Cmin = Math.Min(R_, Math.Min(G_, B_));
            float delta = Cmax - Cmin;

            value = (byte)(100 * Cmax);

            if (Cmax < eps)
                saturation = 0;
            else saturation = (byte)(100 * delta / Cmax);

            if (delta < eps)
                hue = 0;
            else if (Cmax == R_)
                hue = (ushort)(60f * (((G_ - B_) / delta) % 6f));
            else if (Cmax == R_)
                hue = (ushort)(60f * (((B_ - R_) / delta) % 2f));
            else if (Cmax == R_)
                hue = (ushort)(60f * (((R_ - G_) / delta) % 4f));

            return new HSV_color(clamp(hue, 0, 359), saturation, value);

        }

        public Color hsv_to_rgb(HSV_color source)
        {
            float R_ = 0f, G_ = 0f, B_ = 0f;
            float C = source.s * source.v / 100f / 100f;
            float X = C * (1 - Math.Abs((source.h / 60f) % 2f - 1));
            float m = source.v / 100f - C;

            if (source.h < 60)
            { R_ = C; G_ = X; B_ = 0f; }
            else if (source.h < 120)
            { R_ = X; G_ = C; B_ = 0f; }
            else if (source.h < 180)
            { R_ = 0f; G_ = C; B_ = X; }
            else if (source.h < 240)
            { R_ = 0f; G_ = X; B_ = C; }
            else if (source.h < 300)
            { R_ = X; G_ = 0f; B_ = C; }
            else if (source.h < 360)
            { R_ = C; G_ = 0f; B_ = X; }

            byte R = (byte)(255f * (R_ + m));
            byte G = (byte)(255f * (G_ + m));
            byte B = (byte)(255f * (B_ + m));

            return Color.FromArgb(clamp(R, 0, 255), clamp(G, 0, 255), clamp(B, 0, 255));

        }

        byte clamp(byte val, byte min, byte max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        ushort clamp(ushort val, ushort min, ushort max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public void rgb_img_to_hsv()
        {
            int width = rgb_img.Width;
            int heigh = rgb_img.Height;
            hsv_img = new HSV_color[width, heigh];
            for (int i = 0; i < width; ++i)
                for(int j = 0; j < heigh; ++j)
                {
                    hsv_img[i, j] = rgb_to_hsv(rgb_img.GetPixel(i, j));                        
                }
        }

        public void hsv_img_to_grb()
        {
            int width = hsv_img.GetLength(0);
            int heigh = hsv_img.GetLength(1);
            rgb_img = new Bitmap(width, heigh);
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < heigh; ++j)
                {
                    rgb_img.SetPixel(i, j, hsv_to_rgb(hsv_img[i, j]));
                }
        }
    }
  
}
