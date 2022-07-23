using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
// using CsvHelper;
// using CsvHelper.Configuration;
// using System.Globalization;

namespace ETL_Service
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {
            string filePath = ConfigurationManager.AppSettings["path"] + "\\sample.csv";
            Console.WriteLine(filePath);
            
            ReadCsv(filePath);
            
        }
        

        public static void ReadCsv(string filePath)
        {
            
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    // Read and display lines from the file until the end of
                    // the file is reached.

                    //Skip header of csv
                    for(var i = 0; i < 1; i++) {
                        sr.ReadLine();
                    }
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        char[] charsToSplit = {' '};
                        char[] charsToTrim = {'"', ','};
                        
                        //Console.WriteLine(line);
                        
                        var transactionProps = line.Split(charsToSplit, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < transactionProps.Length; i++)
                        {
                            transactionProps[i] = transactionProps[i].Trim(charsToTrim);
                        }
                        
                        ExtractPerson(transactionProps);
                        ExtractCity(transactionProps);
                        ExtractService(transactionProps);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read or invalid:");
                Console.WriteLine(e.Message);
            }
            
        }

        public static List<object> ExtractPerson(string[] transactionProps)
        {
            List<object> personsData  = new List<object>();
            
            Person person = new Person();
            
            person.FirstName = transactionProps[0];
            person.LastName = transactionProps[1];
            person.AccountNumber = long.Parse(transactionProps[8]);
            person.Date = DateTime.ParseExact(transactionProps[7], "yyyy-dd-MM", null);
            person.Payment = Decimal.Parse(transactionProps[6], CultureInfo.InvariantCulture);
            
            personsData.Add(person);
            
            // Console.WriteLine(person.FirstName + " " + person.LastName + " " + person.AccountNumber + " " + person.Date + " " + person.Payment);

            return personsData;
        }
        
        public static List<object> ExtractCity(string[] transactionProps)
        {
            List<object> citiesData  = new List<object>();
            City city = new City();

            city.Name = transactionProps[2];
            citiesData.Add(city);

            return citiesData;
        }
        
        public static List<object> ExtractService(string[] transactionProps)
        {
            List<object> serviceData  = new List<object>();
            Service service = new Service();

            service.Name = transactionProps[9];
            serviceData.Add(service);
            
            return serviceData;
        }

        
        
        // public static List<string[]> PLINQAll(string filePath)
        // {
        //     var sw = new Stopwatch();
        //     sw.Start();
        //
        //     var results = System.IO.File.ReadAllLines(filePath)
        //         .AsParallel()
        //         .Select(line => Regex.Split(line, ","))
        //         .ToList();
        //
        //     sw.Stop();
        //     Console.WriteLine($"PLINQ using all cores: completed in {Math.Round(sw.Elapsed.TotalSeconds)} seconds");
        //
        //     return results;
        // }


        // public static void GetConfigurationValue()
        // {
        //     var title = ConfigurationManager.AppSettings["title"];
        //     var path = ConfigurationManager.AppSettings["path"];
        //     Console.WriteLine("'{0}' is running in '{1}' file.", title, path);
        // }

        // public static void FileToJson(string path)
        // {
        //     int counter = 1;
        //     string[] strheader = new string[100];
        //     string json = "[\n";
        //     
        //     foreach (string line in File.ReadLines(path))
        //     {
        //         if (counter == 1)
        //         {
        //             strheader = line.Split(',');
        //         }
        //         else
        //         {
        //             json = json + "{";
        //             string[] values = line.Split(',');
        //             int i = 0;
        //             foreach (string value in values)
        //             {
        //                 json = json + "\"" + strheader[i] + "\":\"" + value + "\",";
        //                 i = i + 1;
        //             }
        //             json = json.Remove(json.Length - 1);
        //             json = json + "}\n,";
        //         }
        //
        //         counter++;
        //
        //     }
        //     json = json.Remove(json.Length - 1);
        //     json = json + "]";
        //     File.WriteAllText("filejson.json", json);
        //
        //
        // }
        //
        // public static void WriteToJson()
        // {
        //     string json = "[\n";
        //
        // }
        
        
    }

    
}