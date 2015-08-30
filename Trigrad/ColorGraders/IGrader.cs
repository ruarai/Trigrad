using System.Drawing;
using Trigrad.DataTypes;

namespace Trigrad.ColorGraders
{
    /// <summary> Interface for producing color graders. </summary>
    public interface IGrader
    {
        /// <summary> Base method for color grading. </summary>
        Color Grade(Color cU, Color cV, Color cW, BarycentricCoordinates coords, Point p, Point pU, Point pV, Point pW);
    }
}
