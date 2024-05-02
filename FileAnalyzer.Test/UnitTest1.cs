using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;
using System.Linq;
using FileAnalyzer;

namespace FileAnalyzer.Tests 
{
    [TestClass]
    public class FileAnalyzerTests
    {
        [TestMethod]
        public void TestTotalSalesByMonthAndAllProducts()
        {
            // arrange
            string inputData = "Product 1,01-01-2023,350.00\nProduct 2,02-01-2023,350.50\nProduct 3,03-01-2023,75.50\nProduct 4,03-01-2023,200.00";
            var stringReader = new StringReader(inputData);
            var stringWriter = new StringWriter();

            var analyzer = new FileAnalyzer.FileAnalyzer(stringReader, stringWriter);

            // act
            analyzer.ReadSalesData();
            analyzer.CalculateAndDisplayTotalSalesByMonth();
            analyzer.CalculateAndDisplayTotalSalesForAllProducts();

            // get the output
            var result = stringWriter.ToString();

            // assert
            var expectedOutput = "Total sales by month:\nMarch 2023: 375.00\nFebruary 2023: 350.50\nJanuary 2023: 350.00\n\n" +
                                 "Total sales for all products:\nProduct 2: 350.50\nProduct 1: 350.00\nProduct 4: 200.00\nProduct 3: 75.50\n";
            Assert.AreEqual(expectedOutput, result);
        }
    }
}
