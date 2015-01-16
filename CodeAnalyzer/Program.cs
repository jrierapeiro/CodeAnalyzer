using System;
using System.Diagnostics;
using System.Timers;
using CodeAnalyzer.Reports.Reports;

namespace CodeAnalyzer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();


            if (args == null || (args.Length == 1 && args[0].Equals("help", StringComparison.InvariantCultureIgnoreCase)))
            {
                WriteHelp();
                return;
            }

            if (args.Length < 3)
            {
                Console.WriteLine("Invalid number of args");
                WriteHelp();
                return;
            }


            var solutionFile = args[0];
            var reportType = (ReportType) Enum.Parse(typeof (ReportType), args[1]);
            var outputPath = args[2];
            var writeOnlyCodeWithWarnings = false;

            if (args.Length >= 4)
            {
                writeOnlyCodeWithWarnings = Convert.ToBoolean(args[3]);
            }


            SolutionReport.AnalizeSolution(solutionFile, reportType, outputPath, writeOnlyCodeWithWarnings).GetAwaiter().GetResult();

            // Stop timing
            stopwatch.Stop();

            // Write result
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
        }

        private static void WriteHelp()
        {
            Console.WriteLine("Usage: .exe {solutionfile}  {reportype [html or txt]} {outputpath} {writeOnlyCodeWithWarnings [true or false]} ");
            Console.WriteLine("Example: CodeAnalyzer.exe \"C:\\folder\\solutionfile.sln\" Html \"c:\\temp\" true");
        }
    }
}