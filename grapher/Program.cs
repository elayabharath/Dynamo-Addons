using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

    public class Plot
    {
        protected Plot()
        {
        }

        protected static System.IO.StreamWriter CreateNewSVGFile(String filePath, String fileName)
        {
            //check for invalid characters in the file name
            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidFileChars) != -1 || fileName.CompareTo("CON") == 0)
                throw new ArgumentException("The file name does not satisfy valid windows file name criteria", "fileName");

            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + "\\" + fileName + DateTime.Now.ToString("h_mm_ss_fff_tt") + ".svg");
            return file;
        }
    }

    public class scatterPlot : Plot
    {
        protected scatterPlot() { }

        public static System.Drawing.Bitmap create(IList<double> Values, List<object> chartOptions = null)
        {
            double[] list = new double[Values.Count];
            
            for (int i = 0; i < Values.Count; ++i)
            {
                list[i] = i;
            }
            return create(list, Values, chartOptions);
        }

        public static System.Drawing.Bitmap create(IList<double> XValues, IList<double> YValues, List<object> chartOptions = null)
        {
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


            string chartTitle;
            string XLabel;
            string YLabel;
            double chartAreaWidth;
            double chartAreaHeight;
            int LineOrAreaFlag;
            double plotRadius;
            double opacity;
            int interactiveOption;
            int saveAsSVGOption;
            string saveLocation;

            //read all options here
            if (chartOptions.Count == 11)
            {
                chartTitle = chartOptions[0].ToString();
                XLabel = chartOptions[1].ToString();
                YLabel = chartOptions[2].ToString();
                chartAreaWidth = Convert.ToDouble(chartOptions[3]);
                chartAreaHeight = Convert.ToDouble(chartOptions[4]);
                LineOrAreaFlag = Convert.ToInt32(chartOptions[5]);
                plotRadius = Convert.ToDouble(chartOptions[6]);
                opacity = Convert.ToDouble(chartOptions[7]);
                interactiveOption = Convert.ToInt32(chartOptions[8]);
                saveAsSVGOption = Convert.ToInt32(chartOptions[9]);
                saveLocation = chartOptions[10].ToString();
            }
            else
            {
                chartTitle = "Chart title";
                XLabel = "X Values";
                YLabel = "Y Values";
                chartAreaWidth = 500;
                chartAreaHeight = 500;
                LineOrAreaFlag = 0;
                plotRadius = 2.0;
                opacity = 50;
                interactiveOption = 0;
                saveAsSVGOption = 0;
                saveLocation = "Desktop";
            }

            if (saveLocation == "" || string.Compare(saveLocation, "Desktop") == 0 || saveLocation == null)
                saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            opacity = (opacity <= 0.0) ? 0.01 : opacity / 100;
            opacity = (opacity >= 1.0) ? 1.0 : opacity;

            //get all the axis values for X and Y axis
            var xAxisVals = findAxis(XValues);
            var yAxisVals = findAxis(YValues);

            double maxX = XValues.Max();
            double minX = XValues.Min();
            double maxY = YValues.Max();
            double minY = YValues.Min();

            var chartMargin = 50;
            var chartWid = chartAreaWidth + chartMargin; var chartHei = chartAreaHeight + chartMargin;

            var minPosX = chartMargin; var minPosY = chartMargin;
            var maxPosX = chartWid - chartMargin; var maxPosY = chartHei - chartMargin;

            var SVGStringBuilder = new StringBuilder();
            SVGStringBuilder.Append("<?xml version='1.0' encoding='UTF-8'?>");
            SVGStringBuilder.Append(String.Format("<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='{0}px' height='{1}px' viewBox='0 0 {2} {3}' xml:space='preserve' >\n", chartWid, chartHei, chartWid, chartHei));

            //draw graph background. The plot is always 500pt by 500pt
            SVGStringBuilder.Append(String.Format("<rect x='0' y='0' width='{0}' height='{1}' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />\n", chartWid, chartHei));


            //draw axis and marking
            SVGStringBuilder.Append("<g name='xaxis'>  \n");

            for (int i = 0; i < xAxisVals.Count; ++i)
            {
                var x = minPosX + (maxPosX - minPosX) * i / (xAxisVals.Count - 1);

                SVGStringBuilder.Append(String.Format("<path d='M {0} {1} L {2} {3}' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>\n", x, chartMargin, x, (chartHei - chartMargin)));
                SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' style='fill:#888; font-family: sans-serif; font-size: 10px;' text-anchor='middle'>{2}</text>", x, (chartHei - chartMargin + 15), xAxisVals[i]));
            }
            SVGStringBuilder.Append("</g>  \n");


            SVGStringBuilder.Append("<g name='yaxis'>  \n");
            for (int i = 0; i < yAxisVals.Count; ++i)
            {
                var y = minPosY + (maxPosY - minPosY) * i / (yAxisVals.Count - 1);
                SVGStringBuilder.Append(String.Format("<path d='M{0} {1} L{2} {3}' style='stroke-width:1px; stroke: #DDD; fill: none; shape-rendering: crispEdges;'/>\n", chartMargin, y, (chartWid - chartMargin), y));
                SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' style='fill:#888; font-family: sans-serif; font-size: 10px; ' text-anchor='middle' transform='rotate(270 {2},{3})'>{4}</text>", (chartMargin - 10), y, (chartMargin - 10), y, (yAxisVals[yAxisVals.Count - i - 1])));
            }
            SVGStringBuilder.Append("</g>  \n");

            SVGStringBuilder.Append(String.Format("<rect x='{0}' y='{1}' width='{2}' height='{3}' style='stroke-width:1px; stroke: #AAA; fill: none; shape-rendering: crispEdges;' />\n", chartMargin, chartMargin, (chartWid - 2 * chartMargin), (chartHei - 2 * chartMargin)));

            //draw labels and title along axis
            SVGStringBuilder.Append(String.Format("<g>"));
            SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' font-size='12px' fill='#222' text-anchor='middle' font-family='sans-serif' >{2}</text>", (chartWid / 2), (30), chartTitle));
            SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' font-size='12px' fill='#222' text-anchor='middle' font-family='sans-serif' >{2}</text>", (chartWid / 2), (chartHei - 15), XLabel));
            SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' font-size='12px' fill='#222' text-anchor='middle' font-family='sans-serif' transform='rotate(270 {2},{3})'>{4}</text>", (chartMargin / 2), (chartHei / 2), (chartMargin / 2), (chartHei / 2), YLabel));
            SVGStringBuilder.Append(String.Format("</g>"));

            StringBuilder tempSVGStringBuilder = new StringBuilder();

            var circleColor = "#C81E1E";


            //draw the points on the graph
            if (LineOrAreaFlag == 1)
            {
                circleColor = "rgba(0, 0, 0, 0)";
                tempSVGStringBuilder.Append("<path id='linePlot' d='");
            }

            if (LineOrAreaFlag == 2)
            {
                circleColor = "rgba(0, 0, 0, 0)";
                tempSVGStringBuilder.Append("<path id='linePlot' d='");
            }

            SVGStringBuilder.Append(String.Format("<g>", circleColor));

            for (int i = 0; i < XValues.Count; ++i)
            {

                var posX = (minPosX * (XValues[i] - xAxisVals[xAxisVals.Count - 1]) + maxPosX * (xAxisVals[0] - XValues[i])) / (xAxisVals[0] - xAxisVals[xAxisVals.Count - 1]);
                var posY = (minPosY * (YValues[i] - yAxisVals[yAxisVals.Count - 1]) + maxPosY * (yAxisVals[0] - YValues[i])) / (yAxisVals[0] - yAxisVals[yAxisVals.Count - 1]);

                if (i == 0 && LineOrAreaFlag == 1)
                {
                    tempSVGStringBuilder.Append(String.Format("M {0},{1} ", posX, (chartHei - posY)));
                }
                else if (i == 0 && LineOrAreaFlag == 2)
                {
                    tempSVGStringBuilder.Append(String.Format("M {0},{1} L {2},{3} ", posX, (chartHei - chartMargin), posX, (chartHei - posY)));
                }
                else
                {
                    tempSVGStringBuilder.Append(String.Format("L {0},{1} ", posX, (chartHei - posY)));
                }

                if (i == XValues.Count - 1 && LineOrAreaFlag == 2)
                {
                    tempSVGStringBuilder.Append(String.Format("L {0},{1} ", posX, (chartHei - chartMargin)));
                }

                SVGStringBuilder.Append(String.Format("<circle class='sp' id='sp{0}' cx='{1}' cy='{2}' r='{3}' fill='{4}' style='fill-opacity:{5};'/>", i, posX, (chartHei - posY), plotRadius, circleColor, opacity));
                if (interactiveOption == 1)
                {
                    SVGStringBuilder.Append(String.Format("<text id='tt' x='{0}' y='{1}'  visibility='hidden' fill='#000' font-family='sans-serif' font-size='12' >({2},{3})", (posX + 20), (chartHei - posY + 20), XValues[i], YValues[i]));
                    SVGStringBuilder.Append(String.Format("<set attributeName='visibility' from='hidden' to='visible' begin='sp{0}.mouseover' end='sp{1}.mouseout'/>", i, i));
                    SVGStringBuilder.Append("</text>");
                }
            }


            if (LineOrAreaFlag == 1)
                tempSVGStringBuilder.Append("' style='stroke-width:2px; stroke: rgba(200, 30, 30, 0.7); fill:none; ' />");
            else if (LineOrAreaFlag == 2)
                tempSVGStringBuilder.Append("' style='stroke-width:0; stroke: rgba(200, 30, 30, 0.7); fill:rgba(200, 30, 30, 0.5); ' />");

            SVGStringBuilder.Append(tempSVGStringBuilder.ToString());
            SVGStringBuilder.Append("</g>");
            //SVGStringBuilder.Append("<defs><style type='text/css'><![CDATA[ .sp:hover { stroke: rgba(200, 30, 30, 1); stroke-width:1px; } ]]></style></defs>");
            SVGStringBuilder.Append("</svg>");

            var SVGContent = SVGStringBuilder.ToString();

            if (saveAsSVGOption != 0)
            {
                var file = CreateNewSVGFile(saveLocation, "scatterPlot");
                file.WriteLine(SVGContent);
                file.Close();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SVGContent);


            Svg.SvgDocument svgdoc = new Svg.SvgDocument();

            var xmlStream = new System.IO.MemoryStream(UTF8Encoding.Default.GetBytes(doc.InnerXml));
            svgdoc = Svg.SvgDocument.Open(xmlStream);

            var imageStream = new System.IO.MemoryStream();
            svgdoc.Draw().Save(imageStream, System.Drawing.Imaging.ImageFormat.Bmp);

            System.Drawing.Bitmap imageFile = new System.Drawing.Bitmap(imageStream);
            return imageFile;

        }

        public static List<object> chartOptions(String chartTitle = "Chart title", String XLabel = "X Values", String YLabel = "Y Values",
                                  int chartAreaWidth = 500, int chartAreaHeight = 500, int LineOrAreaFlag = 0, double plotRadius = 2.0, 
                                  double opacity = 50.0, int interactiveOption = 0, int saveAsSVGOption = 0, String saveLocation = "Desktop")
        {
            var optionsList = new List<object>();
            optionsList.Add(chartTitle);
            optionsList.Add(XLabel);
            optionsList.Add(YLabel);
            optionsList.Add(chartAreaWidth);
            optionsList.Add(chartAreaHeight);
            optionsList.Add(LineOrAreaFlag);
            optionsList.Add(plotRadius);
            optionsList.Add(opacity);
            optionsList.Add(interactiveOption);
            optionsList.Add(saveAsSVGOption);
            optionsList.Add(saveLocation);

            return optionsList;
        }

        internal static List<double> findAxis(IList<double> values)
        {
            double max = values.Max();
            double min = values.Min();
            double diff = (max - min) / 5;

            ///calculate the significant digits of the diff
            var digits = Math.Truncate((Math.Log10(diff)));

            if (digits <= 0)
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

    public class guage : Plot
    {
        protected guage() { }

        public static System.Drawing.Bitmap createLinear(double valueToMonitor, List<object> chartOptions = null)
        {
            //sanitize the incoming data
            if (valueToMonitor.GetType() != typeof(double))
            {
                throw new ArgumentException("The value to monitor is not valid, sorry!");
            }

            string label;
            double minValue;
            double maxValue;
            int chartWidth;
            int saveAsSVGOption;
            string saveLocation;

            //read all options here
            if (chartOptions.Count == 6)
            {
                label = chartOptions[0].ToString();
                minValue = Convert.ToDouble(chartOptions[1]);
                maxValue = Convert.ToDouble(chartOptions[2]);
                chartWidth = Convert.ToInt32(chartOptions[3]);
                saveAsSVGOption = Convert.ToInt32(chartOptions[4]);
                saveLocation = chartOptions[5].ToString();
            }
            else
            {
                label = "Label";
                minValue = 0;
                maxValue = 100;
                chartWidth = 500;
                saveAsSVGOption = 1;
                saveLocation = "Desktop";
            }

            if (minValue >= maxValue)
            {
                throw new ArgumentException("Minimum value needs to be smaller than the maximum value.");
            }

            if (chartWidth < 500)
            {
                chartWidth = 500;
            }

            if (saveLocation == "" || string.Compare(saveLocation, "Desktop") == 0 || saveLocation == null)
                saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var progressHeight = 20;
            var chartMargin = 50;
            var chartWid = chartWidth + 2 * chartMargin + 2 * progressHeight; var chartHei = progressHeight + 2 * chartMargin;

            var SVGStringBuilder = new StringBuilder();
            SVGStringBuilder.Append("<?xml version='1.0' encoding='UTF-8'?>");
            SVGStringBuilder.Append(String.Format("<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='{0}px' height='{1}px' viewBox='0 0 {2} {3}' xml:space='preserve' >\n", chartWid, chartHei, chartWid, chartHei));

            //draw graph background. The plot is always 500pt by 500pt
            SVGStringBuilder.Append(String.Format("<rect x='0' y='0' width='{0}' height='{1}' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />\n", chartWid, chartHei));

            var progressWidth = (chartWidth * valueToMonitor - minValue * chartWidth) / (maxValue - minValue);
            if (progressWidth < 0)
                progressWidth = 0;
            if (progressWidth > chartWidth)
                progressWidth = chartWidth;


            var minFillColor = "none";
            var maxFillColor = minFillColor;
            if (valueToMonitor < minValue)
            {
                minFillColor = "rgba(214, 63, 41, 1)";
            }

            if (valueToMonitor > maxValue)
            {
                maxFillColor = "rgba(214, 63, 41, 1)";
            }

            //draw marking
            SVGStringBuilder.Append("<g name='skeleton'>  \n");
            //draw rectangles
            SVGStringBuilder.Append(String.Format("<rect x='{0}' y='{1}' width='{2}' height='{3}' style='fill:#3498DB; shape-rendering: crispEdges;' />\n", chartMargin + progressHeight, chartMargin, progressWidth, progressHeight));
            SVGStringBuilder.Append(String.Format("<rect x='{0}' y='{1}' width='{2}' height='{2}' style='fill:{3}; stroke-width:1px; stroke: #888; shape-rendering: crispEdges;' />\n", chartMargin, chartMargin, progressHeight, minFillColor));
            SVGStringBuilder.Append(String.Format("<rect x='{0}' y='{1}' width='{2}' height='{3}' style='fill:none; stroke-width:1px; stroke: #888; shape-rendering: crispEdges;' />\n", chartMargin + progressHeight, chartMargin, chartWidth, progressHeight));
            SVGStringBuilder.Append(String.Format("<rect x='{0}' y='{1}' width='{2}' height='{2}' style='fill:{3}; stroke-width:1px; stroke: #888; shape-rendering: crispEdges;' />\n", chartMargin + progressHeight + chartWidth, chartMargin, progressHeight, maxFillColor));

            SVGStringBuilder.Append("</g>  \n");

            //chart title
            SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' font-size='12px' fill='#222' text-anchor='middle' font-family='sans-serif' >{2}</text>", (chartWid / 2), (30), label));
            SVGStringBuilder.Append(String.Format("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' style='stroke:#888; stroke-width:1;shape-rendering: crispEdges;'/>", chartMargin + progressHeight, chartMargin, chartMargin + progressHeight, chartMargin + progressHeight + 10));
            SVGStringBuilder.Append(String.Format("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' style='stroke:#888; stroke-width:1;shape-rendering: crispEdges;'/>", chartMargin + progressHeight + chartWidth, chartMargin, chartMargin + progressHeight + chartWidth, chartMargin + progressHeight + 10));

            SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' style='fill:#888; font-family: sans-serif; font-size: 10px; ' text-anchor='middle'>{2}</text>", chartMargin + progressHeight, chartMargin + progressHeight + 20, minValue));
            SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' style='fill:#888; font-family: sans-serif; font-size: 10px; ' text-anchor='middle'>{2}</text>", chartMargin + progressHeight + chartWidth, chartMargin + progressHeight + 20, maxValue));
            SVGStringBuilder.Append(String.Format("<text x='{0}' y='{1}' style='fill:#333; font-family: sans-serif; font-size: 13px; ' text-anchor='middle'>{2}</text>", chartMargin + progressHeight + chartWidth / 2, chartMargin + progressHeight + 20, valueToMonitor));

            SVGStringBuilder.Append("</svg>");

            var SVGContent = SVGStringBuilder.ToString();


            if (saveAsSVGOption != 0)
            {
                var file = CreateNewSVGFile(saveLocation, "scatterPlot");
                file.WriteLine(SVGContent);
                file.Close();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SVGContent);


            Svg.SvgDocument svgdoc = new Svg.SvgDocument();

            var xmlStream = new System.IO.MemoryStream(UTF8Encoding.Default.GetBytes(doc.InnerXml));
            svgdoc = Svg.SvgDocument.Open(xmlStream);

            var imageStream = new System.IO.MemoryStream();
            svgdoc.Draw().Save(imageStream, System.Drawing.Imaging.ImageFormat.Bmp);

            System.Drawing.Bitmap imageFile = new System.Drawing.Bitmap(imageStream);
            return imageFile;



        }

        public static List<object> chartOptions(String label = "Label", double minValue = 0, double maxValue = 100,
                                  int chartWidth = 500, int saveAsSVGOption = 0, String saveLocation = "Desktop")
        {
            var optionsList = new List<object>();
            optionsList.Add(label);
            optionsList.Add(minValue);
            optionsList.Add(maxValue);
            optionsList.Add(chartWidth);
            optionsList.Add(saveAsSVGOption);
            optionsList.Add(saveLocation);

            return optionsList;
        }
    }

    public class histogramPlot : Plot
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

                SVGContent += "<rect id='histogramBar" + i + "' x='" + x + "' y='" + (chartHei - y) + "' width='" + ((maxPosX - minPosX) / (histogram.Count)) + "' height='" + (y - chartMargin) + "' style='stroke-width:1px; stroke: rgba(200, 30, 30, 0.9); fill: rgba(200, 30, 30, 0.7); shape-rendering: crispEdges;' />   \n";
                SVGContent += "<text x='" + (x + ((maxPosX - minPosX) / (histogram.Count)) / 2) + "' y='" + (chartHei - y - 10) + "'  visibility='hidden' fill='#4192d9' font-family='sans-serif' font-size='12' text-anchor='middle'>" + histogram[i] + "<text/>";
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

    public class pieChartPlot : Plot
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
            var chartRad = chartWid / 2 - 40;
            var legendWid = 200;
            var legendHei = height;

            // the chart is 400px by 400px and the legend is 200px by 400px
            var SVGContent = "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='" + (chartWid + legendWid) + "px' height='" + chartHei + "px' viewBox='0 0 " + (chartWid + legendWid) + " " + chartHei + "' xml:space='preserve' >     \n";

            //draw graph background. The plot is always 500pt by 500pt
            SVGContent += "<rect x='0' y='0' width='" + chartWid + "' height='" + chartHei + "' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />\n";
            SVGContent += "<rect x='" + chartWid + "' y='" + 0 + "' width='" + legendWid + "' height='" + legendHei + "' style='fill:#FAFAFA; stroke-width:1px; stroke: #EEE' />\n";

            //calculate the % of each of the items
            List<double> percentages = new List<double>();
            double sum = Values.Sum();
            foreach (var value in Values)
            {
                percentages.Add((value * Math.PI * 2) / sum);
            }

            double lastPercentage = -1 * Math.PI / 2;
            string[] color = new string[]{"71d0f4", "e16452", "74c493", "f2d435", "8361a9",
                             "9abf88", "ef778c", "5698c4", "8c4646", "578b7e",
                             "cbc49d", "ef464c", "a8c542", "322f28", "9f3169",
                             "e99123", "878787", "6e8314", "2854a4", "555555"};

            var i = 0;
            var j = 0;

            //generate charts
            foreach (var percentage in percentages)
            {
                var fillColor = color[i++];
                if (i >= 20)
                {
                    i = 0;
                }

                var largeArcFlag = (percentage < Math.PI) ? 0 : 1;

                SVGContent += "<path id='pieChartSector" + i + "' d='M " + chartCenterXY + "," + chartCenterXY +
                    " l " + chartRad * Math.Cos(lastPercentage) + "," + chartRad * Math.Sin(lastPercentage) +
                    " A" + chartRad + "," + chartRad + " " + lastPercentage + " " + largeArcFlag + ",1 " + (chartCenterXY + chartRad * Math.Cos(lastPercentage + percentage)) + "," + (chartCenterXY + chartRad * Math.Sin(lastPercentage + percentage)) + " z' fill='#" + fillColor + "' stroke='rgba(0, 0, 0, 0.4)' stroke-width='0' />\n";

                SVGContent += "<text x='" + ((chartCenterXY + chartRad * Math.Cos(lastPercentage + percentage / 2))) + "' y='" + ((chartCenterXY + chartRad * Math.Sin(lastPercentage + percentage / 2))) + "' fill='#444444' visibility='hidden' font-family='sans-serif' font-size='12'>" + "  " + Items[j] + " (" + Values[j] + ")";
                SVGContent += "<set attributeName='visibility' from='hidden' to='visible' begin='pieChartSector" + i + ".mouseover' end='pieChartSector" + i + ".mouseout'/>";
                SVGContent += "</text>";
                lastPercentage += percentage;
                ++j;
            }

            var margin = 20;
            SVGContent += "<text x='" + (chartWid / 2) + "' y='" + 15 + "' fill='#444444' font-family='sans-serif' font-size='12' text-anchor='middle'>" + chartTitle + "</text>";

            i = 0;
            j = 0;
            //generate legend for the chart
            foreach (var item in Items)
            {
                if (i >= 20)
                {
                    i = 0;
                }

                var centage = Math.Round((percentages[i] / (Math.PI * 2)) * 100, 2);
                SVGContent += "<rect x='" + (chartWid + margin) + "' y='" + ((i * 25) + 50) + "' width='20' height='20' rx='10' ry='10' fill='#" + color[i] + "' />\n";
                SVGContent += "<text x='" + (chartWid + margin + 25) + "' y='" + ((i * 25) + 50 + 15) + "' fill='#444444' font-family='sans-serif' font-size='12'>" + "  " + centage + "%  " + item + " (" + Values[j] + ")</text>";
                i = i + 1;
                ++j;
            }

            //close the svg
            SVGContent += "</svg>";

            //export as a SVG in desktop
            var file = CreateNewSVGFile(exportLocation, "pieChart");
            file.WriteLine(SVGContent);
            file.Close();
        }

    }
}