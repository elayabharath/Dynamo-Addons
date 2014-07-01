using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;


public class spirals
{

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

    public static NurbsCurve fibonnaciSpiral(double angle, double scale = 1)
    {
        if (angle == 0.0 && scale == 0.0)
        { throw new ArgumentException("The angle and scale values can't be 0."); }
        else if (angle == 0.0)
        { throw new ArgumentException("The angle value can't be 0.", "angle"); }
        else if (scale == 0.0)
        { throw new ArgumentException("The scale value can't be 0.", "scale"); }

        int numPts = 1000;
        var pts = new List<Point>();

        double b = Math.Log(goldenRatio()) / (Math.PI / 2);
        double c = Math.Pow(Math.E, b);
        angle = degToRad(angle);


        for (int i = 0; i < numPts; ++i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = scale * Math.Pow(c, currAngle);

            double xVal = scale * Math.Cos(currAngle) * Math.Pow(c, currAngle);
            double yVal = scale * Math.Sin(currAngle) * Math.Pow(c, currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByControlPoints(pts);
    }
    public static PolyCurve fermatSpiral(double angle)
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
    public static NurbsCurve archimedeanSpiral(double angle, double a, double b)
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
            double radius = a + b * currAngle;

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByPoints(pts);
    }
    public static NurbsCurve hyperbolicSprial(double angle, double a)
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
            double radius = a / currAngle;

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByPoints(pts);
    }
    public static NurbsCurve logSpiral(double angle, double a, double b)
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
            double radius = a * Math.Pow(eulerConstant(), (b*currAngle));

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByPoints(pts);
    }
    public static NurbsCurve lituusSpiral(double angle)
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

        for (int i = 1; i <= numPts; ++i)
        {
            double currAngle = (double)i / 1000.0 * angle;
            double radius = Math.Sqrt(1/currAngle);

            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            pts.Add(Point.ByCoordinates(xVal, yVal, 0));
        }

        return NurbsCurve.ByPoints(pts);
    }
    public static NurbsCurve theodorusSpiral(int n)
    {
        if (n == 0)
        { throw new ArgumentException("The angle value can't be 0.", "angle"); }

        if (n < 0)
        {
            n = n * -1;
        }

        var pts = new List<Point>();
        double radius = 1;
        double currAngle = 0;

        for (int i = 1; i <= n; ++i)
        {
            radius = Math.Sqrt(i);
            double xVal = radius * Math.Cos(currAngle);
            double yVal = radius * Math.Sin(currAngle);

            
            pts.Add(Point.ByCoordinates(xVal, yVal, 0));

            currAngle = currAngle + Math.Atan(1 / Math.Sqrt(i));
            //radius = radius + Math.Sqrt(n+1) - Math.Sqrt(n);
        }

        return NurbsCurve.ByControlPoints(pts, 1, false);
    }
    public static NurbsCurve breakSpiral(Curve spiral, int numBreaks)
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
    public static List<Point> getPointsAtEqualDistance(Curve spiral, int numPts)
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
