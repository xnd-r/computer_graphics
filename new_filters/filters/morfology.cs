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
        protected Color Dil(Bitmap sourceImage, int x, int y)
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
                        intensity = (int)(0.36 * color.R + 0.53 * color.G + 0.11 * color.B);
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
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
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
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            dilation dilat;
            erosion eros;
            dilat = new dilation();
            eros = new erosion();

            return eros.processImage(dilat.processImage(sourceImage, worker), worker);
        }
    }

    class top_hat : opening
    {
        public top_hat()
        {
            //kernel = null;
            
        }

        protected Color calculateNewPixelColor(Bitmap src, Bitmap edited, int x, int y)
        {
            Color minuend_color = src.GetPixel(x, y);
            Color substract_color = edited.GetPixel(x, y);
            return Color.FromArgb(
                clamp(minuend_color.R - substract_color.R, 0, 255),
                clamp(minuend_color.G - substract_color.G, 0, 255),
                clamp(minuend_color.B - substract_color.B, 0, 255));
        }

        Bitmap substraction(Bitmap src, Bitmap editedImage, BackgroundWorker worker)
        {
            Bitmap res = new Bitmap(src.Width, src.Height);
            for (int i = 0; i < src.Width; i++)
            {
                worker.ReportProgress((int)((float)i / res.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < src.Height; j++)
                {
                    res.SetPixel(i, j, calculateNewPixelColor(src, editedImage, i, j));
                }
            }
            return res;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap new_src = sourceImage;
            //dilation dilat;
            //erosion eros;
            //dilat = new dilation();
            //eros = new erosion();
            opening open = new opening();
            return substraction(new_src, open.processImage(sourceImage, worker), worker);
        }
    }
  }
