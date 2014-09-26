using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Nodes;
/*
 * any graph always produces 500pts by 500pts

 * Types of graph nodes
 * scatter plot, axis, values at top
 * Column chart, single, axis, values at top
 * Histogram single, axis, values at top
 * Area chart, single, axis, values at top
 * Stepped chart single, axis, values at top
 * Line chart single, axis, values at top
 * Pie chart, legend with name and percentage
 * Donut chart, legend with name and percentage
 * 
 * node to view graph
 */


namespace grapher
{
    public class Program
    {
        /*
        static void Main(string[] args)
        {
            List<double> x = new List<double> { 14.2, 16.4, 11.9, 15.2, 18.5, 22.1, 19.4, 25.1, 23.4, 18.1, 22.6, 17.2};
            List<double> y = new List<double> { 215, 325, 185, 332, 406, 522, 412, 614, 544, 421, 445, 408 };//9.2, 1, 4.4, 6.3, 0.6, 1.2, 9, 3.4, 2.3, 3.1};
            scatterPlot(x, y);
        }
        */
        public static void scatterPlot(IList<double> XValues, IList<double> YValues)
        {
            double maxX = XValues.Max();
            double minX = XValues.Min();
            double diffX = (maxX - minX) / 5;
            maxX = maxX + diffX/2;
            minX = minX - diffX / 2;
            diffX = (maxX - minX) / 5;

            double maxY = YValues.Max();
            double minY = YValues.Min();
            double diffY = (maxY - minY) / 5;
            maxY = maxY + diffY / 2;
            minY = minY - diffY / 2;
            diffY = (maxY - minY) / 5;

            var minPosX = 50; var minPosY = 50;
            var maxPosX = 450; var maxPosY = 450;

            var SVGContent = "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='500px' height='500px' viewBox='0 0 500 500' xml:space='preserve' >     \n";
            
            //draw graph background. The plot is always 500pt by 500pt
            SVGContent += "<rect x='0' y='0' width='500' height='500' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />    \n";

            //draw axis and marking
            SVGContent += "<g name='axis'>  \n";
            for (int i = 0; i < 6; ++i)
            {
                var x = (400 / 5 * i) + 50; 
                SVGContent += "<path d='M" + x + " 50 L" + x + " 450' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>     \n";
                SVGContent += "<text x='" + x + "' y='465' style='fill:#666; font-family: sans-serif; font-size: 10px;' text-anchor='middle'>" + Math.Round(minX + (diffX*i), 3) + "</text>";

                SVGContent += "<path d='M50 " + x + " L450 " + x + "' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>     \n";
                SVGContent += "<text x='40' y='" + x + "' style='fill:#666; font-family: sans-serif; font-size: 10px; ' text-anchor='middle' transform='rotate(270 " + 40 + "," + x + ")'>" + Math.Round(minY + ((5 - i) * diffY), 3) + "</text>";
            }

            SVGContent += "<rect x='50' y='50' width='400' height='400' style='stroke-width:1px; stroke: #AAA; fill: none; shape-rendering: crispEdges;' />   \n";
            
            //draw values along axis
            SVGContent += "<text x='" + 250 + "' y='485' style='fill:#444; font-family: sans-serif; font-size: 12px;' text-anchor='middle'>X Values</text>";
            SVGContent += "<text x='" + 25 + "' y='250' style='fill:#444; font-family: sans-serif; font-size: 12px;' transform='rotate(270 25, 250)' text-anchor='middle'>Y Values</text>";
            SVGContent += "</g>  \n";

            //draw the points on the graph
            for (int i = 0; i < XValues.Count(); ++i)
            {
                var posX = (minPosX *( XValues[i] - maxX) + maxPosX * (minX - XValues[i])) / (minX - maxX);
                var posY = (minPosY * (YValues[i] - maxY) + maxPosY * (minY - YValues[i])) / (minY - maxY);

                SVGContent += " <circle cx='"+posX+"' cy='"+(500-posY)+"' r='3' fill='rgba(200, 30, 30, 0.8)' />";
            }


            //close the svg
            SVGContent += "</svg>";

            //export as a SVG in desktop
            var file = CreateNewSVGFile("C:\\users\\t_elane\\Desktop\\", "scatterPlot");
            file.WriteLine(SVGContent);
            file.Close();
        }


        private static System.IO.StreamWriter CreateNewSVGFile(String filePath, String fileName)
        {
            //check for invalid characters in the file name
            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidFileChars) != -1 || fileName.CompareTo("CON") == 0)
                throw new ArgumentException("The file name does not satisfy valid windows file name criteria", "fileName");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + fileName + ".svg");
            return file;
        }
    }
}
