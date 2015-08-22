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
            Width = width;
            Height = height;
            map = new Color[Width, Height];
            BPP = 4;
            format = PixelFormat.Format32bppArgb;
        }

        public PixelMap(PixelMap original)
        {
            Width = original.Width;
            Height = original.Height;
            map = new Color[Width, Height];

            BPP = original.BPP;
            format = original.format;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    this[x, y] = original[x, y];
                }
            }
        }

        public PixelMap(Bitmap b)
        {
            Width = b.Width;
            Height = b.Height;
            map = new Color[Width, Height];
            format = b.PixelFormat;


            var data = b.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, format);

            switch (b.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                    BPP = 4;
                    break;
                case PixelFormat.Format24bppRgb:
                    BPP = 3;
                    break;
            }


            int bytes = Math.Abs(data.Stride) * b.Height;

            byte[] raw = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, raw, 0, bytes);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int offset = (y * data.Width + x) * BPP;

                    byte B = raw[offset];
                    byte G = raw[offset + 1];
                    byte R = raw[offset + 2];
                    if (BPP == 4)
                    {
                        byte A = raw[offset + 3];
                        map[x, y] = Color.FromArgb(R, G, B, A);
                    }
                    else
                    {
                        map[x, y] = Color.FromArgb(R, G, B);
                    }

                }
            }

            b.UnlockBits(data);
        }


        private readonly Color[,] map;
        public readonly int Width;
        public readonly int Height;
        private readonly int BPP = 4;
        private readonly PixelFormat format;

        public Color this[int x, int y]
        {
            get
            {
                if (Inside(new Point(x, y)))
                    return map[x, y];
                return Color.HotPink;
            }
            set
            {
                if (Inside(new Point(x, y)))
                    map[x, y] = value;
            }
        }

        public Color this[Point p]
        {
            get { return this[p.X, p.Y]; }
            set { this[p.X, p.Y] = value; }
        }

        public Color this[int i]
        {
            get { return map[i / Height, i % Height]; }
            set { map[i / Height, i % Height] = value; }
        }

        public void DrawLine(Point a, Point b, Color c)
        {
            int dx = Math.Abs(b.X - a.X), sx = a.X < b.X ? 1 : -1;
            int dy = -Math.Abs(b.Y - a.Y), sy = a.Y < b.Y ? 1 : -1;
            int err = dx + dy, e2;

            while (true)
            {
                if (Inside(a))
                    this[a.X, a.Y] = c;

                if (a.X == b.X && a.Y == b.Y) break;

                e2 = 2 * err;

                if (e2 > dy)
                {
                    err += dy;
                    a.X += sx;
                }
                else if (e2 < dx)
                {
                    err += dx;
                    a.Y += sy;
                }
            }
        }

        public bool Inside(Point p)
        {
            return (p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height);
        }

        public Bitmap Bitmap
        {
            get
            {
                var bitmap = new Bitmap(Width, Height);

                var data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);
                int bytes = Math.Abs(data.Stride) * bitmap.Height;

                byte[] raw = new byte[bytes];

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        int offset = (y * data.Width + x) * BPP;

                        Color c = this[x, y];

                        raw[offset] = c.B;
                        raw[offset + 1] = c.G;
                        raw[offset + 2] = c.R;
                        if (BPP == 4)
                            raw[offset + 3] = c.A;
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(raw, 0, data.Scan0, bytes);
                bitmap.UnlockBits(data);

                return bitmap;
            }
        }

        public static PixelMap SlowLoad(Bitmap b)
        {
            PixelMap m = new PixelMap(b.Width,b.Height);

            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    m[x, y] = b.GetPixel(x, y);
                }
            }

            return m;
        }

    }
}
