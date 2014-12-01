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


namespace Grapher
{

    public struct localPoint 
    {
        public double X, Y;

        public localPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(localPoint pt)
        {
            double diffX = pt.X - X;
            double diffY = pt.Y - Y;

            return Math.Sqrt(diffX * diffX - diffY * diffY);
        }
    }

    public class scatterPlot
    {
        private scatterPlot()
        {

        }

        public static void create(IList<double> XValues, IList<double> YValues, String exportLocation = "Desktop", String chartTitle = "Chart title", string XLabel = "X Values", string YLabel = "Y Values", int width = 500, int height = 500)
        {
            if (exportLocation == "" || string.Compare(exportLocation, "Desktop") == 0 || exportLocation == null)
                exportLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //sanitize the incoming data
            if (XValues.Count() <= 0 || YValues.Count() <= 0)
            {
                throw new ArgumentException("The inputs need to be at least 1 item.");
            }

            if (XValues.GetType() != typeof(double[]) || YValues.GetType() != typeof(double[]))
            {
                throw new ArgumentException("Currently only numbers are supported on X and Y values, sorry!");
            }

            if (XValues.Count() != YValues.Count())
            {
                throw new ArgumentException("The values of X and Y are not of same count. Not currently supported, sorry!");
            }


            //get all the axis values for X and Y axis
            var xAxisVals = findAxis(XValues);
            var yAxisVals = findAxis(YValues);

            double maxX = XValues.Max();
            double minX = XValues.Min();
            double maxY = YValues.Max();
            double minY = YValues.Min();

            var chartWid = width; var chartHei = height;
            var chartMargin = 50;
            chartWid = chartWid + chartMargin;
            chartHei = chartHei + chartMargin;

            var minPosX = chartMargin; var minPosY = chartMargin;
            var maxPosX = chartWid - chartMargin; var maxPosY = chartHei - chartMargin;

            var SVGContent = "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='" + chartWid + "px' height='" + chartHei + "px' viewBox='0 0 " + chartWid + " " + chartHei + "' xml:space='preserve' >     \n";

            //draw graph background. The plot is always 500pt by 500pt
            SVGContent += "<rect x='0' y='0' width='" + chartWid + "' height='" + chartHei + "' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />    \n";

            //draw axis and marking
            SVGContent += "<g name='xaxis'>  \n";
            for (int i = 0; i < xAxisVals.Count; ++i)
            {
                var x = minPosX + (maxPosX - minPosX) * i / (xAxisVals.Count - 1);
                SVGContent += "<path d='M" + x + " " + chartMargin + " L" + x + " " + (chartHei - chartMargin) + "' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>     \n";
                SVGContent += "<text x='" + x + "' y='" + (chartHei - chartMargin + 15) + "' style='fill:#888; font-family: sans-serif; font-size: 10px;' text-anchor='middle'>" + xAxisVals[i] + "</text>";

            }
            SVGContent += "</g>  \n";

            SVGContent += "<g name='yaxis'>  \n";
            for (int i = 0; i < yAxisVals.Count; ++i)
            {
                var x = minPosY + (maxPosY - minPosY) * i / (yAxisVals.Count - 1);
                SVGContent += "<path d='M" + chartMargin + " " + x + " L" + (chartWid - chartMargin) + " " + x + "' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>     \n";
                SVGContent += "<text x='" + (chartMargin - 10) + "' y='" + x + "' style='fill:#888; font-family: sans-serif; font-size: 10px; ' text-anchor='middle' transform='rotate(270 " + (chartMargin - 10) + "," + x + ")'>" + yAxisVals[yAxisVals.Count - i - 1] + "</text>";
            }
            SVGContent += "</g>  \n";

            SVGContent += "<rect x='" + chartMargin + "' y='" + chartMargin + "' width='" + (chartWid - 2 * chartMargin) + "' height='" + (chartHei - 2 * chartMargin) + "' style='stroke-width:1px; stroke: #AAA; fill: none; shape-rendering: crispEdges;' />   \n";

            //draw values along axis
            SVGContent += "<text x='" + (chartWid / 2) + "' y='" + (30) + "' style='fill:#222; font-family: sans-serif; font-size: 12px;' text-anchor='middle'>" + chartTitle + "</text>";
            SVGContent += "<text x='" + (chartWid / 2) + "' y='" + (chartHei - 15) + "' style='fill:#444; font-family: sans-serif; font-size: 10px;' text-anchor='middle'>" + XLabel + "</text>";
            SVGContent += "<text x='" + (chartMargin / 2) + "' y='" + (chartHei / 2) + "' style='fill:#444; font-family: sans-serif; font-size: 10px;' transform='rotate(270 " + (chartMargin / 2) + ", " + (chartHei / 2) + ")' text-anchor='middle'>" + YLabel + "</text>";
            //SVGContent += "</g>  \n";

            //draw the points on the graph
            for (int i = 0; i < XValues.Count; ++i)
            {

                var posX = (minPosX * (XValues[i] - xAxisVals[xAxisVals.Count - 1]) + maxPosX * (xAxisVals[0] - XValues[i])) / (xAxisVals[0] - xAxisVals[xAxisVals.Count - 1]);
                var posY = (minPosY * (YValues[i] - yAxisVals[yAxisVals.Count - 1]) + maxPosY * (yAxisVals[0] - YValues[i])) / (yAxisVals[0] - yAxisVals[yAxisVals.Count - 1]);

                SVGContent += "<circle id='scatterPoint" + i + "' cx='" + posX + "' cy='" + (chartHei - posY) + "' r='3' fill='rgba(200, 30, 30, 0.7)' />";
                SVGContent += "<text x='" + (posX + 20) + "' y='" + (chartHei - posY + 20) + "'  visibility='hidden' fill='#4192d9' font-family='sans-serif' font-size='12' >(" + XValues[i] + ", " + YValues[i] + ")";
                SVGContent += "<set attributeName='visibility' from='hidden' to='visible' begin='scatterPoint" + i + ".mouseover' end='scatterPoint" + i + ".mouseout'/>";
                SVGContent += "</text>";
            }


            //close the svg
            SVGContent += "</svg>";

            //export as a SVG in desktop
            var file = CreateNewSVGFile(exportLocation, "scatterPlot");
            file.WriteLine(SVGContent);
            file.Close();
        }

