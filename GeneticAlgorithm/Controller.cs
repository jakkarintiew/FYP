using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace GeneticAlgorithm
{
    public class Controller
    {
        private GA ga;
        private Printer printer;
        private Writer csvWriter;

        // Constructor
        public Controller()
        {
        }

        public void Start()
        {
            // Start timer 
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Initialize ga instance
            ga = new GA();
            printer = new Printer();

            //csvWriter = new Writer(@"..\..\..\out.csv");

            printer.PrintProblemDescription();

            // Update algorithm until stopping conditions are met:
            // Condition 1: if total number of generation reaches a threshold
            // Condition 2: if number of generation where the fitness remained constant reaches a threshold
            // Condition 3: if global optimizor (if defined) is reached

            int repeated_generations = 0;

            while (
                ga.Generation < Data.max_generations &&
                repeated_generations < Data.max_repeated_generations &&
                ga.BestFitness > Data.solution ||
                ga.BestFitness == 0
                )
            {
                double prevBestFitness = ga.BestFitness;

                // Update 
                ga.NewGeneration();

                printer.PrintCurrentGen(ga);
                //csvWriter.WriteLine(ga);

                if (prevBestFitness == ga.BestFitness)
                {
                    repeated_generations++;
                }
                else
                {
                    repeated_generations = 0;
                }
            }

            // Get the elapsed time as a TimeSpan value and format the TimeSpan value.
            stopWatch.Stop();
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", stopWatch.Elapsed.Hours, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds);

            printer.PrintResult(ga, elapsedTime);
            //csvWriter.WriteLine(ga);
            //csvWriter.SaveFile();
        }

    }

}
