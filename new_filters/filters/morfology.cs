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
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Color resultColor = Color.Black;

            byte max = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = clamp(y + l, 0, sourceImage.Height - 1);
                    Color color = sourceImage.GetPixel(idX, idY);
                    int intensity = color.R;
                    if ((color.R != color.G) || (color.R != color.B) || (color.G != color.B))
                    {
                        intensity = (int)(0.36 * color.R + 0.53 * color.G + 0.11 * color.R);
                    }
                    if (kernel[k + radiusX, l + radiusY] > 0 && intensity > max)
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
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            Color resultColor = Color.Black;

            byte min = 255;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = clamp(y + l, 0, sourceImage.Height - 1);
                    Color color = sourceImage.GetPixel(idX, idY);
                    int intensity = color.R;
                    if ((color.R != color.G) || (color.R != color.B) || (color.G != color.B))
                    {
                        intensity = (int)(0.36 * color.R + 0.53 * color.G + 0.11 * color.R);
                    }
                    if (kernel[k + radiusX, l + radiusY] > 0 && intensity < min)
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
            //if (kernel == null)
            //{
            dilat = new dilation();
            eros = new erosion();
            //}
            //else
            //{
            //    dilat = new dilation(kernel);
            //    eros = new erosion(kernel);
            //}
            return eros.processImage(dilat.processImage(sourceImage, worker), worker);
        }
    }
    class Subtract : Filters
    {
        Bitmap MinuendImage = null;
        public Subtract(Bitmap minuendImage)
        {
            MinuendImage = minuendImage;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color MinuendColor = MinuendImage.GetPixel(x, y);
            Color SubtractColor = sourceImage.GetPixel(x, y);
            return Color.FromArgb(clamp(MinuendColor.R - SubtractColor.R, 0, 255),
                                  clamp(MinuendColor.G - SubtractColor.G, 0, 255),
                                  clamp(MinuendColor.B - SubtractColor.B, 0, 255));
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
        public new Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            closing close;
            //if (this.kernel == null)
            //{
                close = new closing();
            //}
            //else
            //{
            //    close = new closing(kernel);
            //}
            Subtract subtraction = new Subtract(sourceImage);
            return subtraction.processImage(close.processImage(sourceImage, worker), worker);
        }
    }
  }
