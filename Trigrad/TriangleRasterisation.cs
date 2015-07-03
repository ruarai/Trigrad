using System;
using System.Collections.Generic;
using System.Drawing;

namespace Trigrad
{
    internal static class TriangleRasterization
    {
        public static IEnumerable<Point> PointsInTriangle(Point pt1, Point pt2, Point pt3)
        {
            if (pt1.Y == pt2.Y && pt1.Y == pt3.Y)
            {
                throw new ArgumentException("The given points must form a triangle.");
            }

            Point tmp;

            if (pt2.X < pt1.X)
            {
                tmp = pt1;
                pt1 = pt2;
                pt2 = tmp;
            }

            if (pt3.X < pt2.X)
            {
                tmp = pt2;
                pt2 = pt3;
                pt3 = tmp;

                if (pt2.X < pt1.X)
                {
                    tmp = pt1;
                    pt1 = pt2;
                    pt2 = tmp;
                }
            }

            var baseFunc = CreateFunc(pt1, pt3);
            var line1Func = pt1.X == pt2.X ? (x => pt2.Y) : CreateFunc(pt1, pt2);

            for (var x = pt1.X; x < pt2.X; x++)
            {
                int maxY;
                int minY = GetRange(line1Func(x), baseFunc(x), out maxY);

                for (var y = minY; y <= maxY; y++)
                {
                    yield return new Point(x, y);
                }
            }

            var line2Func = pt2.X == pt3.X ? (x => pt2.Y) : CreateFunc(pt2, pt3);

            for (var x = pt2.X; x <= pt3.X; x++)
            {
                int maxY;
                int minY = GetRange(line2Func(x), baseFunc(x), out maxY);

                for (var y = minY; y <= maxY; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        private static int GetRange(double y1, double y2, out int maxY)
        {
            if (y1 < y2)
            {
                maxY = (int)Math.Floor(y2);
                return (int)Math.Ceiling(y1);
            }

            maxY = (int)Math.Floor(y1);
            return (int)Math.Ceiling(y2);
        }

        private static Func<int, double> CreateFunc(Point pt1, Point pt2)
        {
            var y0 = pt1.Y;

            if (y0 == pt2.Y)
            {
                return x => y0;
            }

            var m = (double)(pt2.Y - y0) / (pt2.X - pt1.X);

            return x => m * (x - pt1.X) + y0;
        }
    }
}
