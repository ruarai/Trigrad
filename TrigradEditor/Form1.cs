using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trigrad;
using Trigrad.ColorGraders;
using Trigrad.DataTypes;
using Trigrad.DataTypes.Compression;

namespace TrigradEditor
{
    public partial class Form1 : Form
    {
        private TrigradCompressed meshImage;
        private TrigradOptions options;
        public Form1()
        {
            InitializeComponent();

            string input = "tests\\input\\Art.jpg";

            PixelMap inputBitmap = PixelMap.SlowLoad(new Bitmap(input));
            FrequencyTable table = new FrequencyTable(inputBitmap, 1, 0);

            options = new TrigradOptions { SampleCount = 10000, FrequencyTable = table, ScaleFactor = 1, Resamples = 30, Iterations = 1, Grader = new BarycentricGrader() };

            meshImage = TrigradCompressor.CompressBitmap(inputBitmap, options);
            meshImage.Mesh = MeshBuilder.BuildMesh(meshImage.SampleTable);

            render();
        }

        private void render()
        {
            GPUT.CalculateMesh(meshImage.Mesh);

            var returned = TrigradDecompressor.DecompressBitmap(meshImage, options);

            outputPictureBox.Image = returned.Output.Bitmap;
        }

        private Point mouseDownPoint = Point.Empty;

        private void outputPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPoint = e.Location;
        }

        private static double dist(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;

            return Math.Sqrt(dx*dx + dy*dy);
        }

        private void outputPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);


            List<SampleTri> tris = new List<SampleTri>();


            foreach (var tri in meshImage.Mesh)
            {
                double dist = Form1.dist(p, tri.CenterPoint);

                if (dist < 10d)
                {
                    tris.Add(tri);
                }
            }

            foreach (var tri in tris)
            {
                foreach (var sample in tri.Samples)
                {
                    sample.Color = Color.White;
                }
            }

            render();
        }
    }
}
