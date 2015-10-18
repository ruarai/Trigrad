using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cloo;
using Trigrad.DataTypes;
using Parallel = System.Threading.Tasks.Parallel;
using System.Drawing;

namespace Trigrad
{
    public class GPUT
    {
        static GPUT()
        {
            program.Build(new[] { context.Devices[0] }, null, null, IntPtr.Zero);
        }

        public static void CalculateMesh(List<SampleTri> mesh)
        {
            List<Calculation> calculations = new List<Calculation>();
            foreach (var sampleTri in mesh)
            {
                sampleTri.Points.Clear();
                calculations.AddRange(TriangleRasterization.BuildTriCalculations(sampleTri));
            }

            Calculate(calculations);
        }

        private static ComputePlatform platform = ComputePlatform.Platforms[0];
        private static ComputeContextPropertyList properties = new ComputeContextPropertyList(platform);
        private static ComputeContext context = new ComputeContext(platform.Devices, properties, null, IntPtr.Zero);

        private static ComputeProgram program = new ComputeProgram(context, System.IO.File.ReadAllText("GPUT.c"));

        public static void Calculate(List<Calculation> calculations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            int count = calculations.Count;

            IntVec2[] p_p = new IntVec2[count];

            IntVec2[] p_a = new IntVec2[count];
            IntVec2[] p_b = new IntVec2[count];
            IntVec2[] p_c = new IntVec2[count];

            FloatVec3[] c = new FloatVec3[count];

            int[] c_valid = new int[count];

            Parallel.For(0, count, i =>
            {
                var calc = calculations[i];

                p_p[i] = new IntVec2(calc.P);
                p_a[i] = new IntVec2(calc.A);
                p_b[i] = new IntVec2(calc.B);
                p_c[i] = new IntVec2(calc.C);
            });

            mark(s, "memory init");

            ComputeBuffer<IntVec2> _p_p = new ComputeBuffer<IntVec2>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_p);

            ComputeBuffer<IntVec2> _p_a = new ComputeBuffer<IntVec2>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_a);

            ComputeBuffer<IntVec2> _p_b = new ComputeBuffer<IntVec2>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_b);

            ComputeBuffer<IntVec2> _p_c = new ComputeBuffer<IntVec2>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, p_c);

            ComputeBuffer<FloatVec3> _c = new ComputeBuffer<FloatVec3>(context, ComputeMemoryFlags.WriteOnly, c.Length);
            ComputeBuffer<int> _c_valid = new ComputeBuffer<int>(context, ComputeMemoryFlags.WriteOnly, c_valid.Length);

            mark(s, "memory buffer init");

            ComputeKernel kernel = program.CreateKernel("Barycentric");
            kernel.SetMemoryArgument(0, _p_p);

            kernel.SetMemoryArgument(1, _p_a);

            kernel.SetMemoryArgument(2, _p_b);

            kernel.SetMemoryArgument(3, _p_c);

            kernel.SetMemoryArgument(4, _c);
            kernel.SetMemoryArgument(5, _c_valid);

            mark(s, "memory init 2");

            ComputeEventList eventList = new ComputeEventList();

            ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

            commands.Execute(kernel, null, new long[] { count }, null, eventList);

            mark(s, "execute");

            commands.ReadFromBuffer(_c, ref c, false, eventList);
            commands.ReadFromBuffer(_c_valid, ref c_valid, false, eventList);
            commands.Finish();

            mark(s, "read 1");

            Parallel.For(0, count, i =>
            {
                var calc = calculations[i];
                calc.Coords = new BarycentricCoordinates(c[i].U,c[i].V,c[i].W);

                if (c_valid[i] == 1)
                {
                    lock (calc.Tri)
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

            _p_p.Dispose();

            _p_a.Dispose();
            _p_b.Dispose();
            _p_c.Dispose();

            _c.Dispose();
            _c_valid.Dispose();

            mark(s, "dispose");
        }

        private static void mark(Stopwatch s, string str)
        {
            Console.WriteLine(str + " completed in " + s.ElapsedMilliseconds);
            s.Restart();
        }

        private struct IntVec2
        {
            public IntVec2(Point p)
            {
                X = p.X;
                Y = p.Y;
            }

            public int X;
            public int Y;
        }

        private struct FloatVec3
        {
            public float U;
            public float V;
            public float W;
            public float nil;//dummy value for memory alignment
        }
    }
}
