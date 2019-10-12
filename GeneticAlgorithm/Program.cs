using System;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {

            GetAppInfo(); // Run GetAppInfo function to get info

            while (true)
            {
                // Initialize solver
                Controller controller = new Controller();
                controller.Start();

                PrintColourMessage(ConsoleColor.Green, "DONE!!!");

                // Ask to run again
                PrintColourMessage(ConsoleColor.Yellow, "Run again? [Y or N]");

                // Get answer
                string answer = Console.ReadLine().ToUpper();
                if (answer == "Y")
                {
                    continue;
                }
                else if (answer == "N")
                {
                    return;
                }
                else
                {
                    return;
                }                
            }
        }

        static void GetAppInfo()
        {
            // Set app vars
            string appName = "Generalized Genetic Algorithm Assignment Problem Console App Implementation";
            string appVersion = "1.0.0";
            string appAuthor = "Jakkarin Sae-Tiew";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0}: Version {1} by {2}\n", appName, appVersion, appAuthor);
            Console.ResetColor();
        }

        // Print color message
        static void PrintColourMessage(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}


