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
    abstract class morfology : matrix_filter
    {
        public morfology()
        {
            kernel = new float[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
        }
        public morfology(float[,] struct_elem)
        {
            kernel = struct_elem;
        }
    }
    class dilation : morfology
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int range_x = kernel.GetLength(0) / 2;
            int range_y = kernel.GetLength(1) / 2;

            Color resultColor = Color.Black;

            byte max = 0;
            for (int l = -range_y; l <= range_y; l++)
                for (int k = -range_x; k <= range_x; k++)
                {
                    int id_x = clamp(x + k, 0, sourceImage.Width - 1);
                    int id_y = clamp(y + l, 0, sourceImage.Height - 1);
                    Color color = sourceImage.GetPixel(id_x, id_y);
                    int intensity = color.R;
                    if ((color.R != color.G) || (color.R != color.B) || (color.G != color.B))
                    {
                        intensity = (int)(0.36 * color.R + 0.53 * color.G + 0.11 * color.B);
                    }
                    if (kernel[k + range_x, l + range_y] > 0 && intensity > max)
                    {
                        max = (byte)intensity;
                        resultColor = color;
                    }
                }
            return resultColor;
        }
    }
    class erosion : morfology
    {
        protected override Color calculateNewPixelColor(Bitmap src_img, int x, int y)
        {
            int range_x = kernel.GetLength(0) / 2;
            int range_y = kernel.GetLength(1) / 2;

            Color resultColor = Color.White;

            byte min = 255;
            for (int l = -range_y; l <= range_y; l++)
                for (int k = -range_x; k <= range_x; k++)
                {
                    int id_x = clamp(x + k, 0, src_img.Width - 1);
                    int id_y = clamp(y + l, 0, src_img.Height - 1);
                    Color color = src_img.GetPixel(id_x, id_y);
                    int intensity = color.R;
                    if ((color.R != color.G) || (color.R != color.B) || (color.G != color.B))
                    {
                        intensity = (int)(0.36 * color.R + 0.53 * color.G + 0.11 * color.B);
                    }
                    if (kernel[k + range_x, l + range_y] > 0 && intensity < min)
                    {
                        min = (byte)intensity;
                        resultColor = color;
                    }
                }
            return resultColor;
        }
    }

    class opening : morfology
    {
        public new Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            dilation dilat;
            erosion eros;

            dilat = new dilation();
            eros = new erosion();
            return dilat.processImage(eros.processImage(sourceImage, worker), worker);
        }
    }

    class closing : morfology
    {
        public new Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            dilation dilat;
            erosion eros;
            dilat = new dilation();
            eros = new erosion();

            return eros.processImage(dilat.processImage(sourceImage, worker), worker);
        }
    }
    class subtract : morfology
    {
        Bitmap minuend_img = null;
        public subtract() {}

        public subtract(Bitmap min_img)
        {
            minuend_img = min_img;
        }
        protected override Color calculateNewPixelColor(Bitmap src_img, int x, int y)
        {
            Color minuend_color = minuend_img.GetPixel(x, y);
            Color substract_color = src_img.GetPixel(x, y);
            return Color.FromArgb(
                clamp(minuend_color.R - substract_color.R, 0, 255),
                clamp(minuend_color.G - substract_color.G, 0, 255),
                clamp(minuend_color.B - substract_color.B, 0, 255));
        }
    }

    class top_hat : morfology
    {
        public top_hat()
        {
            kernel = null;
        }
        public top_hat(float[,] kernel)
        {
            this.kernel = kernel;
        }
        public new Bitmap processImage(Bitmap src_img, BackgroundWorker worker)
        {
            closing close = new closing();
            //}
            //else
            //{
            //    close = new closing(kernel);
            //}
            subtract subtraction = new subtract(src_img);
            return subtraction.processImage(close.processImage(src_img, worker), worker);
        }
    }
  }
