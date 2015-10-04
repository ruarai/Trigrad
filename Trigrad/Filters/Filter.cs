using System.Collections.Generic;
using Trigrad.DataTypes;

namespace Trigrad.Filters
{
    interface ITriGradFilter
    {
        void Run(List<SampleTri> Mesh);
    }
}
