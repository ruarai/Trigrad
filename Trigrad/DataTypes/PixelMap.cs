using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.DataTypes
{
    public class PixelMap
    {
        public PixelMap(int width, int height)
        {
            map = new Color[width, height];
            Width = width;
            Height = height;
        }

        public PixelMap(Bitmap b)
        {
            Width = b.Width;
            Height = b.Height;
            map = new Color[Width, Height];

            var data = b.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = Math.Abs(data.Stride) * b.Height;

            byte[] raw = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, raw, 0, bytes);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int offset = (y * data.Width + x) * 4;
                    if (offset > raw.Length)
                        break;

                    byte B = raw[offset];
                    byte G = raw[offset + 1];
                    byte R = raw[offset + 2];
                    byte A = raw[offset + 3];

                    map[x, y] = Color.FromArgb(R, G, B, A);
                }
            }

            b.UnlockBits(data);
        }


        private readonly Color[,] map;
        public readonly int Width;
        public readonly int Height;

        public Color this[int x, int y]
        {
            get { return map[x, y]; }
            set { map[x, y] = value; }
        }
        public Color this[Point p]
        {
            get { return map[p.X,p.Y]; }
            set { map[p.X,p.Y] = value; }
        }

        public Bitmap Bitmap
        {
            get
            {
                var bitmap = new Bitmap(Width, Height);

                var data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                int bytes = Math.Abs(data.Stride) * bitmap.Height;

                byte[] raw = new byte[bytes];

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        int offset = (y * data.Width + x) * 4;
                        if (offset > raw.Length)
                            break;

                        Color c = this[x, y];

                        raw[offset] = c.B;
                        raw[offset + 1] = c.G;
                        raw[offset + 2] = c.R;
                        raw[offset + 3] = c.A;
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(raw, 0, data.Scan0, bytes);
                bitmap.UnlockBits(data);

                return bitmap;
            }
        }

    }
}
