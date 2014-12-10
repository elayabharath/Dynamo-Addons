using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;


public class Spiral
{
    private Spiral()
    {
    }

    private static double goldenRatio()
    {
        return 1.61803398875;
    }
    
    private static double degToRad(double angle)
    {
        return Math.PI * angle / 180.0;
    }

    private static double eulerConstant()
    {
        return 2.7182818284;
    }


    /// <summary>
    ///     A golden spiral is a logarithmic spiral whose growth factor is φ, the golden ratio.
    /// </summary>
    /// <param name="angle">360 completes 1 circulation</param>
    /// <search>golden,fibonacci</search>
    public static NurbsCurve Golden(double angle=720)
    {
        if (angle == 0.0)
        { throw new ArgumentException("The angle and scale values can't be 0."); }

        angle = (angle < 0.0) ? -1 * angle : angle;
        
        int numPts = 1000;
        var pts = new List<Point>();

        double b = Math.Log(goldenRatio()) / (Math.PI / 2);
        double c = Math.Pow(Math.E, b);
        angle = degToRad(angle);


        for (int i = 0; i < numPts; ++i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = Math.Pow(c, currAngle);

            double xVal = Math.Cos(currAngle) * Math.Pow(c, currAngle);
            double yVal = Math.Sin(currAngle) * Math.Pow(c, currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByControlPoints(pts);
    }

    /// <summary>
    ///     Fermat's spiral (also known as a parabolic spiral) follows the equation r = +/- Theta^0.5
    /// </summary>
    /// <param name="angle">360 completes 1 circulation</param>
    /// <search>golden,fibonacci</search>
    public static PolyCurve Fermat(double angle=3600)
    {
        if (angle == 0.0)
        { throw new ArgumentException("The angle value can't be 0.", "angle"); }

        if (angle < 0.0)
        {
            angle = angle * -1;
        }

        int numPts = 1000;
        angle = degToRad(angle);

        var pts = new List<Point>();
        
        for (int i = numPts-1; i >=0; --i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = Math.Sqrt(currAngle);

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);
            pts.Add(Point.ByCoordinates(-xVal, -yVal, 0));
        }

        for (int i = 1; i < numPts; ++i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = Math.Sqrt(currAngle);

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);
            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return PolyCurve.ByPoints(pts);
    }

    /// <summary>
    ///     The Archimedean spiral is formed by locus of points corresponding to the locations over time of a point moving away from a 
    ///     fixed point with a constant speed along a line which rotates with constant angular velocity.
    /// </summary>
    /// <param name="angle">360 completes 1 circulation</param>
    /// <param name="innerRadius">Changing this will change the starting place of the spiral</param>
    /// <param name="turnDistance">This controls the distance between successive turnings.</param>
    /// <search>archimedes,arithmetic</search>
    public static NurbsCurve Archimedean(double angle = 3600, double innerRadius = 0.2, double turnDistance = 0.2)
    {
        if (angle == 0.0)
        { throw new ArgumentException("The angle value can't be 0.", "angle"); }

        if (angle < 0.0)
        {
            angle = angle * -1;
        }

        int numPts = 1000;
        angle = degToRad(angle);

        var pts = new List<Point>();

        for (int i = 0; i < numPts; ++i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = innerRadius + turnDistance * currAngle;

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByPoints(pts);
    }

    /// <summary>
    ///     A hyperbolic spiral is a transcendental plane curve also known as a reciprocal spiral, this the opposite of an Archimedean spiral and is a type of Cotes' spiral.
    /// </summary>
    /// <param name="angle">360 completes 1 circulation</param>
    /// <param name="innerScale">Changing this will expand the inner circular part of the spiral</param>
    /// <search>Hyperbolic,cotes,archimedes</search>
    public static PolyCurve Hyperbolic(double angle = 3600, double innerScale = 100)
    {
        if (angle == 0.0)
        { throw new ArgumentException("The angle value can't be 0.", "angle"); }

        if (angle < 0.0)
        {
            angle = angle * -1;
        }

        int numPts = 1000;
        angle = degToRad(angle);

        var pts = new List<Point>();

        for (int i = 30; i < numPts; ++i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = innerScale / currAngle;

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return PolyCurve.ByPoints(pts);
    }

    /// <summary>
    ///     The logarithmic spiral can be distinguished from the Archimedean spiral by the fact that 
    ///     the distances between the turnings of a logarithmic spiral increase in geometric progression,
    ///      while in an Archimedean spiral these distances are constant.
    /// </summary>
    /// <param name="angle">360 completes 1 circulation</param>
    /// <param name="scale">Determines the scale of the spiral</param>
    /// <param name="growth">This determines the factor of increase along the spiral. WARNING: Values beyond 0.3 may not give any meaningful visualization</param>
    /// <search>log,equiangular,growth,marvelous</search>
    public static NurbsCurve Logarithmic(double angle = 1800, double scale = 0.5, double growth = 0.1)
    {
        if (angle == 0.0)
        { throw new ArgumentException("The angle value can't be 0.", "angle"); }

        if (angle < 0.0)
        {
            angle = angle * -1;
        }

        int numPts = 1000;
        angle = degToRad(angle);

        var pts = new List<Point>();

        for (int i = 0; i < numPts; ++i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = scale * Math.Pow(eulerConstant(), (growth * currAngle));

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByPoints(pts);
    }


    /// <summary>
    ///     A lituus is a spiral in which the angle is inversely proportional to the square of the radius.
    /// </summary>
    /// <param name="angle">360 completes 1 circulation</param>
    /// <param name="scale">Determines the scale of the spiral</param>
    /// <search>lituus,cotes</search>
    public static NurbsCurve Lituus(double angle = 3600, double scale = 5)
    {
        
        if (angle == 0.0)
        { throw new ArgumentException("The angle value can't be 0.", "angle"); }

        if (scale == 0.0)
        { throw new ArgumentException("The scale value can't be 0.", "scale"); }

        if (angle < 0.0)
        {
            angle = angle * -1;
        }

        int numPts = 1000;
        angle = degToRad(angle);

        var pts = new List<Point>();

        for (int i = 1; i <= numPts; ++i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = Math.Sqrt(1/currAngle);

            double xVal = scale * radius * Math.Cos(currAngle);
            double yVal = scale * radius * Math.Sin(currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByPoints(pts);
    }


    /// <summary>
    ///     The spiral of Theodorus is a spiral composed of contiguous right triangles
    /// </summary>
    /// <param name="numberRot">Determines the number of turns in the spiral</param>
    /// <search>square root,einstein,pythagorean</search>
    public static NurbsCurve Theodorus(int numberRot=100)
    {
        if (numberRot == 0)
        { throw new ArgumentException("The angle value can't be 0.", "angle"); }

        if (numberRot < 0)
        {
            numberRot = numberRot * -1;
        }

        var pts = new List<Point>();
        double radius = 1;
        double currAngle = 0;

        for (int i = 1; i <= numberRot; ++i)
        {
            radius = Math.Sqrt(i);
            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            
            pts.Add(Point.ByCoordinates(xVal, yVal, 0));

            currAngle = currAngle + Math.Atan(1 / Math.Sqrt(i));
        }

        return NurbsCurve.ByControlPoints(pts, 1, false);
    }

    /// <summary>
    ///     Breaks the continous spiral into a series of straight lines
    /// </summary>
    /// <param name="spiral">The spiral to break</param>
    /// <param name="numBreaks">The number of lines to generate</param>
    /// <search>break,divide</search>
    public static NurbsCurve getBrokenSpiral(Curve spiral, int numBreaks=100)
    {
        if (numBreaks == 0)
        { throw new ArgumentException("The numPts needs to be non-zero positive integer"); }

        numBreaks = (int)Math.Abs(numBreaks);
        var pts = new List<Point>();

        for (int i = 0; i <= numBreaks; ++i)
        {
            pts.Add(spiral.PointAtParameter((double)i / (double)numBreaks));
        }

        return NurbsCurve.ByControlPoints(pts, 1, false);
    }

    /// <summary>
    ///     Gets the points at equal distance on the spiral
    /// </summary>
    /// <param name="spiral">The spiral to plot the points on</param>
    /// <param name="numPts">The number of points needed on the curve</param>
    /// <search>divide</search>
    public static List<Point> getPointsAtEqualDistance(Curve spiral, int numPts=100)
    {
        if (numPts == 0)
        { throw new ArgumentException("The numPts needs to be non-zero positive integer"); }

        numPts = (int)Math.Abs(numPts);
        var pts = new List<Point>();
        double distance = spiral.Length / numPts;

        for (int i = 0; i <= numPts; ++i)
        {
            pts.Add(spiral.PointAtDistance(i * distance));
        }

        return pts;
    }

}
