using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AForge;
using Cloo;
using Trigrad.DataTypes;
using Parallel = System.Threading.Tasks.Parallel;

namespace Trigrad
{
    public class GPUT
    {
        public static void CalculateMesh(List<SampleTri> mesh)
        {
            List<Calculation> calculations = new List<Calculation>();
            foreach (var sampleTri in mesh)
            {
                sampleTri.Points.Clear();
                calculations.AddRange(TriangleRasterization.PointsInTriangle(sampleTri));
            }

            Calculate(calculations);
        }

        private static int[] p_p_x;
        private static int[] p_p_y;

        private static int[] p_a_x;
        private static int[] p_a_y;

        private static int[] p_b_x;
        private static int[] p_b_y;

        private static int[] p_c_x;
        private static int[] p_c_y;


        private static float[] c_u;
        private static float[] c_v;
        private static float[] c_w;
        private static int[] c_valid;

        private static ComputePlatform platform = ComputePlatform.Platforms[0];
        private static ComputeContextPropertyList properties = new ComputeContextPropertyList(platform);
        private static ComputeContext context = new ComputeContext(platform.Devices, properties, null, IntPtr.Zero);
        public static void Calculate(List<Calculation> calculations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            int count = calculations.Count;

            p_p_x = new int[count];
            p_p_y = new int[count];

            p_a_x = new int[count];
            p_a_y = new int[count];

            p_b_x = new int[count];
            p_b_y = new int[count];

            p_c_x = new int[count];
            p_c_y = new int[count];

            c_u = new float[count];
            c_v = new float[count];
            c_w = new float[count];
            c_valid = new int[count];

            Parallel.For(0, count, i =>
            {
                var calc = calculations[i];

                p_p_x[i] = calc.P.X;
                p_p_y[i] = calc.P.Y;

                p_a_x[i] = calc.A.X;
                p_a_y[i] = calc.A.Y;

                p_b_x[i] = calc.B.X;
                p_b_y[i] = calc.B.Y;

                p_c_x[i] = calc.C.X;
                p_c_y[i] = calc.C.Y;
            });

            mark(s, "memory init");

            ComputeBuffer<int> _p_p_x = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_p_x);
            ComputeBuffer<int> _p_p_y = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_p_y);

            ComputeBuffer<int> _p_a_x = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_a_x);
            ComputeBuffer<int> _p_a_y = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_a_y);

            ComputeBuffer<int> _p_b_x = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_b_x);
            ComputeBuffer<int> _p_b_y = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_b_y);

            ComputeBuffer<int> _p_c_x = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_c_x);
            ComputeBuffer<int> _p_c_y = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_c_y);

            ComputeBuffer<float> _c_u = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly, c_u.Length);
            ComputeBuffer<float> _c_v = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly, c_v.Length);
            ComputeBuffer<float> _c_w = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly, c_w.Length);
            ComputeBuffer<int> _c_valid = new ComputeBuffer<int>(context, ComputeMemoryFlags.WriteOnly, c_valid.Length);

            mark(s, "memory buffer init");

            var program = new ComputeProgram(context, System.IO.File.ReadAllText("GPUT.c"));
            program.Build(new[]{context.Devices[0]}, null, null, IntPtr.Zero);

            mark(s, "program build");

            ComputeKernel kernel = program.CreateKernel("Barycentric");
            kernel.SetMemoryArgument(0, _p_p_x);
            kernel.SetMemoryArgument(1, _p_p_y);

            kernel.SetMemoryArgument(2, _p_a_x);
            kernel.SetMemoryArgument(3, _p_a_y);

            kernel.SetMemoryArgument(4, _p_b_x);
            kernel.SetMemoryArgument(5, _p_b_y);

            kernel.SetMemoryArgument(6, _p_c_x);
            kernel.SetMemoryArgument(7, _p_c_y);

            kernel.SetMemoryArgument(8, _c_u);
            kernel.SetMemoryArgument(9, _c_v);
            kernel.SetMemoryArgument(10, _c_w);
            kernel.SetMemoryArgument(11, _c_valid);

            mark(s, "memory init 2");

            ComputeEventList eventList = new ComputeEventList();

            ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

            commands.Execute(kernel, null, new long[] { count }, null, eventList);

            mark(s, "execute");

            commands.ReadFromBuffer(_c_u, ref c_u, false, eventList);
            commands.ReadFromBuffer(_c_v, ref c_v, false, eventList);
            commands.ReadFromBuffer(_c_w, ref c_w, false, eventList);
            commands.ReadFromBuffer(_c_valid, ref c_valid, false, eventList);
            commands.Finish();

            mark(s, "read 1");

            Parallel.For(0,count, i =>
            {
                var calc = calculations[i];
                calc.Coords = new BarycentricCoordinates(c_u[i], c_v[i], c_w[i]);

                if (c_valid[i] == 1)
                {
                    lock(calc.Tri)
                    calc.Tri.Points.Add(new DrawPoint(calc.Coords, calc.P));
                }
            });

            mark(s, "read 2");


            // cleanup commands
            commands.Dispose();

            // cleanup events
            foreach (ComputeEventBase eventBase in eventList)
            {
                eventBase.Dispose();
            }
            eventList.Clear();

            // cleanup kernel
            kernel.Dispose();

            _p_p_x.Dispose();
            _p_p_y.Dispose();

            _p_a_x.Dispose();
            _p_a_y.Dispose();

            _p_b_x.Dispose();
            _p_b_y.Dispose();

            _p_c_x.Dispose();
            _p_c_y.Dispose();

            _c_u.Dispose();
            _c_v.Dispose();
            _c_w.Dispose();
            _c_valid.Dispose();

            mark(s, "dispose");
        }

        private static void mark(Stopwatch s, string str)
        {
            Console.WriteLine(str + " completed in " + s.ElapsedMilliseconds);
            s.Restart();
        }
    }
}
