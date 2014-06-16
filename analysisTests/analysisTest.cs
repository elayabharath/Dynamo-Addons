using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace dataAnalysisTests
{
    [TestClass]
    public class analysisTests
    {
        [TestMethod]
        public void findMeanTest1()
        {
            //arrange
            List<double> rawData1 = new List<double> { 0.0421, 0.0941, 0.1064, 0.0242, 0.1331, 0.0773, 0.0243, 0.0815, 0.1186, 0.0356, 0.0728, 0.0999, 0.0614, 0.0479 };
            List<double> rawData2 = new List<double> { 0.1081, 0.0986, 0.1566, 0.1961, 0.1125, 0.1942, 0.1079, 0.1021, 0.1583, 0.1673, 0.1675, 0.1856, 0.1688, 0.1512 };
            List<double> rawData3 = new List<double> { 0.0000, 0.0000, 0.0000, 3.000, 0.0000, 1.000, 1.000, 1.000, 0.000, 0.5000, 7.000, 5.000, 4.000, 0.123 };

            //act
            double result1 = analysis.findMean(rawData1);
            double result2 = analysis.findMean(rawData2);
            double result3 = analysis.findMean(rawData3);

            //assert
            Assert.AreEqual(0.0728, result1);
            Assert.AreEqual(0.1482, result2);
            Assert.AreEqual(1.61592857, result3);

        }
    }
}
