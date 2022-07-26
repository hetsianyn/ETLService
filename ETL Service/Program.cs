using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
            string path = ConfigurationManager.AppSettings["path"];
            List<string> files = new List<string>();
            files.AddRange(System.IO.Directory.GetFiles(path, "*.txt"));
            files.AddRange(System.IO.Directory.GetFiles(path, "*.csv"));
            foreach (var item in files)
            {
                Console.WriteLine(item);
            }
            
            ReadCsv(files);
            
            // foreach (var person in personList)
            // {
            //     Console.WriteLine("Person: {0},{1},{2},{3},{4},{5},{6}", person.FirstName, person.LastName, person.Date, person.AccountNumber, person.Payment,
            //         person.City, person.Service);
            // }
            //
            // foreach (var city in cityList)
            // {
            //     Console.WriteLine("City: {0},{1}", city.Name, city.Total);
            // }
            //
            // foreach (var service in serviceList)
            // {
            //     Console.WriteLine("Service: {0},{1},{2}", service.Name, service.City, service.Payment);
            // }
            //
            // ComputeServiceTotal(serviceList);
            //
            // for (int i = 0; i < serviceTotal.Count; i++)
            // {
            //     Console.WriteLine("Key: {0}, Value: {1}", 
            //         serviceTotal.ElementAt(i).Key, 
            //         serviceTotal.ElementAt(i).Value);
            // }
            //
            
            
            
            
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
                var xxx = personList.Where(x => x.City == city).Select(x => x.Service).Distinct().ToList();
                foreach (var service in xxx)
                {
                    //Console.WriteLine(serviceList.Where(x => x.City == city && x.Name == service).Sum(x => x.Payment)); //Total in every service in every city
                   
                    Console.WriteLine(city + " " + service);
            
                }
                
                //Console.WriteLine("Total: " + serviceList.Where(x => x.City == city).Sum(x => x.Payment)); // Total by city
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
        
        

        public static void ReadCsv(List<string> files)
        {
            
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                foreach (var file in files)
                {

                    using (StreamReader sr = new StreamReader(file))
                    {
                        string line;
                        // Read and display lines from the file until the end of
                        // the file is reached.

                        if (file.Contains(".csv"))
                        {
                            //Skip header of csv
                            for (var i = 0; i < 1; i++)
                            {
                                sr.ReadLine();
                            }
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
                Decimal.Parse(transactionProps[6], CultureInfo.InvariantCulture),
                transactionProps[2],
                transactionProps[9]));

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
        
        
        public static void ToJson()
        {
            string path = @"json.json";
            string json = "[";
            
            foreach (var city in cities)
            {
                json += "{\n";
                json += "\t\"city\":\"" + city + "\",\n";
                json += "\t\"services\": [";
                
                foreach (var service in personList.Where(x => x.City == city).Select(x => x.Service).Distinct().ToList())
                { 
                    json += "\n\t\t{\"name\": \"" + service + "\",\n";
                    json += "\t\t\"payers\": [\n";
                    
                    var payers = personList.Where(x => x.City == city && x.Service == service).ToList();
                    foreach (var payer in payers)
                    {
                        json += "\t\t\t{\"name\": \"" + payer.FirstName + " " + payer.LastName + "\",\n";
                        json += "\t\t\t\"payment\": " + payer.Payment + ",\n";
                        json += "\t\t\t\"date\":  \"" + payer.Date + "\",\n";
                        json += "\t\t\t\"account_number\": " + payer.AccountNumber + "},\n";
                    }
                    json = json.Remove(json.Length - 2) + "],";
                    var serviceTotal = serviceList.Where(x => x.City == city && x.Name == service).Sum(x => x.Payment);
                    json +="\n\t\t\"total\": " + serviceTotal + "},";
                    
                }
                
                json = json.Remove(json.Length - 1) + "],";
                var cityTotal = serviceList.Where(x => x.City == city).Sum(x => x.Payment);
                json += "\n\t\"total\": " + cityTotal + "},\n";
            }
            
            json = json.Remove(json.Length - 2);
            json += "]";
            File.WriteAllText(path, json);
        }
    }

    
}