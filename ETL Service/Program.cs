using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
// using CsvHelper;
// using CsvHelper.Configuration;
// using System.Globalization;

namespace ETL_Service
{
    internal class Program
    {
        static List<Person> personList = new List<Person>();
        static List<City> cityList = new List<City>();
        static List<Service> serviceList = new List<Service>();
        private static List<string> services = new List<string>();
        private static List<string> cities = new List<string>();

        static IDictionary<string, decimal> serviceTotal = new Dictionary<string, decimal>();
        
        public static void Main(string[] args)
        {
            string filePath = ConfigurationManager.AppSettings["path"] + "\\sample.csv";
            Console.WriteLine(filePath);
            
            ReadCsv(filePath);
            
            foreach (var person in personList)
            {
                Console.WriteLine("Person: {0},{1},{2},{3},{4}", person.FirstName, person.LastName, person.Date, person.AccountNumber, person.Payment);
            }
            
            foreach (var city in cityList)
            {
                Console.WriteLine("City: {0},{1}", city.Name, city.Total);
            }
            
            foreach (var service in serviceList)
            {
                Console.WriteLine("Service: {0},{1},{2}", service.Name, service.City, service.Payment);
            }

            ComputeServiceTotal(serviceList);
            
            for (int i = 0; i < serviceTotal.Count; i++)
            {
                Console.WriteLine("Key: {0}, Value: {1}", 
                    serviceTotal.ElementAt(i).Key, 
                    serviceTotal.ElementAt(i).Value);
            }
            
            
            
            

            services = serviceList.Select(x => x.Name).Distinct().ToList();
            foreach (var item in services)
            {
               Console.WriteLine(item);
            }
            
            cities = cityList.Select(x => x.Name).Distinct().ToList();
            foreach (var item in cities)
            {
                Console.WriteLine(item);
            }

            foreach (var city in cities)
            {
                foreach (var service in services)
                {
                    Console.WriteLine(serviceList.Where(x => x.City == city && x.Name == service).Sum(x => x.Payment)); //Total in every service in every city
                }
                
                Console.WriteLine("Total: " + serviceList.Where(x => x.City == city).Sum(x => x.Payment)); // Total by city
            }

            ToJson();

        }

        public static IDictionary<string, decimal> ComputeServiceTotal(List<Service> servicesList)
        {
            foreach (var service in serviceList)
            {
                if (serviceTotal.ContainsKey(service.City + service.Name))
                {
                    serviceTotal[service.City + service.Name] += service.Payment;
                }
                else
                { 
                    serviceTotal.Add(service.City + service.Name, service.Payment);  
                }
            }
            
            return serviceTotal;
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
                        
                        var transactionProperties = line.Split(charsToSplit, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < transactionProperties.Length; i++)
                        {
                            transactionProperties[i] = transactionProperties[i].Trim(charsToTrim);
                        }
                        List<string> transactionProps = new List<string>(transactionProperties);
                        
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

        public static bool ExtractPerson(List<string> transactionProps)
        {
            personList.Add(new Person(transactionProps[0],
                transactionProps[1],
                long.Parse(transactionProps[8]), 
                DateTime.ParseExact(transactionProps[7], "yyyy-dd-MM", null),
                Decimal.Parse(transactionProps[6], CultureInfo.InvariantCulture)));

            return true;
        }
        
        public static bool ExtractCity(List<string> transactionProps)
        {
            cityList.Add(new City(transactionProps[2],
                Decimal.Parse(transactionProps[6], CultureInfo.InvariantCulture)));

            return true;
        }
        
        public static bool ExtractService(List<string> transactionProps)
        {
            serviceList.Add(new Service(transactionProps[9],
                transactionProps[2],
                Decimal.Parse(transactionProps[6], CultureInfo.InvariantCulture)));

            return true;
        }



        // public static void GetConfigurationValue()
        // {
        //     var title = ConfigurationManager.AppSettings["title"];
        //     var path = ConfigurationManager.AppSettings["path"];
        //     Console.WriteLine("'{0}' is running in '{1}' file.", title, path);
        // }


        public static void ToJson()
        {
            string path = @"file.json";
            string json = "[\n";
            
            foreach (var city in cities)
            {
                json = json + "{\n";
                json = json + "\"city\":" + city + "\",\n";
                json = json + "\"services\": [{";
                
                foreach (var service in services )
                { 
                    json = json + "\"name\": \"" + service + "\",\n";;
                }
                
                json = json.Remove(json.Length - 1);
                json = json + "}\n,";
            }
            
            json = json.Remove(json.Length - 1);
            json = json + "]";
            File.WriteAllText(path, json);
        }

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