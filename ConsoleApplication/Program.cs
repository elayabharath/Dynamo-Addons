using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using svgPort;
using System.Collections;

namespace Application
{
    class Program
    {

        public static void foo(IList numbers)
        {
            foreach (var num in numbers)
            {
                if (num is IList)
                { }
                Console.WriteLine("number: " + num);
            }
        }

        static void Main(string[] args)
        {

            int[] num1 = { 1, 2, 3, 4, 5 };
            int[][] arr = new int[2][];

            // Initialize the elements:
            arr[0] = new int[5] { 1, 3, 5, 7, 9 };
            arr[1] = new int[4] { 2, 4, 6, 8 };

            int [,] arr1 = {{1, 2, 3}, {4, 5, 6}};
            int[, ,] arr2 = { {{1, 2, 3}, {4, 5,6}}, {{7, 8, 9}, {10, 11, 12}}};
            //foo(arr2);
            //foo(arr);
            //foo(num1);
            System.Console.ReadKey();
        }
    }
}
