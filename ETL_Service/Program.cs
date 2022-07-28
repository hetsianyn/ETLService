using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ETL_Service
{
    internal class Program
    {
        static List<Transaction> personList = new List<Transaction>();
        static List<City> cityList = new List<City>();
        static List<Service> serviceList = new List<Service>();
        private static List<string> cities = new List<string>();
        private static int errorCount = 0;
        private static List<string> invalidFiles = new List<string>();
        static int linesCount = 0;
        

        public static void Main(string[] args)
        {
            Console.WriteLine("Input \"start\" command to start ETL_Service:");
            string startCommand = Console.ReadLine();
            if (startCommand != "start")
            {
                Console.WriteLine("Unrecognized command\n");
                Environment.Exit(0);
            }
            Console.WriteLine("ETL_Service started execution.");
            
            string input = ConfigurationManager.AppSettings["inputPath"];
            string output = ConfigurationManager.AppSettings["outputPath"];
            
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(input, "*.txt"));
            files.AddRange(Directory.GetFiles(input, "*.csv"));
            int fileNum = 1;
            
            foreach (var file in files)
            {
                
                //Console.WriteLine(file);
                if (new FileInfo(file).Length == 0)
                {
                    Console.WriteLine("File: " + file + " is empty");
                    continue;
                }
                
                linesCount = ReadFile(file);
                cities = cityList.Select(x => x.Name).Distinct().ToList();
                
                string outputDir = output + "\\" + DateTime.Now.ToString("dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                
                ToJson(fileNum, outputDir);
                fileNum++;
                
                CreateLogFile(outputDir, fileNum-1, linesCount, errorCount, invalidFiles);
            }

            Console.WriteLine("ETL_Service finished execution.");
            Console.ReadLine();
        }
        

        public static int ReadFile(string file)
        {

            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                
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

                            linesCount++;
                        }
                    }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file " + file + " Line " + linesCount +
                                  " \ncould not be read or some rows are invalid: " + e.Message);
                errorCount++;
                invalidFiles.Add(file);
            }

            return linesCount;
        }

        

        public static bool ExtractPerson(List<string> transactionProps)
        {
            personList.Add(new Transaction(transactionProps[0],
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
            cityList.Add(new City(transactionProps[2]));

            return true;
        }
        
        public static bool ExtractService(List<string> transactionProps)
        {
            serviceList.Add(new Service(transactionProps[9],
                transactionProps[2],
                Decimal.Parse(transactionProps[6], CultureInfo.InvariantCulture)));

            return true;
        }
        
        
        public static void ToJson(int fileNum, string outputDir)
        {
            string path = outputDir + "\\output" + fileNum + ".json";
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
        
        public static void CreateLogFile(string outputDir, int fileNum, int linesCount, int errorCount, List<string> invalidFiles)
        {
            
            string log = "parsed_files: " + fileNum + "\n";
            log += "parsed_lines: " + linesCount + "\n";
            log += "found_errors: " + errorCount + "\n";
            log += "invalid_files: [";
            foreach (var file in invalidFiles)
            {
                log += file + ",\n"; 
            }
            log = log.Remove(log.Length - 2);
            log += "]";
            
            File.WriteAllText(outputDir + "\\meta.log", log);
        }
    }

    
}