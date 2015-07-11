using System.Drawing;

namespace Trigrad.ColorGraders
{
    /// <summary> Interface for producing color graders. </summary>
    public interface IGrader
    {
        /// <summary> Base method for color grading. </summary>
        Color Grade(Color cU, Color cV, Color cW, double u, double v, double w, int x, int y,Point pU,Point pV,Point pW);
    }
}
