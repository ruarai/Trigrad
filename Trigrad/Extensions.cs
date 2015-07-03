using TriangleNet.Data;
using System.Drawing;

namespace Trigrad
{
    internal static class Extensions
    {
        public static Point Point(this Vertex t)
        {
            return new Point((int)t.X, (int)t.Y);
        }
    }
}
