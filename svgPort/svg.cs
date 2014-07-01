using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace svgPort
{
    public class svg
    {
        private static void CreateNewSVGFile(String filePath, String fileName)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + fileName + ".SVG");
        }

        private static void preSVGBody()
        {

        }

        private static void postSVGBody()
        {

        }

        public static void exportPathsAsSVG()
        {

        }
    }
}
