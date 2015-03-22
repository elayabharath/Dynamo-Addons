using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //TODO: Need to support paths


            for (int i = 0; i < geometry.Count(); ++i)
            {
                geometry[i] = geometry[i].Translate(0 - minPt.X, 0 - minPt.Y, 0);

                if (geometry[i].GetType() == typeof(Point))
                    pts.Add((Point)geometry[i]);

                else if (geometry[i].GetType() == typeof(Line))
                    lines.Add((Line)geometry[i]);

                else if (geometry[i].GetType() == typeof(Ellipse))
                    ellipses.Add((Ellipse)geometry[i]);

                else if (geometry[i].GetType() == typeof(Circle))
                    circles.Add((Circle)geometry[i]);

                else if (geometry[i].GetType() == typeof(Polygon))
                    polygons.Add((Polygon)geometry[i]);

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



            //complete the svg tag
            file.WriteLine("</svg>");
            file.Close();
        }

        struct point
        {
            public double X;
            public double Y;
            public double Z;
        }

        private static void CurveKnotInsertion(int p, double[] up,
            List<Point> P, double u, int k, int s, int r, 
            out double[] uq, out List<Point> Q)
        {
            
            int np = P.Count() - 1;
            int mp = np + p + 1;
            int nq = np + r;
            int mq = nq + p + 1;
            uq = new double[mq + 1];

            // load new knot vector, uq
            for(int i = 0; i <= k; i++)
            {
                uq[i] = up[i];
            }
            for(int i = 1; i <= r; i++)
            {
                uq[k + i] = u;
            }
            for(int i = k+1; i <= mp; i++)
            {
                uq[i + r] = up[i];
            }

            var qq = new point[nq + 1];
            for(int i = 0; i<= k-p; i++)
            {
                qq[i].X = P[i].X;
                qq[i].Y = P[i].Y;
                qq[i].Z = P[i].Z;
            }
            for(int i = k-s; i <= np; i++)
            {
                qq[i + r].X = P[i].X;
                qq[i + r].Y = P[i].Y;
                qq[i + r].Z = P[i].Z;
            }
            var rr = new point[p + 1];
            for(int i=0; i<= p-s; i++)
            {
                rr[i].X = P[k - p + i].X;
                rr[i].Y = P[k - p + i].Y;
                rr[i].Z = P[k - p + i].Z;
            }
            int l = 0;
            for(int j=1; j <= r; j++)
            {
                l = k - p + j;
                for(int i = 0; i <= p-j-s; i++)
                {
                    var alpha = (u - up[l + i]) / (up[i + k + 1] - up[l + i]);
                    rr[i].X = alpha * rr[i + 1].X + (1 - alpha) * rr[i].X;
                    rr[i].Y = alpha * rr[i + 1].Y + (1 - alpha) * rr[i].Y;
                    rr[i].Z = alpha * rr[i + 1].Z + (1 - alpha) * rr[i].Z;
                }
                qq[l] = rr[0];
                qq[k + r - j - s] = rr[p - j - s];
            }
            for(int i = l+1; i < k-s; i++)
            {
                qq[i] = rr[i - l];
            }
            Q = new List<Point>();
            foreach(var pt in qq)
            {
                //pt.X, pt.Y, pt.Z
                //Q.Add();
            }
        }
    }
}
