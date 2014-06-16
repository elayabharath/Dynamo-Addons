using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class analysis
{

    /* -------------------------------------------------------------------------
     * Compute the arithmetic mean of a dataset using the recurrence relation 
     * -------------------------------------------------------------------------*/
    public static double findMean(List<double> numList)
    {
        int count = 0;
        double mean = 0;

        foreach (double i in numList)
        {
            mean = mean + (i - mean) / (++count);
        }

        return mean;
    }


    /* -------------------------------------------------------------------------
    * Sort the given dataset in ascending order using quick sort algorithm
    * -------------------------------------------------------------------------*/
    static List<double> SortAscending(List<double> numList)
    {
        if (numList.Count <= 1)
            return numList;

        // find pivot and remove it
        double pivot = numList[numList.Count / 2];
        numList.RemoveAt(numList.Count / 2);

        // sort the list into smaller and bigger elements, equals go itno smaller number list
        List<double> smallList = new List<double> { };
        List<double> bigList = new List<double> { };
        int smallCount = 0; int bigCount = 0;

        foreach (double i in numList)
        {
            if (i <= pivot)
                smallList.Insert(smallCount++, i);
            else
                bigList.Insert(bigCount++, i);
        }

        // convert pivot into a list to be inserted
        List<double> pivotList = new List<double> { };
        pivotList.Add(pivot);

        // divide and conquer
        return SortAscending(smallList).Concat(pivotList).Concat(SortAscending(bigList)).ToList();
    }


    /* -------------------------------------------------------------------------
     * find mode for a given dataset 
     * -------------------------------------------------------------------------*/
    public static List<double> findMode(List<double> numList)
    {
        // sort the dataset to avoid using hashmap
        numList = SortAscending(numList);

        // final mode list numbers
        List<double> modeList = new List<double> { };
        int maxCount = 1;

        // temproary list that takes the current recurring value
        double currentList = numList[0];
        int currentCount = 1;

        for (int i = 1; i < numList.Count; ++i)
        {
            if (numList[i] == numList[i - 1])
            {
                // current element is being repeated
                ++currentCount;
            }

            else
            {
                // if current list count is greater, this becomes the mode list
                if (currentCount > maxCount)
                {
                    maxCount = currentCount;
                    modeList.Clear();

                    modeList.Add(currentList);
                }

                // if current list count is the same as mode, the mode list is appended
                else if (currentCount == maxCount)
                {
                    modeList.Add(currentList);
                }

                // refresh current list
                currentList = numList[i];
                currentCount = 1;
            }
        }

        return modeList;
    }

    /* -------------------------------------------------------------------------
     * find median for a given dataset 
     * -------------------------------------------------------------------------*/
    public static double findMedian(List<double> numList)
    {
        // sort the number list in ascending order
        numList = SortAscending(numList);
        
        // for even number of dataset, the mean of middle 2 numbers is the median
        if (numList.Count % 2 == 1)
        {
            return numList[numList.Count/2];
        }

        // for odd number of dataset, middle element is the median
        else
            return (numList[numList.Count/2] + numList[numList.Count/2 - 1])/2;
    }


    /* -------------------------------------------------------------------------
    * find kth smallest element using median of medians selection algorithm
    * -------------------------------------------------------------------------*/
    public static double findNthSmallest(List<double> numList, int nth)
    {

        if (numList.Count == 1)
            return numList.ElementAt(0);

        // Divide the array into groups of 5
        // find the median of each array
        List<double> median = new List<double> { };
        int count = 0;
        for(int i=0; i<numList.Count; i=i+5)
        {
            double[] groupOfFive;
            if (i + 4 < numList.Count)
            {
                groupOfFive = new double[5];
                numList.CopyTo(i, groupOfFive, 0, 5);
            }
            else
            {
                groupOfFive = new double[numList.Count - i];
                numList.CopyTo(i, groupOfFive, 0, numList.Count - i);
            }

            List<double> groupOfFiveSorted = SortAscending(groupOfFive.ToList());
            double medianTemp = groupOfFiveSorted[groupOfFiveSorted.Count / 2];

            median.Insert(count++, medianTemp); 
        }

        // select the pivot
        double pivot = findNthSmallest(median, median.Count / 2);

        // split into small and big arrays
        List<double> smallList = new List<double> { };
        List<double> bigList = new List<double> { };
        int pivotCount = 0;

        for (int i = 0; i < numList.Count; ++i)
        {
            if (numList[i].CompareTo(pivot) < 0)
                smallList.Add(numList[i]);
            else if (numList[i].CompareTo(pivot) > 0)
                bigList.Add(numList[i]);
            else 
                ++pivotCount;
        }

        //add extra pivots into small list
        for (int i = 1; i < pivotCount; ++i)
            smallList.Add(pivot);

        // find K
        int k = smallList.Count + 1;
        
        // apply conditions for K
        if (k == nth)
            return pivot;
        else if (nth < k)
            return findNthSmallest(smallList, nth);
        else
            return findNthSmallest(bigList, nth - k);

    }

    /* -------------------------------------------------------------------------
     * find standard variabce using online algorithm by Knuth
     * http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Online_algorithm
     * -------------------------------------------------------------------------*/
    public static double findVariance(List<double> numList)
    {
        int n = 0;
        double mean = 0;
        double M2 = 0;
        double delta = 0;

        foreach(double x in numList)
        {
            n = n + 1;
            delta = x - mean;
            mean = mean + delta / n;
            M2 = M2 + delta * (x - mean);
        }

        return M2/(n-1);
    }

    /* -------------------------------------------------------------------------
    * find standard deviation for a given dataset
    * -------------------------------------------------------------------------*/
    public static double findStdDeviation(List<double> numList)
    {
        // standard deviation is the square root of variance
        return Math.Sqrt(findVariance(numList));
    }

    /* -------------------------------------------------------------------------
    * find pearson correlation
    * http://en.wikipedia.org/wiki/Pearson_product-moment_correlation_coefficient
    * -------------------------------------------------------------------------*/
    public static double findPearsonCorrelation(List<double> numListX, List<double> numListY)
    {
        int n = numListX.Count;

        // find Means
        double xMean = findMean(numListX);
        double yMean = findMean(numListY);

        // find sX^2 and sY^2
        double sX2 = 0;
        double sY2 = 0;

        // this form is used so that the resulting values are always numerically stable
        for (int i = 0; i < n; ++i)
        {
            sX2 = sX2 + (Math.Pow((numListX[i] - xMean), 2)) / (n - 1);
            sY2 = sY2 + (Math.Pow((numListY[i] - yMean), 2)) / (n - 1);
        }

        // find sX and sY
        double summation=0;
        double sX = Math.Sqrt(sX2);
        double sY = Math.Sqrt(sY2);
        
        // this form is used so that the resulting values are always numerically stable
        for (int i = 0; i < n; ++i)
        {
            summation = summation + ((numListX[i] - xMean) / sX * (numListY[i] - yMean) / sY)/(n-1);
        }
        
        return summation;
    }


    public static double findCorrelation(List<double> x, List<double> y)
    {
        double xBar = 0;
        double yBar = 0;
        int n=x.Count;

        for (int i = 0; i < n; ++i)
        {
            xBar = xBar + x[i];
            yBar = yBar + y[i];
        }

        xBar = xBar / n;
        yBar = yBar / n;

        double sX2 = 0;
        double sY2 = 0;
        

        for (int i = 0; i < n; ++i)
        {
            sX2 = sX2 + (x[i] - xBar)*(x[i] - xBar);
            sY2 = sY2 + (y[i] - yBar)*(y[i] - yBar);
        }

        sX2 = sX2 / (n - 1);
        sY2 = sY2 / (n - 1);

        double sX = Math.Sqrt(sX2);
        double sY = Math.Sqrt(sY2);

        double rel = 0;
        for (int i = 0; i < n; ++i)
        {
            rel = rel + ((x[i] - xBar) / sX) * ((y[i] - yBar) / sY);
        }

        return rel / (n - 1);

    }


    public static double sfindCorrelation(List<double> numListX, List<double> numListY)
    {
        //caluclate number count
        int n = numListX.Count;

        double sumX = 0, sumY = 0, sumXX = 0, sumXY = 0, sumYY = 0;
        
        List<double> XY = new List<double> { };
        List<double> XX = new List<double> { };
        List<double> YY = new List<double> { };

        for (int i = 0; i < n; ++i)
        {
            XY.Add(numListX[i] * numListY[i]);
            XX.Add(numListX[i] * numListX[i]);
            YY.Add(numListY[i] * numListY[i]);

            sumX = sumX + numListX[i];
            sumY = sumY + numListY[i];

            sumXX = sumXX + XX[i];
            sumYY = sumYY + YY[i];
            sumXY = sumXY + XY[i];
        }

        return (n * sumXY - sumX * sumY) / Math.Sqrt((n * sumXX - sumX * sumX) * (n * sumYY - sumY * sumY));
       
    }


    /* -------------------------------------------------------------------------
    * find histogram frequency table for a given dataset
    * -------------------------------------------------------------------------*/
    public static List<int> findHistogram (List<double> numList, int numIntervals, double min, double max)
    {
        // calculate the interval
        double interval = (max - min)/numIntervals;

        // create a list of of histogram based on the number of intervals
        List<int> histogram = new List<int>(new int[numIntervals]);
        int intervalNumber = 0;

        // take each element and pass it to the respective interval bin
        foreach (double element in numList)
        {
            intervalNumber = Convert.ToInt32(Math.Floor(element / interval));

            //when the bin is found, the histogram interval count is increased
            histogram[intervalNumber] += 1;
        }

        return histogram;
    }
}


