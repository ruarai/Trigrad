using System;
using PixelMapSharp;

namespace Trigrad.DataTypes
{
    /// <summary> A frequency table defining how likely a sample will be formed during a TrigradCompression. </summary>
    public class FrequencyTable
    {
        /// <summary> The underlying 2D frequency table, with values lying from 0 to 1. </summary>
        public double[,] Table;

        /// <summary> Constructs a frequency table using sobel edge detection. </summary>
        public FrequencyTable(PixelMap pixelmap)
        {
            Table = sobelFilter(pixelmap);

            for (int x = 0; x < Table.GetLength(0); x++)
            {
                for (int y = 0; y < Table.GetLength(1); y++)
                {
                    Table[x, y] = Math.Pow(Table[x,y],1.7);
                }
            }
        }

        /// <summary> The sum of the FrequencyTable. </summary>
        public double Sum
        {
            get
            {
                double chanceSum = 0;
                for (int x = 0; x < Table.GetLength(0); x++)
                {
                    for (int y = 0; y < Table.GetLength(1); y++)
                    {
                        chanceSum += Table[x, y];
                    }
                }
                return chanceSum;
            }
        }

        /// <summary> Constructs a frequency table using a specified table of values. </summary>
        public FrequencyTable(double[,] table)
        {
            Table = table;
        }

        private double[,] sobelFilter(PixelMap map)
        {
            double[,] output = new double[map.Width, map.Height];

            double[,] kernelX = 
            {
                {-1,0,1 },
                {-2,0,2 },
                {-1,0,1 }
            };

            double[,] kernelY = 
            {
                {-1,-2,-1},
                {0,0,0},
                {1,2,1}
            };


            double[,] xFiltered = sobelPass(map, kernelX);
            double[,] yFiltered = sobelPass(map, kernelY);

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    double xVal = xFiltered[x, y];
                    double yVal = yFiltered[x, y];

                    output[x,y] = Math.Sqrt(xVal*xVal + yVal*yVal);
                }
            }

            return output;
        }

        private double[,] sobelPass(PixelMap map, double[,] kernel)
        {
            double[,] output =new double[map.Width,map.Height];

            for (int x = 1; x < map.Width-1; x++)
            {
                for (int y = 0; y < map.Height-1; y++)
                {
                    double sum = 0;
                    for (int u = 0; u < 3; u++)
                    {
                        for (int v = 0; v < 3; v++)
                        {
                            double kVal = kernel[u, v];
                            double imgVal = map[x-1 + u,y-1 + v].Lightness;

                            sum += kVal*imgVal;
                        }
                    }
                    output[x, y] = sum;
                }
            }
            return output;
        }
    }
}
