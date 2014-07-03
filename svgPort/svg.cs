using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;

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

        
        public static void exportPathsAsSVG(List<Geometry> geometry, String exportLocation, string fileName )
        {
            
            //TODO: Handle replication for geometry

            var file = CreateNewSVGFile(exportLocation, fileName);
            
            //fill the SVG headers
            preSVGBody(file);
            
            //start the svg tag
            file.WriteLine(@"<svg version=""1.1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" x=""0px"" y=""0px"" width=""1000px"" height=""1000px"" viewBox=""0 0 1000 1000"" xml:space=""preserve""> ");
            

            //segregate all points
            List<Point> pts = new List<Point>();
            List<Line> lines = new List<Line>();
            List<Ellipse> ellipses = new List<Ellipse>();
            List<Circle> circles = new List<Circle>();
            List<Polygon> polygons = new List<Polygon>();
            List<Curve> polylines = new List<Curve>();
            //TODO: Need to support paths


            for (int i = 0; i < geometry.Count; ++i)
            {
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

                file.WriteLine("<line x1='" + x1.ToString() + "' y1='" + y1.ToString() + "' x2='"+ x2.ToString() +"' y2='"+y2.ToString()+"' style='stroke:black; stroke-width: 1'/>");

                if (i == pts.Count - 1)
                    file.WriteLine("</g>");
            }



            //complete the svg tag
            file.WriteLine("</svg>");
            file.Close();
        }
    }
}
