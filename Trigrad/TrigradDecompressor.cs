using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;
using Trigrad.DataTypes;
using Point = System.Drawing.Point;

namespace Trigrad
{
    /// <summary> Holds methods for decompressing trigrad compressed imagery. </summary>
    public static class TrigradDecompressor
    {
        /// <summary> Decompresses a trigrad compressed bitmap. </summary>
        /// <param name="compressionData"> The TrigradCompressed data.</param>
        /// <param name="colorMode"> Optional mode for specifying how output colors will be chosen.</param>
        public static TrigradDecompressed DecompressBitmap(TrigradCompressed compressionData, ColorMode colorMode = ColorMode.Gradient)
        {
            TrigradDecompressed decompressed = new TrigradDecompressed(compressionData.Width, compressionData.Height);

            Mesh mesh = buildMesh(compressionData.SampleTable);

            Parallel.ForEach(mesh.Triangles, triangle =>
            {
                var vU = triangle.GetVertex(0);
                var vV = triangle.GetVertex(1);
                var vW = triangle.GetVertex(2);

                var points = TriangleRasterization.PointsInTriangle(vU.Point(), vV.Point(), vW.Point());

                Color cU;
                Color cV;
                Color cW;

                lock (compressionData)
                {
                    cU = compressionData.SampleTable[vU.Point()];
                    cV = compressionData.SampleTable[vV.Point()];
                    cW = compressionData.SampleTable[vW.Point()];
                }


                foreach (var point in points)
                {
                    var coords = Barycentric.GetCoordinates(point, vU, vV, vW);

                    Color gradedColor;

                    if (colorMode == ColorMode.Gradient)
                        gradedColor = gradientColor(cU, cV, cW, coords.U, coords.V, coords.W);
                    else
                        gradedColor = nearestColor(cU, cV, cW, coords.U, coords.V, coords.W);

                    lock (decompressed.Output)
                        decompressed.Output.SetPixel(point.X, point.Y, gradedColor);

                    lock (decompressed.DebugOutput)
                        decompressed.DebugOutput.SetPixel(point.X, point.Y, Color.FromArgb((byte)(coords.U * 255), (byte)(coords.V * 255), (byte)(coords.W * 255)));

                }
            });

            return decompressed;
        }

        private static Mesh buildMesh(Dictionary<Point, Color> pointIndex)
        {
            InputGeometry g = new InputGeometry();
            foreach (var value in pointIndex)
            {
                g.AddPoint(value.Key.X, value.Key.Y);
            }

            Mesh m = new Mesh();
            m.Triangulate(g);
            return m;
        }

        private static Color gradientColor(Color cU, Color cV, Color cW, double u, double v, double w)
        {
            byte R = (byte)(cU.R * u + cV.R * v + cW.R * w);
            byte G = (byte)(cU.G * u + cV.G * v + cW.G * w);
            byte B = (byte)(cU.B * u + cV.B * v + cW.B * w);

            return Color.FromArgb(R, G, B);
        }

        private static Color nearestColor(Color cU, Color cV, Color cW, double u, double v, double w)
        {
            if (u >= v && u >= w)
                return cU;
            if (v >= w)
                return cV;
            return cW;
        }
        /// <summary> Enumeration of options for how colors are chosen when using the TrigradDecompressor. </summary>
        public enum ColorMode
        {
            /// <summary> Nearest neighbour colouring. </summary>
            Nearest,
            /// <summary> Barycentric gradient colouring. </summary>
            Gradient
        }
    }
}
