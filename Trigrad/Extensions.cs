using System.Collections.Generic;
using TriangleNet.Data;
using System.Drawing;
using TriangleNet;
using Trigrad.DataTypes;

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
