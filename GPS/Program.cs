using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Autodesk.DesignScript.Geometry;
using System.Drawing;
using System.Net;
using System.IO;


namespace GPS
{
    public class GPSViewer
    {
        GPSViewer()
        {
        }

        /// <summary>
        ///     GPS Viewer that takes a GPX file (/path/name.gpx as string) and gives a bitmap image of the plot. Use Watch Image node to view the result.
        ///     Limitations: 1. Only the first track will be plotted 
        ///                  2. All the track segments will be joined as one.
        ///                  3. The track will be approximated to suit the http request requiements
        ///     WARNNG: The google maps allows only 25000 requests per day per application, please be considerate in number of requests you use.
        /// </summary>
        /// <param name="gpxFile">Use File Path node OR String /path/name.gpx</param>
        /// <param name="lineWidth">Thickness of the plotted track (1-9)</param>
        /// <param name="mapHeight">Height of the bitmap map (400 - 800)</param>
        /// <param name="mapWidth">Width of the bitmap map (400 - 800)</param>
        /// <search>GPS,view,plot,track,gpx</search>
        public static Bitmap plotTrack(string gpxFile, int mapWidth = 400, int mapHeight = 400, int lineWidth = 1)
        {
            if (mapWidth > 800)
                mapWidth = 800;
            else if (mapWidth < 400)
                mapWidth = 400;

            if (mapHeight > 800)
                mapHeight = 800;
            else if (mapHeight < 400)
                mapHeight = 400;

            if (lineWidth > 9)
                lineWidth = 9;
            else if (lineWidth < 1)
                lineWidth = 1;

            List<string> trackSegments = new List<string>();

            //open the xml doc
            XmlDocument doc = new XmlDocument();
            doc.Load(gpxFile);

            //namespace of the gps viewer
            string namespc = "http://www.topografix.com/GPX/1/1";
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("", namespc);

            //get all trks
            //only one track is going to be supported because of the limitation in url encoding
            XmlNodeList trksList = doc.GetElementsByTagName("trk");
            List<XmlNode> trks = new List<XmlNode>(trksList.Cast<XmlNode>());

            //foreach code for all tracks -- but we will take only the first track
            //foreach (var trk in trks)
            //{
            XmlNode trk = trks[0];
            List<XmlNode> trksegs = ProcessNodes(trk.ChildNodes, "trkseg");

            List<Autodesk.DesignScript.Geometry.Point> pts = new List<Autodesk.DesignScript.Geometry.Point>();

            //get aggregate of all the trkpoints and store it in pts -- note we just approximately join the tracksegments
            foreach (XmlNode trkseg in trksegs)
            {
                List<XmlNode> trkpts = ProcessNodes(trkseg.ChildNodes, "trkpt");

                foreach (XmlNode trkpt in trkpts)
                {
                    double tempLat = Double.Parse(trkpt.Attributes["lat"].Value);
                    double tempLng = Double.Parse(trkpt.Attributes["lon"].Value);
                    Autodesk.DesignScript.Geometry.Point m = Autodesk.DesignScript.Geometry.Point.ByCoordinates(tempLat, tempLng, 0);
                    pts.Add(m);
                }
            }

            bool status = true;
            string encodedString = "";
            int originalNumPts = pts.Count;
            int currentNumPts = originalNumPts;
            int iterationCount = 0;
            List<Autodesk.DesignScript.Geometry.Point> simplifiedPts = pts;

            do
            {
                //first encode all the raw coordinates 
                encodedString = Encode(simplifiedPts);

                //webrequests cant go beyond 2048 characters
                if (encodedString.Length > (2048 - 100))
                {
                    //need to reduce the number of points by 5%
                    currentNumPts = (int)(currentNumPts * (1 - 0.05 * ++iterationCount));

                    NurbsCurve crv = NurbsCurve.ByPoints(simplifiedPts);
                    simplifiedPts.Clear();
                    List<double> parameter = Enumerable.Range(0, currentNumPts).Select(i => 0 + (1 - 0) * ((double)i / (currentNumPts - 1))).ToList<double>();

                    foreach (double para in parameter)
                    {
                        simplifiedPts.Add(crv.PointAtParameter(para));
                    }

                    status = false;
                }
                else
                {
                    status = true;
                }
            } while (status == false);

            StringBuilder finalStr = new StringBuilder();
            finalStr.Append(String.Format("https://maps.googleapis.com/maps/api/staticmap?size={0}x{1}", mapWidth, mapHeight));
            finalStr.Append(String.Format("&path=color:0xFF0000FF%7Cweight:{0}%7Cenc:", lineWidth));
            finalStr.Append(encodedString);

            Uri uri = new Uri(finalStr.ToString(), UriKind.Absolute);

            WebRequest http = HttpWebRequest.Create(finalStr.ToString());
            http.ContentType = "text/plain";

            HttpWebResponse response = (HttpWebResponse)http.GetResponse();
            Stream stream = response.GetResponseStream();

            Bitmap bitmap = (Bitmap)Bitmap.FromStream(stream);
            return bitmap;
        }

