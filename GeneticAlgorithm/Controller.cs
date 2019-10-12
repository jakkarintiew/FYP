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

            // Set stopping conditions
            int solution = 0;
            int repeated_generations = 0;
            int max_repeated_generations = 1000;
            int max_generations = 1000;

            // Update algorithm until stopping conditions are met:
            // Condition 1: if total number of generation reaches a threshold
            // Condition 2: if number of generation where the fitness remained constant reaches a threshold
            // Condition 3: if global optimizor (if defined) is reached

            while (
                ga.Generation < max_generations &&
                repeated_generations < max_repeated_generations &&
                ga.BestFitness > solution ||
                ga.BestFitness == 0
                )
            {
                printer.PrintCurrentGen(ga);
                //csvWriter.WriteLine(ga);

                double prevBestFitness = ga.BestFitness;

                // Update 
                ga.NewGeneration();

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
