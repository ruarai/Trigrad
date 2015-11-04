using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelMapSharp;
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
        private PixelMap inputBitmap;
        private FrequencyTable table;
        public Form1()
        {
            InitializeComponent();

            string input = "tests\\input\\Art.jpg";

            inputBitmap = PixelMap.SlowLoad(new Bitmap(input));
            table = new FrequencyTable(inputBitmap, 1, 0);

            buildMesh();
            render();
        }

        private void buildMesh()
        {
            options = new TrigradOptions { SampleCount = int.Parse(samplesTextBox.Text), FrequencyTable = table, Resamples = 30, Iterations = 1, Grader = new BarycentricGrader() };

            meshImage = TrigradCompressor.CompressBitmap(inputBitmap, options);
            meshImage.Mesh = MeshBuilder.BuildMesh(meshImage.SampleTable);
        }

        private void render()
        {
            GPUT.CalculateMesh(meshImage.Mesh);

            var returned = TrigradDecompressor.DecompressBitmap(meshImage, options);

            outputPictureBox.Image = returned.Output.GetBitmap();
        }

        private static double dist(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void outputPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);


            List<SampleTri> tris = new List<SampleTri>();

            Console.WriteLine("collecting tris");
            Parallel.ForEach(meshImage.Mesh, tri =>
            {
                double dist = Form1.dist(p, tri.U.Point);

                if (dist < 10d)
                {
                    lock (tris)
                        tris.Add(tri);
                }
            });

            Console.WriteLine("collected");
            Parallel.ForEach(tris, tri =>
            {
                foreach (var sample in tri.Samples)
                {
                    lock (sample)
                        sample.Color = new Pixel(Color.White);
                }
            });
            Console.WriteLine("colored");

            render();
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            buildMesh();
            render();
        }
    }
}
