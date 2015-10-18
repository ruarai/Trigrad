using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigrad.ColorGraders;

namespace Trigrad.DataTypes
{
    /// <summary> Options for the usage of the TrigradCompressor. </summary>
    public class TrigradOptions
    {
        /// <summary> Constructs a TrigradOptions with a random seed. </summary>
        public TrigradOptions()
        {
            Random = new Random();
        }

        /// <summary> Constructs a TrigradOptions with a specified seed. </summary>
        public TrigradOptions(int seed)
        {
            Random = new Random(seed);
        }

        /// <summary> The goal number of samples to be achieved. </summary>
        public int SampleCount = 1000;

        /// <summary> The frequency table providing the TrigradCompressor values regarding the chance a pixel will be sampled. </summary>
        public FrequencyTable FrequencyTable = null;

        /// <summary> The random number generator to be used by the TrigradCompressor. </summary>
        public Random Random;

        /// <summary> The number of resamples. </summary>
        public int Resamples = 25;

        /// <summary> The number of iterations performed during minimisation. </summary>
        public int Iterations = 4;

        public IGrader Grader = new BarycentricGrader();
    }
}
