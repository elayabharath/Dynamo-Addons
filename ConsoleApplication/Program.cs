using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Collections;
using svgPort;


namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            SVG.exportPathsAsSVG(null, "", "");
            

            Console.ReadKey();
        }


    }
}
