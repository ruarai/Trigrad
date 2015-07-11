using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;
using Point = System.Drawing.Point;

namespace Trigrad
{
    /// <summary> Holds methods for decompressing trigrad compressed imagery. </summary>
    public static class TrigradDecompressor
    {
        /// <summary> Decompresses a trigrad compressed bitmap. </summary>
        /// <param name="compressionData"> The TrigradCompressed data.</param>
        /// <param name="colorGrader"> The color grader that will be used to fill the output bitmap.</param>
        /// <param name="debug"> Bool specifying whether a debug output will be produced.</param>
        public static TrigradDecompressed DecompressBitmap(TrigradCompressed compressionData, IGrader colorGrader = null,bool debug = false)
        {
            if (colorGrader == null)
                colorGrader = new BarycentricGrader();

            TrigradDecompressed decompressed = new TrigradDecompressed(compressionData.Width, compressionData.Height);

            //build the triangle mesh
            decompressed.Mesh = buildMesh(compressionData.SampleTable);

            Parallel.ForEach(decompressed.Mesh.Triangles, triangle =>
            {
                var vU = triangle.GetVertex(0);
                var vV = triangle.GetVertex(1);
                var vW = triangle.GetVertex(2);

                //find sample points of triangle
                Color cU = compressionData.SampleTable[vU.Point()];
                Color cV = compressionData.SampleTable[vV.Point()];
                Color cW = compressionData.SampleTable[vW.Point()];

                //rasterize triangle to find points to fill
                var points = TriangleRasterization.PointsInTriangle(vU.Point(), vV.Point(), vW.Point());

                foreach (var point in points)
                {
                    var coords = Barycentric.GetCoordinates(point, vU, vV, vW);

                    Color gradedColor = colorGrader.Grade(cU, cV, cW, coords.U, coords.V, coords.W, point.X, point.Y,vU.Point(),vV.Point(),vW.Point());

                    lock (decompressed.Output)
                        decompressed.Output.SetPixel(point.X, point.Y, gradedColor);

                    if(debug)
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
    }
}
