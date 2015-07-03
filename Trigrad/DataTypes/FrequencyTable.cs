using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace Trigrad.DataTypes
{
    /// <summary> A frequency table defining how likely a sample will be formed during a TrigradCompression. </summary>
    public class FrequencyTable
    {
        /// <summary> The underlying 2D frequency table, with values lying from 0 to 1. </summary>
        public double[,] Table;

        /// <summary> Constructs a frequency table using sobel edge detection. </summary>
        public FrequencyTable(Bitmap bitmap)
        {
            SobelEdgeDetector detector = new SobelEdgeDetector();
            var edges = detector.Apply(colorToGrayscale(bitmap));

            Table = new double[bitmap.Width, bitmap.Height];

            for (int x = 0; x < edges.Width; x++)
            {
                for (int y = 0; y < edges.Height; y++)
                {
                    Table[x, y] = edges.GetPixel(x, y).R / 255d;
                }
            }
        }

        /// <summary> Constructs a frequency table using a specified table of values. </summary>
        public FrequencyTable(double[,] table)
        {
            Table = table;
        }

        private static Bitmap colorToGrayscale(Bitmap bmp)
        {
            int w = bmp.Width,
            h = bmp.Height,
            r, ic, oc, bmpStride, outputStride;
            PixelFormat pfIn = bmp.PixelFormat;
            ColorPalette palette;
            Bitmap output;
            BitmapData bmpData, outputData;

            //Create the new bitmap
            output = new Bitmap(w, h, PixelFormat.Format8bppIndexed);

            //Build a grayscale color Palette
            palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;

            //No need to convert formats if already in 8 bit
            if (pfIn == PixelFormat.Format8bppIndexed)
            {
                output = (Bitmap)bmp.Clone();

                //Make sure the palette is a grayscale palette and not some other
                //8-bit indexed palette
                output.Palette = palette;

                return output;
            }

            //Lock the images
            bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly,
            pfIn);
            outputData = output.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly,
            PixelFormat.Format8bppIndexed);
            bmpStride = bmpData.Stride;
            outputStride = outputData.Stride;

            //Traverse each pixel of the image
            unsafe
            {
                byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer(),
                outputPtr = (byte*)outputData.Scan0.ToPointer();

                //Convert the pixel to it's luminance using the formula:
                // L = .299*R + .587*G + .114*B
                //Note that ic is the input column and oc is the output column
                for (r = 0; r < h; r++)
                    for (ic = oc = 0; oc < w; ic += 3, ++oc)
                        outputPtr[r * outputStride + oc] = (byte)(int)
                        (0.299f * bmpPtr[r * bmpStride + ic] +
                        0.587f * bmpPtr[r * bmpStride + ic + 1] +
                        0.114f * bmpPtr[r * bmpStride + ic + 2]);

            }

            //Unlock the images
            bmp.UnlockBits(bmpData);
            output.UnlockBits(outputData);

            return output;
        }
    }
}
