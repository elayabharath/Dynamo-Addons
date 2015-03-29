using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Autodesk.DesignScript.Geometry;
using System.Collections;
using Autodesk.DesignScript.Runtime;

namespace svgPort
{
    public class SVG
    {
        private static System.IO.StreamWriter CreateNewSVGFile(String filePath, String fileName)
        {
            //check for invalid characters in the file name
            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidFileChars) != -1 || fileName.CompareTo("CON") == 0)
                throw new ArgumentException("The file name does not satisfy valid windows file name criteria", "fileName");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + fileName + ".svg");
            return file;
        }

        private static void preSVGBody(System.IO.StreamWriter file)
        {
            String line1 = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>";
            String line2 = "<!-- Generator: Dynamo SVG Export Addon. visit www.dynamobim.org  -->";
            String line3 = "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";

            file.WriteLine(line1);
            file.WriteLine(line2);
            file.WriteLine(line3);
        }

        public static void exportPathsAsSVG([ArbitraryDimensionArrayImport] IList geometryList, String exportLocation, string fileName)
        {
            //IList<Geometry> geometry = geometryList as IList<Geometry>;

            var type = geometryList.GetType();
            
            Geometry[] geometry;

            try
            {
                geometry = geometryList.Cast<List<Geometry>>().SelectMany(i => i).ToArray();
            }
            catch
            {
                geometry = geometryList.Cast<Geometry>().ToArray();
            }

            var boundingBox = BoundingBox.ByGeometry(geometry);
            var maxPt = boundingBox.MaxPoint;
            var minPt = boundingBox.MinPoint;

            //TODO: Handle replication for geometry

            var file = CreateNewSVGFile(exportLocation, fileName);
            
            //fill the SVG headers
            preSVGBody(file);
            
            //start the svg tag
            file.WriteLine("<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='"+(maxPt.X-minPt.X)+"px' height='"+(maxPt.Y-minPt.Y)+"px' viewBox='0 0 "+(maxPt.X-minPt.X)+" "+(maxPt.Y-minPt.Y)+"' xml:space='preserve'> ");
            

            //segregate all points
            List<Point> pts = new List<Point>();
            List<Line> lines = new List<Line>();
            List<Ellipse> ellipses = new List<Ellipse>();
            List<Circle> circles = new List<Circle>();
            List<Polygon> polygons = new List<Polygon>();
            List<Curve> polylines = new List<Curve>();
            List<NurbsCurve> nurbsCurves = new List<NurbsCurve>();
            //TODO: Need to support paths


            for (int i = 0; i < geometry.Length; ++i)
            {
                geometry[i] = geometry[i].Translate(0 - minPt.X, 0 - minPt.Y, 0);
                
                var geomType = geometry[i].GetType();
                if (geomType == typeof(Point))
                    pts.Add((Point)geometry[i]);

                else if (geomType == typeof(Line))
                    lines.Add((Line)geometry[i]);

                else if (geomType == typeof(Ellipse))
                    ellipses.Add((Ellipse)geometry[i]);

                else if (geomType == typeof(Circle))
                    circles.Add((Circle)geometry[i]);

                else if (geomType == typeof(Polygon))
                    polygons.Add((Polygon)geometry[i]);

                else if (geomType == typeof(NurbsCurve))
                    nurbsCurves.Add((NurbsCurve)geometry[i]);

            }

            //TODO: currently Z values are ignored, need a better way to do this
            //TODO: segregate points by layers

            //write all points into the file
            for (int i = 0; i < pts.Count; ++i)
            {
                if (i == 0)
                    file.WriteLine("<g>");

                double x = pts[i].X;
                double y = pts[i].Y;

                file.WriteLine("<circle cx='"+x.ToString()+"' cy='"+y.ToString()+"' r='1' fill='black'/>");

                if (i == pts.Count - 1)
                    file.WriteLine("</g>");
            }


            //write all lines into the file
            for (int i = 0; i < lines.Count; ++i)
            {
                if (i == 0)
                    file.WriteLine("<g>");

                Point startPt = lines[i].StartPoint;
                Point endPt = lines[i].EndPoint;

                double x1 = startPt.X;
                double y1 = startPt.Y;

                double x2 = endPt.X;
                double y2 = endPt.Y;

                file.WriteLine("<line x1='" + x1.ToString() + "' y1='" + y1.ToString() + "' x2='"+ x2.ToString() +"' y2='"+y2.ToString()+"' style='stroke:black; stroke-width: 1;'/>");

                if (i == lines.Count - 1)
                    file.WriteLine("</g>");
            }


            //write all ellipses into the file
            for (int i = 0; i < ellipses.Count; ++i)
            {
                if (i == 0)
                    file.WriteLine("<g>");

                Point centerPt = ellipses[i].CenterPoint;
                var majorAxis = ellipses[i].MajorAxis;
                var minorAxis = ellipses[i].MinorAxis;

                file.WriteLine("<ellipse cx='" + centerPt.X + "' cy='" + centerPt.Y + "' rx='" + majorAxis.Length + "' ry='" + minorAxis.Length + "' transform='rotate("+Math.Atan(majorAxis.Y/majorAxis.X)*180/Math.PI+", "+centerPt.X+", "+centerPt.Y+")' style='stroke:black; stroke-width: 1;'/>");

                if (i == ellipses.Count - 1)
                    file.WriteLine("</g>");
            }


            //write all polygons into the file
            for (int i = 0; i < polygons.Count; ++i)
            {
                if (i == 0)
                    file.WriteLine("<g>");

                var vertices = new List<Point>();
                vertices.AddRange(polygons[i].Points);

                String collectedString = "";
                foreach(var vertex in vertices)
                {
                    collectedString = collectedString + vertex.X + ", " + vertex.Y + " ";
                }

                file.WriteLine("<polygon points='"+collectedString+"' style='fill: none; stroke-width: 1; stroke: #000000;'/>");

                if (i == polygons.Count - 1)
                    file.WriteLine("</g>");
            }

            foreach(var nurbCurve in nurbsCurves)
            {
                var P = nurbCurve.ControlPoints();
                int p = nurbCurve.Degree;
                var U = nurbCurve.Knots();
                int m = P.Length + p;
                Debug.Assert(U.Length == m+1);

                // Assuming this is a clamped NURBS curve with knot vector
                // {U0, ..., Up, Up+1, ..., Um-p-1, Um-p ... Um}
                // we insert each of the internal knots {Up+1, ..., Um-p-1}
                // p-1 times. 
                int n = P.Length - 1;
                int nb;
                Point[][] Q;
                DecomposeCurve(n, p, U, P, out nb, out Q);

                // Write j'th point Q[i][j] for every i'th Bezier curve
            }

            //complete the svg tag
            file.WriteLine("</svg>");
            file.Close();
        }

        public static Point[][] DecomposeNurbsCurve(NurbsCurve nurbCurve)
        {
            var P = nurbCurve.ControlPoints();
            int p = nurbCurve.Degree;
            var U = nurbCurve.Knots();
            int m = P.Length + p;
            Debug.Assert(U.Length == m + 1);

            // Assuming this is a clamped NURBS curve with knot vector
            // {U0, ..., Up, Up+1, ..., Um-p-1, Um-p ... Um}
            // we insert each of the internal knots {Up+1, ..., Um-p-1}
            // p-1 times. 
            int n = P.Length - 1;
            int nb;
            Point[][] Q;
            DecomposeCurve(n, p, U, P, out nb, out Q);
            return Q;
        }

        struct point
        {
            public double X;
            public double Y;
            public double Z;
        }

        private static void DecomposeCurve(int n, int p, double[] U,
            Point[] P, out int nb, out Point[][] Q)
        {
            int m = n + p + 1;
            int a = p;
            int b = p + 1;
            nb = 0;
            // If there are m+1 knots and a clamped knot vector
            // The number of knot intervals would ideally be (m-2p)
            // which is also the max number of Bezier curves that can be created
            point[,] qq = new point[m-2*p, p+1];
            for(int i = 0; i <= p; i++)
            {
                qq[nb, i].X = P[i].X;
                qq[nb, i].Y = P[i].Y;
                qq[nb, i].Z = P[i].Z;
            }
            while(b < m)
            {
                int i = b;
                while (b < m && U[b + 1] == U[b])
                    b++;
                int mult = b - i + 1;
                if(mult < p)
                {
                    double[] alphas = new double[p];
                    var numer = U[b] - U[a];
                    for(int j = p; j > mult; j--)
                    {
                        alphas[j-mult-1] = numer/(U[a+j]-U[a]);
                    }
                    int r = p-mult;
                    for(int j=1; j <= r; j++)
                    {
                        var save = r - j;
                        int s = mult + j;
                        for(int k=p; k >=s; k--)
                        {
                            var alpha = alphas[k - s];
                            qq[nb, k].X = alpha * qq[nb, k].X + (1 - alpha) * qq[nb, k - 1].X;
                            qq[nb, k].Y = alpha * qq[nb, k].Y + (1 - alpha) * qq[nb, k - 1].Y;
                            qq[nb, k].Z = alpha * qq[nb, k].Z + (1 - alpha) * qq[nb, k - 1].Z;
                        }
                        if(b < m)
                        {
                            qq[nb + 1, save] = qq[nb, p];
                        }
                    }
                }
                nb = nb + 1;
                if(b < m)
                {
                    for(int j = p - mult; j <= p; j++)
                    {
                        qq[nb, j].X = P[b - p + j].X;
                        qq[nb, j].Y = P[b - p + j].Y;
                        qq[nb, j].Z = P[b - p + j].Z;
                    }
                    a = b;
                    b = b + 1;
                }
            }
            int nrows = qq.GetLength(0);
            Q = new Point[nrows][];
            for(int i = 0; i < nrows; i++)
            {
                Q[i] = new Point[p + 1];
                for(int j=0; j<=p; j++)
                {
                    Q[i][j] = Point.ByCoordinates(
                        qq[i, j].X, qq[i, j].Y, qq[i, j].Z);
                }
            }
        }
    }
}
