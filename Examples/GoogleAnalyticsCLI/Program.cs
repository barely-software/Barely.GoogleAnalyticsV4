using Barely.GoogleAnalyticsV4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAnalyticsCLI
{
    class Program
    {
        static IList<(string commandId, string longCommand, string description, Func<Task> method)> Commands = new List<(string, string, string, Func<Task>)>
        {
            ("p", "pageview", "Send a pageview.", RunPageview),
            ("e", "event", "Send an event.", RunEvent),
            ("q", "quit", "Quit the application.", null),
            ("?", "help", "Print this help.", PrintHelp),
        };

        static GoogleAnalytics googleAnalytics = null;

        static async Task Main(string[] args)
        {
            string tid = ReadTrackingId();
            string cid = ReadClientId();

            googleAnalytics = new GoogleAnalytics(tid, cid);

            Console.WriteLine("\nGoogle analytics client initialized: ");
            Console.WriteLine($"  tid: {tid}");
            Console.WriteLine($"  cid: {cid}");
            Console.WriteLine("You are ready to rock & roll!\n");

            await RunUserCommands();
        }

        private static string ReadTrackingId()
        {
            Console.Write("Please enter a valid google analytics tracking ID (UA-XXXXX-Y): ");
            var tid = Console.ReadLine();

            while (!GoogleAnalytics.IsValidTid(tid))
            {
                Console.Write($"That's invalid. Please enter a valid tracking id: ");
                tid = Console.ReadLine();
            }

            return tid;
        }

        static string ReadClientId()
        {
            Console.Write("Please provide a unique client id (or press <enter> and we'll generate one for you): ");
            var cid = Console.ReadLine();

            if (string.IsNullOrEmpty(cid))
            {
                cid = Guid.NewGuid().ToString();
            }

            return cid;
        }
        private async static Task PrintHelp()
        {
            Console.WriteLine("\nSupported Commands: ");

            foreach (var c in Commands)
            {
                Console.WriteLine($"  {c.commandId} or {c.longCommand}: {c.description}");
            }

            Console.WriteLine("\nFor detailed documentation on the Google Analytics Measurement Protocol, see: ");
            Console.WriteLine("  https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide \n");
        }

        private static async Task RunUserCommands()
        {
            while (true)
            {
                Console.Write("Please enter a command (? for help): ");
                var userCommand = Console.ReadLine();

                if (string.IsNullOrEmpty(userCommand))
                    continue;

                if (string.Equals("q", userCommand))
                    break;

                var command = Commands.FirstOrDefault(_ => string.Equals(_.commandId, userCommand));
                if (command == default((string, string, string, Func<Task>)))
                {
                    Console.WriteLine($"Command not found: {userCommand}");
                    await PrintHelp();
                    continue;
                }

                if (command.method != null)
                    await command.method.Invoke();
            }
        }

        private async static Task RunPageview()
        {
            Console.Write("Please enter a page (e.g. /home): ");
            var dp = Console.ReadLine();

            Console.Write("Please enter a hostname (e.g. example.com): ");
            var dh = Console.ReadLine();

            Console.Write("Please enter a title (e.g. Home View): ");
            var dt = Console.ReadLine();

            try
            {
                await googleAnalytics.SendPageview(dp, dh, dt);
            }
            catch (Exception e)
            {
                Console.WriteLine($"  *** Failed to send page view: {e.Message} ***");
            }
        }

        private async static Task RunEvent()
        {
            Console.Write("Please enter an event category (e.g. video): ");
            var ec = Console.ReadLine();

            Console.Write("Please enter an event action (e.g. play): ");
            var ea = Console.ReadLine();

            Console.Write("Please enter an event label (e.g. holiday-promo): ");
            var el = Console.ReadLine();

            Console.Write("Please enter an event value (e.g. 300): ");
            var ev = Console.ReadLine();

            try
            {
                await googleAnalytics.SendEvent(ec, ea, el, ev);
            }
            catch (Exception e)
            {
                Console.WriteLine($"  *** Failed to send event: {e.Message} ***");
            }
        }


    }
}