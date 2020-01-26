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
        private Writer out1_csvWriter;
        private Writer out2_csvWriter;


        // Constructor
        public Controller()
        {
        }

        public void Start()
        {
            // Start timer 
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Data.init3rdMachines();
            printer = new Printer();
            printer.PrintProblemDescription();

            // Initialize ga instance
            ga = new GA();


            out1_csvWriter = new Writer(
                @"..\..\..\out1.csv",
                "Generation,Fitness"
                );

            out2_csvWriter = new Writer(
                @"..\..\..\out2.csv",
                "machineId,position,readyTime,procRate,latestPosition,latestReadyTime,loadingUnitCost,isGearAccepting,isDedicated,dedicatedCustomer,isThirdParty,isCompulsary,eventId,eventType,eventStartTime,eventEndTime,jobId,position,readyTime,requestedProcRate,requestedProcTime,procTime,quantity,isGeared,isDedicated,shipper,isOutOfLaycan,isUnloading,machineIdUnload,isBarge,machineIdBarge,demurrage,despatch,priority,totalCost,travelCost,handlingCost,dndCost"
                );


            // Update algorithm until stopping conditions are met:
            // Condition 1: if total number of generation reaches a threshold
            // Condition 2: if number of generation where the fitness remained constant reaches a threshold
            // Condition 3: if global optimizor (if defined) is reached

            int repeated_generations = 0;

            while (
                ga.Generation < Data.MaxGenerations &&
                repeated_generations < Data.MaxRepeatedGenerations &&
                ga.BestFitness > Data.Solution ||
                ga.BestFitness == 0
                )
            {
                double prevBestFitness = ga.BestFitness;

                // Update 
                ga.NewGeneration();

                printer.PrintCurrentGen(ga);
                out1_csvWriter.WriteLine(string.Format("{0}, {1}", ga.Generation, ga.BestFitness));

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

            foreach (Machine machine in ga.BestChromosome.schedule.machines)
            {
                foreach (Event evt in machine.scheduledEvents)
                {
                    out2_csvWriter.WriteLine(
                        string.Format(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37}",
                            machine.index,
                            machine.position,
                            machine.readyTime,
                            machine.procRate,
                            machine.latestPosition,
                            machine.latestReadyTime,
                            machine.loadingUnitCost,
                            machine.isGearAccepting,
                            machine.isDedicated,
                            machine.dedicatedCustomer,
                            machine.isThirdParty,
                            machine.isCompulsary,
                            evt.index,
                            evt.type,
                            evt.startTime,
                            evt.endTime,
                            evt.job.index,
                            evt.job.position,
                            evt.job.readyTime,
                            evt.job.requestedProcRate,
                            evt.job.requestedProcTime,
                            evt.job.procTime,
                            evt.job.quantity,
                            evt.job.isGeared,
                            evt.job.isDedicated,
                            evt.job.shipper,
                            evt.job.isOutOfLaycan,
                            evt.job.isUnloading,
                            evt.job.machineIdUnload,
                            evt.job.isBarge,
                            evt.job.machineIdBarge,
                            evt.job.demurrage,
                            evt.job.despatch,
                            evt.job.priority,
                            evt.job.totalCost,
                            evt.job.travelCost,
                            evt.job.handlingCost,
                            evt.job.dndCost
                        )
                    );
                }

            }

            out1_csvWriter.SaveFile();
            out2_csvWriter.SaveFile();

        }

    }

}
