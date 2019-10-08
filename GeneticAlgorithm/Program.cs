using System;
using System.Diagnostics;
using System.Text;
using System.IO;
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
                // Start timer 
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                int[,] costs;

                //costs = new int[,]
                //{
                //    {90,  75,  75,  80  },
                //    {35,  85,  55,  65  },
                //    {125, 95,  90,  105 },
                //    {45,  110, 95,  115 }
                //};

                costs = new int[,]
                {
                    { 11, 40, 51, 21, 18, 21 },
                    { 45, 19, 30, 45, 44, 13 },
                    { 29, 30, 29, 46, 30, 17 },
                    { 29, 14, 31, 38, 21, 27 },
                    { 13, 25, 23, 44, 51, 23 }
                };

                costs = new int[,]
                {
                    { 155, 205, 245, 70, 210, 195, 45, 135, 60, 35, 85, 90, 180, 50, 210 },
                    { 55, 120, 250, 130, 160, 115, 200, 230, 235, 65, 235, 115, 125, 235, 190 },
                    { 120, 55, 170, 185, 45, 175, 235, 30, 230, 40, 100, 240, 35, 160, 235 },
                    { 95, 130, 25, 25, 230, 40, 220, 240, 225, 30, 245, 25, 95, 25, 205 },
                    { 170, 60, 150, 160, 210, 155, 30, 230, 100, 230, 225, 160, 95, 65, 205 }
                };

                // Initialize assignment problem
                Assignment asmt = new Assignment(costs);
                Schedule bestSchedule;
                asmt.Start();

                // Print problem description
                Console.WriteLine("====== Assignment Problem Description ======");
                Console.WriteLine("Costs matrix: ");
                Print2DArray(asmt.costs);
                Console.WriteLine("Number of workers: {0}", asmt.num_machines);
                Console.WriteLine("Number of jobs: {0}\n", asmt.num_jobs);

                //// Initialize csv writer
                //string filePath = @"..\..\..\out.csv";
                //File.Delete(filePath);
                //var csv = new StringBuilder();
                //var newLine = string.Format("Generation,Fitness");
                //csv.AppendLine(newLine);

                // Set stopping conditions
                int solution = 0;
                int repeated_generations = 0;
                int max_repeated_generations = 200; 
                int max_generations = 1000;

                // Update algorithm until stopping conditions are met
                while (
                    asmt.ga.Generation < max_generations && 
                    asmt.ga.BestFitness > solution && 
                    repeated_generations < max_repeated_generations || 
                    asmt.ga.BestFitness == 0
                    )
                {
                    // Prevent cursor flickering
                    Console.CursorVisible = false;

                    // Print current best genes
                    var sb = new StringBuilder();
                    sb.AppendLine("-------------------------------");
                    sb.AppendFormat("Current Generation: {0}        \n", asmt.ga.Generation);
                    sb.AppendFormat("Current Assignment: [ {0} ]\n", String.Join(", ", asmt.ga.BestGenes));
                    sb.AppendFormat("Current Best Fitness: {0}      \n", asmt.ga.BestFitness);
                    sb.AppendFormat("Population Size: {0}           \n", asmt.populationSize);
                    sb.AppendFormat("Mutation Rate: {0}             \n", asmt.mutationRate);
                    sb.AppendLine("-------------------------------");
                    Console.Write(sb);
                    Console.SetCursorPosition(0, Console.CursorTop - 7);

                    double prevBestFitness = asmt.ga.BestFitness;

                    // Update 
                    asmt.Update();

                    if (prevBestFitness == asmt.ga.BestFitness)
                    {
                        repeated_generations++;
                    }
                    else
                    {
                        repeated_generations = 0;
                    }

                    //// Append to csv
                    //newLine = string.Format("{0}, {1}", asmt.ga.Generation, asmt.ga.BestFitness);
                    //csv.AppendLine(newLine);
                }

                Console.CursorVisible = true;

                // Get the elapsed time as a TimeSpan value and format the TimeSpan value.
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

                bestSchedule = new Schedule(asmt.ga.BestGenes);

                // Print solution and run time
                Console.SetCursorPosition(0, Console.CursorTop + 8);
                Console.WriteLine("========= Result =========");
                Console.WriteLine("Generation: {0}", asmt.ga.Generation);
                Console.WriteLine("Assignment: [ {0} ]", String.Join(", ", asmt.ga.BestGenes));
                bestSchedule.PrintSchedule();
                Console.WriteLine("Best Fitness (cost): " + asmt.ga.BestFitness);
                Console.WriteLine("Best Population Size: " + asmt.ga.Population.Count);
                Console.WriteLine("RunTime: " + elapsedTime);
                Console.WriteLine("==========================\n");

                //// Save as csv
                //File.AppendAllText(filePath, csv.ToString());

                PrintColourMessage(ConsoleColor.Green, "SOLVED!!!");

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

        static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }


    }
}


