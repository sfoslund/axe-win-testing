using Axe.Windows.Automation;
using Axe.Windows.Automation.Data;
using Humanizer;
using System.Diagnostics;
using System.Globalization;

namespace AxeWinTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var langCode = "fr";
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(langCode);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langCode);

            var process = StartTestExe();

            try
            {
                var scanner = CreateScanner(process.Id);

                var scanResults = GetScanResults(scanner);

                PrintScanResults(scanResults);

                PrintControlString();
            }
            finally
            {
                process.Kill();
            }
        }

        private static Process StartTestExe()
        {
            var exePath = "WildlifeManager.exe";
            var process = Process.Start(exePath);
            return process;
        }

        private static IScanner CreateScanner(int processId)
        {
            Console.WriteLine("Creating scanner...");
            var configBuilder = Config.Builder.ForProcessId(processId);
            configBuilder.WithOutputFileFormat(OutputFileFormat.None);
            var config = configBuilder.Build();
            var scanner = ScannerFactory.CreateScanner(config);
            return scanner;
        }

        private static ScanOutput GetScanResults(IScanner scanner)
        {
            Console.WriteLine("Scanning...");
            var scanResults = scanner.Scan(null);
            return scanResults;
        }

        private static void PrintScanResults(ScanOutput scanResults)
        {
            Console.WriteLine("Scan results:");
            Console.WriteLine();
            Console.WriteLine("===========================================");
            foreach (var scanResult in scanResults.WindowScanOutputs)
            {
                foreach (var error in scanResult.Errors)
                {
                    Console.WriteLine(error.Rule.Description);
                }
            }
            Console.WriteLine("===========================================");
            Console.WriteLine();
        }

        private static void PrintControlString()
        {
            Console.WriteLine("Control string: " + DateTime.UtcNow.Humanize(culture: CultureInfo.CurrentCulture));
        }
    }
}