        private static List<XmlNode> ProcessNodes(XmlNodeList nodes, string name)
        {
            List<XmlNode> list = new List<XmlNode>();

            foreach (XmlNode node in nodes)
            {
                if (node.Name == name)
                    list.Add(node);
            }
            return list;
        }

        private static string Encode(IEnumerable<Autodesk.DesignScript.Geometry.Point> points)
        {
            var str = new StringBuilder();

            var encodeDiff = (Action<int>)(diff =>
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;

                int rem = shifted;

                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));

                    rem >>= 5;
                }

                str.Append((char)(rem + 63));
            });

            int lastLat = 0;
            int lastLng = 0;

            foreach (Autodesk.DesignScript.Geometry.Point point in points)
            {
                int lat = (int)Math.Round(point.X * 1E5);
                int lng = (int)Math.Round(point.Y * 1E5);

                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);

                lastLat = lat;
                lastLng = lng;
            }
            return str.ToString();
        }

        public static void getElevationValues(string gpxFile)
        {
            List<double> elevationData = new List<double>();

            List<string> trackSegments = new List<string>();

            //open the xml doc
            XmlDocument doc = new XmlDocument();
            doc.Load(gpxFile);

            //namespace of the gps viewer
            string namespc = "http://www.topografix.com/GPX/1/1";
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("", namespc);

            //get all trks
            //only one track is going to be supported because of the limitation in url encoding
            XmlNodeList trksList = doc.GetElementsByTagName("trk");
            List<XmlNode> trks = new List<XmlNode>(trksList.Cast<XmlNode>());

            //foreach code for all tracks -- but we will take only the first track
            //foreach (var trk in trks)
            //{
            XmlNode trk = trks[0];
            List<XmlNode> trksegs = ProcessNodes(trk.ChildNodes, "trkseg");

            List<Autodesk.DesignScript.Geometry.Point> pts = new List<Autodesk.DesignScript.Geometry.Point>();

            //get aggregate of all the trkpoints and store it in pts -- note we just approximately join the tracksegments
            foreach (XmlNode trkseg in trksegs)
            {
                List<XmlNode> trkpts = ProcessNodes(trkseg.ChildNodes, "trkpt");

                foreach (XmlNode trkpt in trkpts)
                {
                    List<XmlNode> eleList = ProcessNodes(trkpt.ChildNodes, "ele");
                    foreach (XmlNode ele in eleList)
                    {
                        elevationData.Add(Double.Parse(ele.InnerText));
                    }
                }
            }

            //return elevationData;
        }

        public static List<double> getVelocityValues(string gpxFile)
        {
            List<double> velocity = new List<double>();

            List<string> trackSegments = new List<string>();
            List<DateTime> time = new List<DateTime>();
            //open the xml doc
            XmlDocument doc = new XmlDocument();
            doc.Load(gpxFile);

            //namespace of the gps viewer
            string namespc = "http://www.topografix.com/GPX/1/1";
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("", namespc);

            //get all trks
            //only one track is going to be supported because of the limitation in url encoding
            XmlNodeList trksList = doc.GetElementsByTagName("trk");
            List<XmlNode> trks = new List<XmlNode>(trksList.Cast<XmlNode>());

            //foreach code for all tracks -- but we will take only the first track
            //foreach (var trk in trks)
            //{
            XmlNode trk = trks[0];
            List<XmlNode> trksegs = ProcessNodes(trk.ChildNodes, "trkseg");

            List<Autodesk.DesignScript.Geometry.Point> pts = new List<Autodesk.DesignScript.Geometry.Point>();

            //get aggregate of all the trkpoints and store it in pts -- note we just approximately join the tracksegments
            foreach (XmlNode trkseg in trksegs)
            {
                List<XmlNode> trkpts = ProcessNodes(trkseg.ChildNodes, "trkpt");

                foreach (XmlNode trkpt in trkpts)
                {
                    double tempLat = Double.Parse(trkpt.Attributes["lat"].Value);
                    double tempLng = Double.Parse(trkpt.Attributes["lon"].Value);
                    Autodesk.DesignScript.Geometry.Point m = Autodesk.DesignScript.Geometry.Point.ByCoordinates(tempLat, tempLng, 0);
                    pts.Add(m);

                    List<XmlNode> eleList = ProcessNodes(trkpt.ChildNodes, "time");
                    foreach (XmlNode ele in eleList)
                    {
                        time.Add(DateTime.Parse(ele.InnerText));
                    }
                }
            }

            for (int i = 0; i < pts.Count - 1; ++i)
            {
                velocity.Add(calculateVelocity(pts[i].X, pts[i].Y, pts[i + 1].X, pts[i + 1].Y, time[i], time[i + 1]));
                Console.WriteLine(velocity[i]);
            }

            return velocity;
        }

        private static double calculateVelocity(double lat1, double long1, double lat2, double long2, DateTime start, DateTime end)
        {
            TimeSpan timeDiff = (end - start);

            double dlong = (long2 - long1) * Math.PI / 180;
            double dlat = (lat2 - lat1) * Math.PI / 180;

            // Haversine formula:
            double R = 6371;
            double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dlong / 2) * Math.Sin(dlong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;

            return d / (timeDiff.TotalSeconds);
        }
    }
}
