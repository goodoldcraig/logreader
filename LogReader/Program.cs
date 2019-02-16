using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    class Program
    {
        public static string[] LogHeaders;
        static void Main(string[] args)
        {            
            List<string> logs = ReadFiles(Properties.Settings.Default.FilePath);
            CreateCSV(ParseLogs(logs),Properties.Settings.Default.Destination);
        }

        private static void CreateCSV(List<string[]> logs,string destination)
        {
            try
            {
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }
                using (StreamWriter sw = new StreamWriter(string.Format($"{destination}\\logs.csv"),false))
                {
                    var csv = new CsvHelper.CsvWriter(sw);
                    foreach (var header in LogHeaders)
                    {
                        csv.WriteField(header);
                    }
                    csv.NextRecord();
                    foreach (var log in logs)
                    {
                        foreach (var field in log)
                        {
                            csv.WriteField(field);
                        }
                        csv.NextRecord();
                    }                    
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static List<string[]> ParseLogs(List<string> logs)
        {
            List<string[]> logSplit = new List<string[]>();
            try
            {
                foreach (string log in logs)
                {
                    logSplit.Add(log.Split(new string[] {Properties.Settings.Default.delimiter}, StringSplitOptions.None));
                }
                return logSplit;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return logSplit;
            }
        }

        private static List<string> ReadFiles(string filePath)
        {
            List<string> logs = new List<string>();
            try
            {                
                string[] files = Directory.GetFiles(filePath);
                string processedPath = string.Format($"{filePath}\\processed");
                if (!Directory.Exists(processedPath))
                {
                    Directory.CreateDirectory(processedPath);
                }
                foreach (string file in files)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {                        
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            if (line.StartsWith("#Fields"))
                            {
                                LogHeaders = line.Replace("#Fields: ", "").Split(new string[] { Properties.Settings.Default.delimiter }, StringSplitOptions.None);
                            }
                            if (!line.StartsWith("#Software") && !line.StartsWith("#Date") && !line.StartsWith("#LogTime") && !line.StartsWith("#Fields"))
                            {
                                logs.Add(line);
                            }
                        }
                    }                    
                    File.Move(file,string.Format($"{processedPath}\\{Path.GetFileName(file)}"));
                }
                return logs;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return logs;
            }
        }
    }
}
