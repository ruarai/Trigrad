using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trigrad.DataTypes
{
    public class DrawPoint
    {
        public DrawPoint(BarycentricCoordinates coords, Point p)
        {
            BarycentricCoordinates = coords;
            Point = p;
        }

        public BarycentricCoordinates BarycentricCoordinates;
        public Point Point;
    }
}
