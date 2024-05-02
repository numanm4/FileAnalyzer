using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FileAnalyzer
{
    public class SalesRecord
    {
        public string ProductName { get; set; }
        public DateTime DateOfSale { get; set; }
        public double SalesAmount { get; set; }
    }

    public class FileAnalyzer
    {
        private List<SalesRecord> salesRecords;

        public FileAnalyzer()
        {
            salesRecords = new List<SalesRecord>();
        }

        public void ReadSalesData(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Error: File '{filePath}' was not found.");
                    return;
                }

                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] data = line.Split(',');
                    if (data.Length == 3)
                    {
                        SalesRecord record = new SalesRecord
                        {
                            ProductName = data[0].Trim(),
                            DateOfSale = DateTime.ParseExact(data[1].Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                            SalesAmount = double.Parse(data[2].Trim(), CultureInfo.InvariantCulture)
                        };
                        salesRecords.Add(record);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading file: {e.Message}");
                throw e;
            }
        }

        public void CalculateAndDisplayTotalSalesByMonth()
        {
            var salesPerMonth = salesRecords
                .GroupBy(record => record.DateOfSale.ToString("MMMM yyyy"))
                .Select(group => new
                {
                    MonthYear = group.Key,
                    TotalSales = group.Sum(record => record.SalesAmount)
                })
                .OrderByDescending(sale => sale.TotalSales);

            DisplaySalesByMonth(salesPerMonth);
        }

        private void DisplaySalesByMonth(IEnumerable<dynamic> salesPerMonth)
        {
            Console.WriteLine("\nTotal sales by month:");
            foreach (var sale in salesPerMonth)
            {
                Console.WriteLine($"{sale.MonthYear}: {sale.TotalSales:F2}");
            }
        }

        public void CalculateAndDisplayTotalSalesForFilteredProducts(string[] filterStrings)
        {
            var filteredSales = salesRecords
                .Where(record => filterStrings.Any(filter => record.ProductName.Equals(filter, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            CalculateAndDisplayTotalSales(filteredSales, "\nTotal sales for filtered products:");
        }

        public void CalculateAndDisplayTotalSalesForAllProducts()
        {
            CalculateAndDisplayTotalSales(salesRecords, "\nTotal sales for all products:");
        }

        private void CalculateAndDisplayTotalSales(IEnumerable<SalesRecord> records, string header)
        {
            var salesData = records
                .GroupBy(record => record.ProductName)
                .Select(group => new
                {
                    ProductName = group.Key,
                    TotalSales = group.Sum(record => record.SalesAmount)
                })
                .OrderByDescending(sale => sale.TotalSales);

            Console.WriteLine(header);
            foreach (var sale in salesData)
            {
                Console.WriteLine($"{sale.ProductName}: {sale.TotalSales:F2}");
            }
        }

        public void WriteModifiedSalesData(string filePath)
        {
            string outputPath = filePath;

            try
            {
                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    foreach (var record in salesRecords)
                    {
                        writer.WriteLine($"{record.ProductName}, {record.DateOfSale:MM-dd-yyyy}, {record.SalesAmount:F2}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
                throw ex;
            }
        }

        static void Main(string[] args)
        {
            FileAnalyzer analyzer = new FileAnalyzer();

            string inputFilePath, outputFilePath;

            Console.Write("Enter input file path: ");
            inputFilePath = Console.ReadLine();

            Console.Write("Enter output file path: ");
            outputFilePath = Console.ReadLine();


            analyzer.ReadSalesData(inputFilePath);


            analyzer.CalculateAndDisplayTotalSalesByMonth();


            analyzer.CalculateAndDisplayTotalSalesForAllProducts();


            Console.Write("\nEnter search string(s) for filtered products (comma-separated & no spaces b/w products): ");
            string searchInput = Console.ReadLine();
            string[] filterStrings = searchInput.Split(',');


            analyzer.CalculateAndDisplayTotalSalesForFilteredProducts(filterStrings);

            analyzer.WriteModifiedSalesData(outputFilePath);

            Console.WriteLine("\nAnalysis completed. Modified sales data was successfully written to the output file.");
        }
    }
}