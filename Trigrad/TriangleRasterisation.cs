using System;
using System.Collections.Generic;
using System.Drawing;
using Trigrad.DataTypes;

namespace Trigrad
{
    internal static class TriangleRasterization
    {
        public static IEnumerable<DrawPoint> PointsInTriangle(Point pt1, Point pt2, Point pt3)
        {
            int minX = pt1.X < pt2.X && pt1.X < pt3.X ? pt1.X : (pt2.X < pt3.X ? pt2.X : pt3.X);
            int minY = pt1.Y < pt2.Y && pt1.Y < pt3.Y ? pt1.Y : (pt2.Y < pt3.Y ? pt2.Y : pt3.Y);

            int maxX = pt1.X > pt2.X && pt1.X > pt3.X ? pt1.X : (pt2.X > pt3.X ? pt2.X : pt3.X);
            int maxY = pt1.Y >pt2.Y && pt1.Y > pt3.Y ? pt1.Y : (pt2.Y > pt3.Y ? pt2.Y : pt3.Y);

            for (int x = minX; x < maxX+1; x++)
            {
                for (int y = minY; y < maxY + 1; y++)
                {
                    Point p = new Point(x,y);

                    var coords = Barycentric.GetCoordinates(p, pt1, pt2, pt3);

                    if (Barycentric.ValidCoords(coords))
                    {
                        yield return new DrawPoint(coords,p);
                    }
                }
            }
        }
    }
}
