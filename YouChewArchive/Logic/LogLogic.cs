using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive.Logic
{
    public static class LogLogic
    {
        public const string Href = "Href";
        public const string IFrame = "Iframe";
        public const string Image = "Image";
        public const string News = "News";
        public const string Time = "Time";
        public const string Error = "Error";
        private static bool LogEnabled  = true;

        public static void EnableLog()
        {
            LogEnabled = true;
        }

        public static void DisableLog()
        {
            LogEnabled = false;
        }

        private static string GetLogFile(string LogFile)
        {
            return $@"{Output.OutputDirectory}\Logs\{LogFile}.log";
        }

        private static Dictionary<string, StreamWriter> Streams = new Dictionary<string, StreamWriter>();
        private static Dictionary<string, Stopwatch> Stopwatches = new Dictionary<string, Stopwatch>();

        public static void ResetAll()
        {
            if(!LogEnabled)
            {
                return;
            }

            Reset(Href);
            Reset(IFrame);
            Reset(Image);
            Reset(News);
            Reset(Time);
            Reset(Error);
        }

        public static void Reset(string LogFile)
        {
            if (!LogEnabled)
            {
                return;
            }

            string log = GetLogFile(LogFile);

            if (File.Exists(log))
            {
                File.Delete(log);
            }

            FileInfo fi = new FileInfo(log);

            if(!Directory.Exists(fi.Directory.FullName))
            {
                Directory.CreateDirectory(fi.Directory.FullName);
            }

        }

        private static StreamWriter GetStream(string logFile)
        {
            StreamWriter sw;

            if(!Streams.TryGetValue(logFile, out sw))
            {
                sw = new StreamWriter(GetLogFile(logFile), true);

                Streams.Add(logFile, sw);
            }

            return sw;
        }

        private static bool HasBeenMoreThan(string logFile, int seconds)
        {
            Stopwatch sw;

            if(!Stopwatches.TryGetValue(logFile, out sw))
            {
                sw = new Stopwatch();
                sw.Start();

                Stopwatches.Add(logFile, sw);

                return false;
            }

            if(sw.ElapsedMilliseconds > seconds * 1000)
            {
                sw.Restart();

                return true;
            }

            return false;
        }

        public static void Log(string LogFile, string text)
        {
            if (!LogEnabled)
            {
                return;
            }

            StreamWriter sw = GetStream(LogFile);
            
            sw.WriteLine(text);

            if(HasBeenMoreThan(LogFile, 120))
            {
                sw.Flush();
            }
        }

        public static void Dispose()
        {
            foreach(var kvp in Streams)
            {
                if(kvp.Value != null)
                {
                    kvp.Value.Dispose();
                }
            }

            Streams = new Dictionary<string, StreamWriter>();
        }

        public static void SortAndDistinctAll()
        {
            if (!LogEnabled)
            {
                return;
            }

            SortAndDistinctLogFile(Href);
            SortAndDistinctLogFile(IFrame);
            SortAndDistinctLogFile(Image);
            SortAndDistinctLogFile(News);
            SortAndDistinctLogFile(Error);
        }

        public static void SortAndDistinctLogFile(string LogFile)
        {
            if (!LogEnabled)
            {
                return;
            }

            string log = GetLogFile(LogFile);

            if (File.Exists(log))
            {

                string contents;

                using (StreamReader sr = new StreamReader(log))
                {
                    contents = sr.ReadToEnd();
                }

                List<string> sortedAndOrderedList = (new HashSet<string>(contents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))).OrderBy(s => s).ToList();

                string sortedAndOrderedFile = GetLogFile($"{LogFile}_DistinctAndSorted");

                using (StreamWriter sw = new StreamWriter(sortedAndOrderedFile, false))
                {
                    foreach (string line in sortedAndOrderedList)
                    {
                        sw.WriteLine(line);
                    }
                }

            }

        }

    }
}
