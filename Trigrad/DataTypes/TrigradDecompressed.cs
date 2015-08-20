using System.Collections.Generic;
using System.Drawing;
using TriangleNet;

namespace Trigrad.DataTypes
{
    /// <summary> The results of a TrigradDecompression, containing both the output and debug bitmaps. </summary>
    public class TrigradDecompressed
    {
        /// <summary> Constructor for a TrigradDecompressed object, defining the width and height of output bitmaps. </summary>
        public TrigradDecompressed(int width, int height)
        {
            Output = new PixelMap(width, height);
            DebugOutput = new PixelMap(width, height);
        }
        /// <summary> The decompressed output bitmap. </summary>
        public PixelMap Output;
        /// <summary> The debug output bitmap, showing calculated barycentric coordinates. </summary>
        public PixelMap DebugOutput;

        internal List<SampleTri> Mesh;
    }
}
