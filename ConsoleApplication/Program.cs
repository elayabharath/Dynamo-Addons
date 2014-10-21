using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Collections;
using Autodesk.DesignScript.Geometry;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {

            var exportLoc = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            Console.WriteLine(exportLoc);

            Console.ReadKey();
        }


    }
}
