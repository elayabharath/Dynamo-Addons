using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {

            List<double> x = new List<double> { };
            List<double> y = new List<double> { };

            Random rand = new Random();
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Users\\t_elane\\Desktop\\file.txt");
            
            for (int i = 0; i < 100; ++i)
            {
                x.Add(rand.Next(0, 10));
                file.WriteLine(x[i]);
            }
            file.WriteLine("Histogram");
            List<int> hist = analysis.findHistogram(x, 5, 0, 10);
            foreach (int elements in hist)
            {
                file.Write(elements + ", ");
            }
            
            //double mean = analysis.findMean(x);
            //file.WriteLine("Mean: " + mean);

            //List<double> mode = analysis.findMode(x);
            //file.WriteLine("Mode:");
            //for (int i = 0; i < mode.Count; ++i)
            //    file.Write(" " + mode[i]);
            //file.WriteLine("\n");

            //double stdDev = analysis.findStdDeviation(x);
            //file.WriteLine("Std dev: " + stdDev);

            //double vari = analysis.findVariance(x);
            //file.WriteLine("Std Var: " + vari);

            //double median = analysis.findMedian(x);
            //file.WriteLine("Median: " + median);

            //double cor = analysis.sfindCorrelation(x, y);
            //file.WriteLine("Correlation: " + cor);

            //double pcor = analysis.findPearsonCorrelation(x, y);
            //file.WriteLine("Pearson: " + pcor);
            

            file.Close();

            //Console.ReadKey();

        }
    }
}
