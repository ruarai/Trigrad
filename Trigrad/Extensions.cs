using TriangleNet.Data;
using System.Drawing;
using TriangleNet;

namespace Trigrad
{
    internal static class Extensions
    {
        public static Point Point(this Vertex t)
        {
            return new Point((int)t.X, (int)t.Y);
        }

        public static Bitmap ToBitmap(this Mesh mesh,int width,int height)
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);

            foreach (var tri in mesh.Triangles)
            {
                var pU = tri.GetVertex(0).Point();
                var pV = tri.GetVertex(1).Point();
                var pW = tri.GetVertex(2).Point();

                g.DrawLine(new Pen(Color.DarkSlateGray), pU, pV);
                g.DrawLine(new Pen(Color.DarkSlateGray), pV, pW);
                g.DrawLine(new Pen(Color.DarkSlateGray), pU, pW);
            }

            b.Save("mesh.png");
            return b;
        }
    }
}