        private static System.IO.StreamWriter CreateNewSVGFile(String filePath, String fileName)
        {
            //check for invalid characters in the file name
            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidFileChars) != -1 || fileName.CompareTo("CON") == 0)
                throw new ArgumentException("The file name does not satisfy valid windows file name criteria", "fileName");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + "\\" + fileName + DateTime.Now.ToString("h_mm_ss_fff_tt") + ".svg");
            return file;
        }

        internal static List<double> findAxis(IList<double> values)
        {
            double max = values.Max();
            double min = values.Min();
            double diff = (max - min) / 5;

            ///calculate the significant digits of the diff
            var digits = Math.Truncate((Math.Log10(diff)));

            if (digits < 0)
                digits = digits - 1;
            //check the first integer of the number
            double firstNum = Math.Truncate(diff / Math.Pow(10, digits));

            diff = firstNum * Math.Pow(10, digits);

            //get significant digits in difference
            digits = Math.Truncate(Math.Log10(diff));

            double minAxis = min * Math.Pow(10, digits);
            minAxis = Math.Truncate(minAxis);
            minAxis = minAxis / Math.Pow(10, digits);

            if (minAxis == min)
            {
                minAxis = minAxis - diff;
            }

            List<double> axisValues = new List<double>();
            double currentAxisVal = minAxis;
            while (currentAxisVal < max)
            {
                axisValues.Add(currentAxisVal);
                currentAxisVal = currentAxisVal + diff;
            }

            axisValues.Add(currentAxisVal);
            if (currentAxisVal == max)
            {
                axisValues.Add(currentAxisVal + diff);
            }

            return axisValues;
        }

    }

    public class histogramPlot
    {
        private histogramPlot()
        {

        }

        public static List<int> create(IList<double> values, int intervals = 10, double minValuePlot = -1, double maxValuePlot = -1, String exportLocation = "Desktop", string Label = "X Values", string chartTitle = "", int width = 500, int height = 500)
        {
            if (exportLocation == "" || string.Compare(exportLocation, "Desktop") == 0 || exportLocation == null)
                exportLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //sanitize the incoming data
            if (values.Count() <= 0)
            {
                throw new ArgumentException("The inputs need to be at least 1 item.");
            }

            if (values.GetType() != typeof(double[]))
            {
                throw new ArgumentException("Currently only numbers are supported, sorry!");
            }


            if (minValuePlot == -1 && maxValuePlot == -1)
            {
                minValuePlot = values.Min();
                maxValuePlot = values.Max();
            }

            //make sure the minVal is smaller than the mininum and maxVal is greater than maximium - so that the plot is not cut off
            if (minValuePlot > values.Min())
            {
                throw new ArgumentException("The minValuePlot needs to be smaller than the minumum inside the values list", "minValuePlot");
            }

            if (maxValuePlot < values.Max())
            {
                throw new ArgumentException("The maxValuePlot needs to be bigger than the maximum inside the values list", "maxValuePlot");
            }


            var chartWid = width; var chartHei = height;
            var chartMargin = 50;
            chartWid = chartWid + chartMargin;
            chartHei = chartHei + chartMargin;

            var minPosX = chartMargin + 20; var minPosY = chartMargin;
            var maxPosX = chartWid - chartMargin - 20; var maxPosY = chartHei - chartMargin;

            var diff = (maxValuePlot - minValuePlot) / intervals;

            List<double> axisVals = new List<double>();

            for (int i = 0; i <= intervals; ++i)
            {
                axisVals.Add(minValuePlot + i * diff);
            }

            //draw the xAxis marking
            var SVGContent = "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='" + chartWid + "px' height='" + chartHei + "px' viewBox='0 0 " + chartWid + " " + chartHei + "' xml:space='preserve' >     \n";

            //draw graph background. The plot is always 500pt by 500pt
            SVGContent += "<rect x='0' y='0' width='" + chartWid + "' height='" + chartHei + "' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />    \n";




            List<int> histogram = new List<int>(new int[intervals]);

            for (int i = 0; i < intervals; ++i)
            {
                histogram[i] = 0;
            }

            //find the histogram values
            foreach (double val in values)
            {
                //var intervalNumber = 0;
                for (int i = 0; i < intervals; ++i)
                {
                    if (val < axisVals[i + 1] && val >= axisVals[i])
                    {
                        histogram[i] = histogram[i] + 1;
                        break;
                    }
                    if (val == values.Max())
                    {
                        histogram[intervals - 1] += 1;
                        break;
                    }
                }
            }

            //the y axis will always start from 0 to the max of the numbers
            var yAxisVals = findAxis(histogram);

            SVGContent += "<g name='yaxis'>  \n";
            for (int i = 0; i < yAxisVals.Count; ++i)
            {
                var x = minPosY + (maxPosY - minPosY) * i / (yAxisVals.Count - 1);
                SVGContent += "<path d='M" + chartMargin + " " + x + " L" + (chartWid - chartMargin) + " " + x + "' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>     \n";
                SVGContent += "<text x='" + (chartMargin - 10) + "' y='" + x + "' style='fill:#888; font-family: sans-serif; font-size: 10px; ' text-anchor='middle' transform='rotate(270 " + (chartMargin - 10) + "," + x + ")'>" + yAxisVals[yAxisVals.Count - i - 1] + "</text>";
            }
            SVGContent += "</g>  \n";

            SVGContent += "<g name='xaxis'>  \n";
            for (int i = 0; i < axisVals.Count; ++i)
            {
                var x = minPosX + (maxPosX - minPosX) * i / (axisVals.Count - 1);
                SVGContent += "<path d='M" + x + " " + (chartHei - chartMargin - 7) + " L" + x + " " + (chartHei - chartMargin + 7) + "' style='stroke-width:1px; stroke: #AAA; fill: none; shape-rendering: crispEdges;'/>     \n";
                SVGContent += "<text x='" + x + "' y='" + (chartHei - chartMargin + 15) + "' style='fill:#888; font-family: sans-serif; font-size: 10px;' text-anchor='middle'>" + axisVals[i] + "</text>";

            }
            SVGContent += "</g>  \n";

            //draw the rectangles
            SVGContent += "<g name='yaxis'>  \n";
            for (int i = 0; i < histogram.Count; ++i)
            {
                var x = minPosX + (maxPosX - minPosX) * i / (histogram.Count);
                var y = (minPosY * (histogram[i] - yAxisVals[yAxisVals.Count - 1]) + maxPosY * (yAxisVals[0] - histogram[i])) / (yAxisVals[0] - yAxisVals[yAxisVals.Count - 1]);

                SVGContent += "<rect id='histogramBar"+i+"' x='" + x + "' y='" + (chartHei - y) + "' width='" + ((maxPosX - minPosX) / (histogram.Count)) + "' height='" + (y - chartMargin) + "' style='stroke-width:1px; stroke: rgba(200, 30, 30, 0.9); fill: rgba(200, 30, 30, 0.7); shape-rendering: crispEdges;' />   \n";
                SVGContent += "<text x='" + (x) + "' y='" + (chartHei - y - 10) + "'  visibility='hidden' fill='#4192d9' font-family='sans-serif' font-size='12' >" + histogram[i] + "<text/>";
                SVGContent += "<set attributeName='visibility' from='hidden' to='visible' begin='histogramBar" + i + ".mouseover' end='histogramBar" + i + ".mouseout'/>";
                SVGContent += "</text>";
            }
            SVGContent += "</g> \n";

            SVGContent += "<rect x='" + chartMargin + "' y='" + chartMargin + "' width='" + (chartWid - 2 * chartMargin) + "' height='" + (chartHei - 2 * chartMargin) + "' style='stroke-width:1px; stroke: #AAA; fill: none; shape-rendering: crispEdges;' />   \n";



            if (chartTitle == "")
                chartTitle = "Histogram of " + Label;

            //draw values along axis
            SVGContent += "<text x='" + (chartWid / 2) + "' y='" + (30) + "' style='fill:#222; font-family: sans-serif; font-size: 12px;' text-anchor='middle'>" + chartTitle + "</text>";
            SVGContent += "<text x='" + (chartWid / 2) + "' y='" + (chartHei - 15) + "' style='fill:#444; font-family: sans-serif; font-size: 10px;' text-anchor='middle'>" + Label + "</text>";
            SVGContent += "<text x='" + (chartMargin / 2) + "' y='" + (chartHei / 2) + "' style='fill:#444; font-family: sans-serif; font-size: 10px;' transform='rotate(270 " + (chartMargin / 2) + ", " + (chartHei / 2) + ")' text-anchor='middle'>" + "Occurances" + "</text>";

            //close the svg
            SVGContent += "</svg>";
            //export as a SVG in desktop
            var file = CreateNewSVGFile(exportLocation, "histogramPlot");
            file.WriteLine(SVGContent);
            file.Close();

            return histogram;
        }

        private static System.IO.StreamWriter CreateNewSVGFile(String filePath, String fileName)
        {
            //check for invalid characters in the file name
            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidFileChars) != -1 || fileName.CompareTo("CON") == 0)
                throw new ArgumentException("The file name does not satisfy valid windows file name criteria", "fileName");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + "\\" + fileName + DateTime.Now.ToString("h_mm_ss_fff_tt") + ".svg");
            return file;
        }

        private static List<double> findAxis(IList<int> values)
        {
            double max = values.Max();
            double min = 0;
            double diff = (max - min) / 5;

            ///calculate the significant digits of the diff
            var digits = Math.Truncate((Math.Log10(diff)));

            if (digits < 0)
                digits = digits - 1;
            //check the first integer of the number
            double firstNum = Math.Truncate(diff / Math.Pow(10, digits));

            diff = firstNum * Math.Pow(10, digits);

            //get significant digits in difference
            digits = Math.Truncate(Math.Log10(diff));

            double minAxis = min * Math.Pow(10, digits);
            minAxis = Math.Truncate(minAxis);
            minAxis = minAxis / Math.Pow(10, digits);

            /*
            if (minAxis == min)
            {
                minAxis = minAxis - diff;
            }*/

            List<double> axisValues = new List<double>();
            double currentAxisVal = minAxis;
            while (currentAxisVal < max)
            {
                axisValues.Add(currentAxisVal);
                currentAxisVal = currentAxisVal + diff;
            }

            axisValues.Add(currentAxisVal);
            if (currentAxisVal == max)
            {
                axisValues.Add(currentAxisVal + diff);
            }

            return axisValues;
        }

    }
    /*
    public class clusterPlot
    {
        private clusterPlot()
        {
        }

        public static List<List<localPoint>> kMeansClustering(IList<double> XValues, IList<double> YValues, int k, String exportLocation = "Desktop", String chartTitle = "Chart title", string XLabel = "X Values", string YLabel = "Y Values", int width = 500, int height = 500)
        {
            if (exportLocation == "" || string.Compare(exportLocation, "Desktop") == 0 || exportLocation == null)
                exportLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //sanitize the incoming data
            if (XValues.Count() <= 0 || YValues.Count() <= 0)
            {
                throw new ArgumentException("The inputs need to be at least 1 item.");
            }

            if (XValues.GetType() != typeof(double[]) || YValues.GetType() != typeof(double[]))
            {
                throw new ArgumentException("Currently only numbers are supported on X and Y values, sorry!");
            }

            if (XValues.Count() != YValues.Count())
            {
                throw new ArgumentException("The values of X and Y are not of same count. Not currently supported, sorry!");
            }

            //get all the axis values for X and Y axis
            var xAxisVals = scatterPlot.findAxis(XValues);
            var yAxisVals = scatterPlot.findAxis(YValues);

            double maxX = XValues.Max();
            double minX = XValues.Min();
            double maxY = YValues.Max();
            double minY = YValues.Min();

            var chartWid = width; var chartHei = height;
            var chartMargin = 50;
            chartWid = chartWid + chartMargin;
            chartHei = chartHei + chartMargin;

            var minPosX = chartMargin; var minPosY = chartMargin;
            var maxPosX = chartWid - chartMargin; var maxPosY = chartHei - chartMargin;

            var SVGContent = "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='" + chartWid + "px' height='" + chartHei + "px' viewBox='0 0 " + chartWid + " " + chartHei + "' xml:space='preserve' >     \n";

            //draw graph background. The plot is always 500pt by 500pt
            SVGContent += "<rect x='0' y='0' width='" + chartWid + "' height='" + chartHei + "' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />    \n";

            //draw axis and marking
            SVGContent += "<g name='xaxis'>  \n";
            for (int i = 0; i < xAxisVals.Count; ++i)
            {
                var x = minPosX + (maxPosX - minPosX) * i / (xAxisVals.Count - 1);
                SVGContent += "<path d='M" + x + " " + chartMargin + " L" + x + " " + (chartHei - chartMargin) + "' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>     \n";
                SVGContent += "<text x='" + x + "' y='" + (chartHei - chartMargin + 15) + "' style='fill:#888; font-family: sans-serif; font-size: 10px;' text-anchor='middle'>" + xAxisVals[i] + "</text>";

            }
            SVGContent += "</g>  \n";

            SVGContent += "<g name='yaxis'>  \n";
            for (int i = 0; i < yAxisVals.Count; ++i)
            {
                var x = minPosY + (maxPosY - minPosY) * i / (yAxisVals.Count - 1);
                SVGContent += "<path d='M" + chartMargin + " " + x + " L" + (chartWid - chartMargin) + " " + x + "' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>     \n";
                SVGContent += "<text x='" + (chartMargin - 10) + "' y='" + x + "' style='fill:#888; font-family: sans-serif; font-size: 10px; ' text-anchor='middle' transform='rotate(270 " + (chartMargin - 10) + "," + x + ")'>" + yAxisVals[yAxisVals.Count - i - 1] + "</text>";
            }
            SVGContent += "</g>  \n";

            SVGContent += "<rect x='" + chartMargin + "' y='" + chartMargin + "' width='" + (chartWid - 2 * chartMargin) + "' height='" + (chartHei - 2 * chartMargin) + "' style='stroke-width:1px; stroke: #AAA; fill: none; shape-rendering: crispEdges;' />   \n";

            //draw values along axis
            SVGContent += "<text x='" + (chartWid / 2) + "' y='" + (30) + "' style='fill:#222; font-family: sans-serif; font-size: 12px;' text-anchor='middle'>" + chartTitle + "</text>";
            SVGContent += "<text x='" + (chartWid / 2) + "' y='" + (chartHei - 15) + "' style='fill:#444; font-family: sans-serif; font-size: 10px;' text-anchor='middle'>" + XLabel + "</text>";
            SVGContent += "<text x='" + (chartMargin / 2) + "' y='" + (chartHei / 2) + "' style='fill:#444; font-family: sans-serif; font-size: 10px;' transform='rotate(270 " + (chartMargin / 2) + ", " + (chartHei / 2) + ")' text-anchor='middle'>" + YLabel + "</text>";

            List<localPoint> points = new List<localPoint>();

            for (int i = 0; i < XValues.Count; ++i)
            {
                var newPt = new localPoint(XValues[i], YValues[i]);
                points.Add(newPt);
            }

            //kMeans clustering algorithm
            List<List<localPoint>> clusters = kMeansAlgorithm(points, k, XValues.Min(), XValues.Max(), YValues.Min(), YValues.Max());
            

            //draw the points on the graph
            for (int i = 0; i < clusters.Count; ++i)
            {
                Random r = new Random();
                System.Threading.Thread.Sleep(r.Next(0, 10));
                var rColor = r.Next(200);
                System.Threading.Thread.Sleep(r.Next(0, 10));
                var gColor = r.Next(200);
                System.Threading.Thread.Sleep(r.Next(0, 10));
                var bColor = r.Next(200);

                //string clusterColor = "rgba(" + rColor + "," + gColor + "," + bColor + ",0.2)";
                string clusterColor = "rgba("+rColor+", "+gColor+", "+bColor+", 0.7)";
                for (int j = 0; j < clusters[i].Count; ++j)
                {
                    //simply plot the points for now
                    var posX = (minPosX * (clusters[i][j].X - xAxisVals[xAxisVals.Count - 1]) + maxPosX * (xAxisVals[0] - clusters[i][j].X)) / (xAxisVals[0] - xAxisVals[xAxisVals.Count - 1]);
                    var posY = (minPosY * (clusters[i][j].Y - yAxisVals[yAxisVals.Count - 1]) + maxPosY * (yAxisVals[0] - clusters[i][j].Y)) / (yAxisVals[0] - yAxisVals[yAxisVals.Count - 1]);

                    SVGContent += " <circle cx='" + posX + "' cy='" + (chartHei - posY) + "' r='3' fill='" + clusterColor+ "' />";
                }
            }

            //close the svg
            SVGContent += "</svg>";

            //export as a SVG in desktop
            var file = CreateNewSVGFile(exportLocation, "kMeansClusterPlot");
            file.WriteLine(SVGContent);
            file.Close();

            return clusters;
        } 

        private static System.IO.StreamWriter CreateNewSVGFile(String filePath, String fileName)
        {
            //check for invalid characters in the file name
            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidFileChars) != -1 || fileName.CompareTo("CON") == 0)
                throw new ArgumentException("The file name does not satisfy valid windows file name criteria", "fileName");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + "\\" + fileName + DateTime.Now.ToString("h_mm_ss_fff_tt") + ".svg");
            return file;
        }

        private static List<double> AfindAxis(IList<double> values)
        {
            double max = values.Max();
            double min = values.Min();
            double diff = (max - min) / 5;

            ///calculate the significant digits of the diff
            var digits = Math.Truncate((Math.Log10(diff)));

            if (digits < 0)
                digits = digits - 1;
            //check the first integer of the number
            double firstNum = Math.Truncate(diff / Math.Pow(10, digits));

            diff = firstNum * Math.Pow(10, digits);

            //get significant digits in difference
            digits = Math.Truncate(Math.Log10(diff));

            double minAxis = min * Math.Pow(10, digits);
            minAxis = Math.Truncate(minAxis);
            minAxis = minAxis / Math.Pow(10, digits);

            if (minAxis == min)
            {
                minAxis = minAxis - diff;
            }

            List<double> axisValues = new List<double>();
            double currentAxisVal = minAxis;
            while (currentAxisVal < max)
            {
                axisValues.Add(currentAxisVal);
                currentAxisVal = currentAxisVal + diff;
            }

            axisValues.Add(currentAxisVal);
            if (currentAxisVal == max)
            {
                axisValues.Add(currentAxisVal + diff);
            }

            return axisValues;
        }

        private static List<List<localPoint>> kMeansAlgorithm(List<localPoint> points, int k, double minXVal, double  maxXVal, double minYVal, double maxYVal)
        {
            List<List<localPoint>> clusters = new List<List<localPoint>>();
            List<localPoint> centroids = new List<localPoint>();

        for (int i = 0; i < k; ++i)
        {
            clusters.Add(new List<localPoint>());
        }

            //find the minXVal, minYVal of these points


        //finding max and min X, Y, Z
        var minPt = new localPoint(minXVal, maxYVal);
        var maxPt = new localPoint(maxXVal, minYVal);

        //intitialize random centers for a given k
        for (int i = 0; i < k; ++i)
        {
            Random r = new Random();

            System.Threading.Thread.Sleep(r.Next(0, 10));
            double xVal = r.NextDouble() * (maxPt.X - minPt.X) + minPt.X;
            System.Threading.Thread.Sleep(r.Next(0, 10));
            double yVal = r.NextDouble() * (maxPt.Y - minPt.Y) + minPt.Y;
            //Thread.Sleep(r.Next(0, 10));
            //double zVal = r.NextDouble() * (maxPt.Z - minPt.Z) + minPt.Z;

            localPoint tempPt = new localPoint(xVal, yVal);
            centroids.Add(new localPoint(xVal, yVal));
        }

        

        double prevDistance = 0;

        //assign points to the initial centroid 
        for (int i = 0; i < points.Count; ++i)
        {
            int clusterToAdd = 0;
            localPoint pointInConsideration = points[i];
            prevDistance = pointInConsideration.DistanceTo(centroids[0]);

            for (int j = 1; j < k; ++j)
            {
                double distance = pointInConsideration.DistanceTo(centroids[j]);

                if (distance < prevDistance)
                {
                    clusterToAdd = j;
                    prevDistance = distance;
                }
            }

            clusters[clusterToAdd].Add(pointInConsideration);
        }

        var oldCentroids = new List<localPoint>();

        loop:
        oldCentroids = centroids;
        //compute new centroids for given clusters. If the clusters are empty, go back to old centroid
        for (int i = 0; i < clusters.Count; ++i)
        {
            if (clusters[i].Count == 0)
                centroids[i] = centroids[i];
            else
            {
                double xSum = 0, ySum = 0;

                for(int j=0; j<clusters[i].Count; ++j)
                {
                    xSum = xSum + clusters[i][j].X;
                    ySum = ySum + clusters[i][j].Y;
                    
                }

                centroids[i] = new localPoint(xSum / clusters[i].Count, ySum / clusters[i].Count);
            }

        }

        for (int i = 0; i < centroids.Count; ++i )
        {
            if((oldCentroids[i].X != centroids[i].X)||(oldCentroids[i].Y != centroids[i].Y))
            {
                oldCentroids = centroids;
                goto loop;
            }
        }

        return clusters;
        }
    }
     * 
     */

    public class pieChartPlot
    {
        private pieChartPlot()
        {
        }

        public static void create(IList<String> Items, IList<double> Values, String exportLocation = "Desktop", String chartTitle = "Chart title", int width = 500, int height = 500)
        {
            if (exportLocation == "" || string.Compare(exportLocation, "Desktop") == 0 || exportLocation == null)
                exportLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //sanitize the incoming data
            if (Items.Count() <= 0 || Values.Count() <= 0)
            {
                throw new ArgumentException("The inputs need to be at least 1 item.");
            }

            if (Values.GetType() != typeof(double[]))
            {
                throw new ArgumentException("Currently only numbers are supported on the values, sorry!");
            }

            if (Items.GetType() != typeof(string[]))
            {
                throw new ArgumentException("Only Strings are allowed for the items, sorry!");
            }

            var chartWid = width;
            var chartHei = height;

            var chartCenterXY = chartWid / 2;
            var chartRad = chartWid/2-20;
            var legendWid = 200;
            var legendHei = height;

            // the chart is 400px by 400px and the legend is 200px by 400px
            var SVGContent = "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='" + (chartWid + legendWid) + "px' height='" + chartHei + "px' viewBox='0 0 " + (chartWid + legendWid) + " " + chartHei + "' xml:space='preserve' >     \n";

            //draw graph background. The plot is always 500pt by 500pt
            SVGContent += "<rect x='0' y='0' width='" + chartWid + "' height='" + chartHei + "' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />\n";
            SVGContent += "<rect x='"+chartWid+"' y='"+0+"' width='" + legendWid + "' height='" + legendHei + "' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />\n";

            //calculate the % of each of the items
            List<double> percentages = new List<double>();
            double sum = Values.Sum();
            foreach (var value in Values)
            {
                percentages.Add((value * Math.PI * 2)/ sum);
            }

            double lastPercentage = -1*Math.PI/2;
            string[] color = new string[]{"71d0f4", "e16452", "74c493", "f2d435", "8361a9",
                             "9abf88", "ef778c", "5698c4", "8c4646", "578b7e",
                             "cbc49d", "ef464c", "a8c542", "322f28", "9f3169",
                             "e99123", "878787", "6e8314", "2854a4", "555555"};

            var i = 0;
            
            //generate charts
            foreach(var percentage in percentages)
            {
                var fillColor = color[i++];
                if (i >= 20)
                {
                    i = 0;
                }

                var largeArcFlag = (percentage < Math.PI) ? 0 : 1;
                
                SVGContent += "<path d='M " + chartCenterXY + "," + chartCenterXY + 
                    " l " + chartRad * Math.Cos(lastPercentage) + "," + chartRad * Math.Sin(lastPercentage) +
                    " A" + chartRad + "," + chartRad + " " + lastPercentage + " " + largeArcFlag + ",1 " + (chartCenterXY + chartRad * Math.Cos(lastPercentage + percentage)) + "," + (chartCenterXY + chartRad * Math.Sin(lastPercentage + percentage)) + " z' fill='#" + fillColor + "' stroke='rgba(0, 0, 0, 0.4)' stroke-width='0' />\n";
                SVGContent += "<text x='" + ((chartCenterXY + chartRad * Math.Cos(lastPercentage + percentage/2))) + "' y='" + ((chartCenterXY + chartRad * Math.Sin(lastPercentage + percentage/2))) + "' fill='#444444' font-family='sans-serif' font-size='12'>" + "  " + (i) +"</text>";
                lastPercentage += percentage;
            }

            var margin = 20;
            SVGContent += "<text x='" + (margin + 25) + "' y='" + 15 + "' fill='#444444' font-family='sans-serif' font-size='12'>" + chartTitle+ "</text>";
            
            i = 0;
            //generate legend for the chart
            foreach (var item in Items)
            {
                if (i >= 20)
                {
                    i = 0;
                }
                SVGContent += "<rect x='"+(chartWid+margin)+"' y='"+((i*25)+50)+"' width='20' height='20' rx='10' ry='10' fill='#"+color[i]+"' />\n";
                SVGContent += "<text x='"+(chartWid+margin+25)+"' y='"+((i*25)+50+15)+"' fill='#444444' font-family='sans-serif' font-size='12'>"+"  "+(i+1)+" "+item+" ("+Values[i]+")</text>";
                i = i + 1;
            }

            //close the svg
            SVGContent += "</svg>";

            //export as a SVG in desktop
            var file = CreateNewSVGFile(exportLocation, "pieChart");
            file.WriteLine(SVGContent);
            file.Close();
        }

        private static System.IO.StreamWriter CreateNewSVGFile(String filePath, String fileName)
        {
            //check for invalid characters in the file name
            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidFileChars) != -1 || fileName.CompareTo("CON") == 0)
                throw new ArgumentException("The file name does not satisfy valid windows file name criteria", "fileName");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + "\\" + fileName + DateTime.Now.ToString("h_mm_ss_fff_tt") + ".svg");
            return file;
        }

    }
}