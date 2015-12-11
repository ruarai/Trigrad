using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PixelMapSharp;
using Trigrad.DataTypes;

namespace Trigrad.Renderers
{
    public class ShapeFill : IFill
    {
        public ShapeFill(int sides)
        {
            ShapeFunction = t => Math.Cos(Math.PI/sides)/Math.Cos(t%((2*Math.PI)/sides) - Math.PI/sides);
        }

        public ShapeFill(Func<double, double> function)
        {
            ShapeFunction = function;
        }

        public Func<double,double> ShapeFunction = t => 1;

        public void Fill(SampleTri t, PixelMap map)
        {
            Point center = t.CenterPoint;

            int radius = (int)t.Samples.Max(s => dist(s.Point, center));

            for (int x = center.X - radius; x < center.X + radius; x++)
            {
                for (int y = center.Y - radius; y < center.Y + radius; y++)
                {
                    Point fillPoint = new Point(x, y);
                    double theta = Math.PI+Math.Atan2(fillPoint.Y - center.Y,fillPoint.X - center.X);
                    double distance = dist(fillPoint, center);

                    if (map.Inside(fillPoint) && distance < ShapeFunction(theta)*radius)
                    {
                        map[fillPoint] = t.CenterColor;

                        //map[fillPoint] = new Pixel(theta*180/Math.PI,0.5,0.5);
                    }

                    if (distance > 1000)
                    {
                        Console.WriteLine("wat");
                    }
                }
            }

        }

        private double dist(Point a, Point b)
        {
            int dX = a.X - b.X;
            int dY = a.Y - b.Y;

            return Math.Sqrt(dX * dX + dY * dY);
        }


    }
}
