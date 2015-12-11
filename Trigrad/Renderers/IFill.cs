using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelMapSharp;
using Trigrad.DataTypes;

namespace Trigrad.Renderers
{
    public interface IFill
    {
        void Fill(SampleTri t, PixelMap map);
    }
}
