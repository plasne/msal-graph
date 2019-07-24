using System;

namespace msal_graph
{
    class Program
    {

        static void Main(string[] args)
        {

            // start the graph
            Graph graph = new Graph();
            graph.Start();

            // query every 30 seconds
            var timer = new System.Timers.Timer(30000);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += async (Object source, System.Timers.ElapsedEventArgs e) =>
            {
                await graph.Poll();
            };

            // wait for user input
            Console.WriteLine("\nPress the Enter key to exit the application...\n");
            Console.ReadLine();

        }
    }
}
