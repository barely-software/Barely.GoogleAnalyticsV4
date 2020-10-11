using Barely.GoogleAnalyticsV4;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoogleAnalyticsCLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tid = string.Empty;

            while (string.IsNullOrEmpty(tid))
            {
                Console.Write("Please enter a valid google analytics tracking ID (UA-XXXXX-Y): ");
                tid = Console.ReadLine();

                if (!IsValidTid(tid))
                {
                    Console.WriteLine($"{tid} is invalid.");
                    tid = string.Empty;
                }
            }

            var analytics = new GoogleAnalyticsClient(tid);

            while (!String.Equals(Console.ReadLine(), "q"))
                await analytics.Send("/foo");
        }

        private static Regex _validTid = new Regex(@"\bUA-\d{4,10}-\d{1,4}\b", RegexOptions.Compiled);
        public static bool IsValidTid(string tid)
        {
            return _validTid.IsMatch(tid);
        }
    }
}