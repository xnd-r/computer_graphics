using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace filters
{

    public abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap src, int x, int y);

        public Bitmap processImage(Bitmap src, BackgroundWorker worker)
        {
            Bitmap res = new Bitmap(src.Width, src.Height);
            for (int i = 0; i < src.Width; i++)
            {
                worker.ReportProgress((int)((float)i / res.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < src.Height; j++)
                {
                    res.SetPixel(i, j, calculateNewPixelColor(src, i, j));
                }
            }
            return res;
        }

        public int clamp(int val, int min = 0, int max = 0)
        {
            return val < min ? min : (val > max ? max : val);
        }
    }

    public class invert_filter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap img, int x, int y)
        {
            Color src = img.GetPixel(x, y);
            Color res = Color.FromArgb(255 - src.R, 255 - src.G, 255 - src.B);
            return res;
        }
    }

    public class matrix_filter : Filters
    {
        protected float[,] kernel = null;
        protected matrix_filter() { }
        public matrix_filter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap src, int x, int y)
        {
            int range_x = kernel.GetLength(0) / 2;
            int range_y = kernel.GetLength(1) / 2;
            float res_R = 0;
            float res_G = 0;
            float res_B = 0;
            for (int l = -range_y; l <= range_y; l++)
                for (int k = -range_x; k <= range_x; k++)
                {
                    int id_x = clamp(x + k, 0, src.Width - 1);
                    int id_y = clamp(y + l, 0, src.Height - 1);
                    Color neighbor_color = src.GetPixel(id_x, id_y);
                    res_R += neighbor_color.R * kernel[k + range_x, l + range_y];
                    res_G += neighbor_color.G * kernel[k + range_x, l + range_y];
                    res_B += neighbor_color.B * kernel[k + range_x, l + range_y];
                }
            return Color.FromArgb
                (clamp((int)res_R, 0, 255), clamp((int)res_G, 0, 255), clamp((int)res_B, 0, 255));
        }
    }

    public class blur : matrix_filter
    {
        public blur()
        {
            int size_x = 3;
            int size_y = 3;
            kernel = new float[size_x, size_y];
            for (int i = 0; i < size_x; i++)
                for (int j = 0; j < size_y; j++)
                    kernel[i, j] = 1.0f / (float)(size_x * size_y);
        }
    }

    public class GaussFilter : matrix_filter
    {
        public GaussFilter()
        {
            create_gauss_kernel(3, 2);
        }

        private void create_gauss_kernel(int range, float sigma)
        {
            int size = 2 * range + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -range; i < range; i++)
                for (int j = -range; j < range; j++)
                {
                    kernel[i + range, j + range] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + range, j + range];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
    }

    public class black_and_white : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap Image, int x, int y)
        {
            Color src = Image.GetPixel(x, y);
            int gray_val = (int)(0.36 * src.R + 0.53 * src.G + 0.11 * src.B);
            return Color.FromArgb(gray_val, gray_val, gray_val);
        }
    }

    public class sepia : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap Image, int x, int y)
        {
            float k = 20f;
            Color src = Image.GetPixel(x, y);
            int sepia_val = (int)(0.36 * src.R + 0.53 * src.G + 0.11 * src.B);
            return Color.FromArgb
                (clamp((int)(sepia_val + 2 * k), 0, 255), clamp((int)(sepia_val + 0.5 * k), 0, 255), clamp((int)(sepia_val - k), 0, 255));
        }
    }

    public class brightness : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap Image, int x, int y)
        {
            int k = 15;
            Color src = Image.GetPixel(x, y);
            return Color.FromArgb
                (clamp(src.R + k, 0, 255), clamp(src.G + k, 0, 255), clamp(src.B + k, 0, 255));
        }
    }

    public class sobel : matrix_filter
    {
        protected override Color calculateNewPixelColor(Bitmap src, int x, int y)
        {
            kernel = new float[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            Color tmp_x = base.calculateNewPixelColor(src, x, y);
            kernel = new float[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            Color tmp_y = base.calculateNewPixelColor(src, x, y);
            return Color.FromArgb(
                clamp((int)(Math.Sqrt(tmp_x.R * tmp_x.R + tmp_y.R * tmp_y.R)), 0, 255),
                clamp((int)(Math.Sqrt(tmp_x.G * tmp_x.G + tmp_y.G * tmp_y.G)), 0, 255),
                clamp((int)(Math.Sqrt(tmp_x.B * tmp_x.B + tmp_y.B * tmp_y.B)), 0, 255));
        }
    }

    public class sharpness : matrix_filter
    {
        public sharpness()
        {
            int size_x = 3;
            int size_y = 3;
            kernel = new float[size_x, size_y];
            kernel[0, 0] = -1; kernel[0, 1] = -1; kernel[0, 2] = -1;
            kernel[1, 0] = -1; kernel[1, 1] = 9; kernel[1, 2] = -1;
            kernel[2, 0] = -1; kernel[2, 1] = -1; kernel[2, 2] = -1;
        }
    }

    class horiz_waves : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap src_img, int x, int y)
        {
            return src_img.GetPixel(
                x, clamp((int)(y + 20 * Math.Sin(2 * Math.PI * y / 30)), 0, src_img.Height - 1));
        }
    }

    class vert_waves : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap src_img, int x, int y)
        {
            return src_img.GetPixel(
                clamp((int)(x + 20 * Math.Sin(2 * Math.PI * x / 60)), 0, src_img.Width - 1), y);
        }

    }

    class glass : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap src_img, int x, int y)
        {
            Random rand = new Random();
            return src_img.GetPixel(
                clamp((int)(x + (rand.Next(1) - 0.5) * 10), 0, src_img.Width - 1), 
                clamp((int)(y + (rand.Next(1) - 0.5) * 10), 0, src_img.Height - 1));
        }
    }
    class gray_world : Filters
    {
        float avg_r = 0f, avg_g = 0f, avg_b =  0f;

        public gray_world(Bitmap src_img)
        { 
            for (int i = 0; i < src_img.Width; ++i)
                for (int j = 0; j < src_img.Height; ++j)
                {
                    avg_r += src_img.GetPixel(i, j).R;
                    avg_g += src_img.GetPixel(i, j).G;
                    avg_b += src_img.GetPixel(i, j).B;
                }
            int tmp_2 = src_img.Width * src_img.Height;
            avg_r /= tmp_2;
            avg_g /= tmp_2;
            avg_b /= tmp_2;
        }
        protected override Color calculateNewPixelColor(Bitmap src_img, int x, int y)
        {
            float average = (avg_r + avg_g + avg_r) / 3;
            Color scr = src_img.GetPixel(x, y);
            return Color.FromArgb(
                clamp((int)(scr.R * average / avg_r), 0, 255), 
                clamp((int)(scr.G * average / avg_g), 0, 255), 
                clamp((int)(scr.B * average / avg_b), 0, 255));
        }
    }

    class median : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap src_img, int x, int y)
        {
            int range = 3;
            int size = (2 * range + 1) * (2 * range + 1);
            int[] tmp_r = new int[size], tmp_g = new int[size], tmp_b = new int[size];
            
            for (int i = 0, l = -range; l <= range; ++l)
                for (int k = -range; k <= range; ++k)
                {
                    int id_x = clamp(x + k, 0, src_img.Width - 1);
                    int id_y = clamp(y + l, 0, src_img.Height - 1);
                    Color neighbor_color = src_img.GetPixel(id_x, id_y);
                    tmp_r[i] = neighbor_color.R;
                    tmp_g[i] = neighbor_color.G;
                    tmp_b[i] = neighbor_color.B;
                    i++;
                }
            Array.Sort(tmp_r);
            Array.Sort(tmp_g);
            Array.Sort(tmp_b);
            return Color.FromArgb(tmp_r[size / 2], tmp_g[size / 2], tmp_b[size / 2]);
        }
    }



}
