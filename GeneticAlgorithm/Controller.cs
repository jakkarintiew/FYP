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

            Settings.Init3rdMachines();
            printer = new Printer();
            printer.PrintProblemDescription();

            // Initialize ga instance
            ga = new GA(
                Settings.SelectionOperator,
                Settings.CrossoverOperator, 
                Settings.MutationOperator
                );


            out1_csvWriter = new Writer(
                @"..\..\..\out1.csv",
                "Generation,Fitness"
                );

            out2_csvWriter = new Writer(
                @"..\..\..\out2.csv",
                "Index, OriginalLatitude, OriginalLongitude, OriginalReadyTime, ProcRate, Latitude, Longitude, LatestReadyTime, LoadingUnitCost, IsGearAccepting, IsDedicated, DedicatedCustomer, IsThirdParty, IsCompulsary, EventIndex, Type, StartTime, EndTime, JobIndex, Latitude, Longitude, ReadyTime, RequestedProcRate, RequestedProcTime, ProcTime, Quantity, IsGeared, IsDedicated, Shipper, IsOutOfLaycan, IsUnloading, MachineIdUnload, IsBarge, MachineIdBarge, Demurrage, Despatch, Priority, TotalCost, TravelCost, HandlingCost, DndCost"
                );

            // Update algorithm until stopping conditions are met:
            // Condition 1: if total number of generation reaches a threshold
            // Condition 2: if number of generation where the fitness remained constant reaches a threshold
            // Condition 3: if global optimizor (if defined) is reached

            int repeated_generations = 0;

            while (
                ga.Generation < Settings.MaxGenerations &&
                repeated_generations < Settings.MaxRepeatedGenerations &&
                ga.BestFitness > Settings.Solution ||
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

            foreach (Machine machine in ga.BestChromosome.Schedule.Machines)
            {
                foreach (Event evt in machine.ScheduledEvents)
                {
                    out2_csvWriter.WriteLine(
                        string.Join(",",machine.Index,
                            machine.OriginalLatitude,
                            machine.OriginalLongitude,
                            machine.OriginalReadyTime,
                            machine.ProcRate,
                            machine.Latitude,
                            machine.Longitude,
                            machine.LatestReadyTime,
                            machine.LoadingUnitCost,
                            machine.IsGearAccepting,
                            machine.IsDedicated,
                            machine.DedicatedCustomer,
                            machine.IsThirdParty,
                            machine.IsCompulsary,
                            evt.Index,
                            evt.Type,
                            evt.StartTime,
                            evt.EndTime,
                            evt._Job.Index,
                            evt._Job.Latitude,
                            evt._Job.Longitude,
                            evt._Job.ReadyTime,
                            evt._Job.RequestedProcRate,
                            evt._Job.RequestedProcTime,
                            evt._Job.ProcTime,
                            evt._Job.Quantity,
                            evt._Job.IsGeared,
                            evt._Job.IsDedicated,
                            evt._Job.Shipper,
                            evt._Job.IsOutOfLaycan,
                            evt._Job.IsUnloading,
                            evt._Job.MachineIdUnload,
                            evt._Job.IsBarge,
                            evt._Job.MachineIdBarge,
                            evt._Job.Demurrage,
                            evt._Job.Despatch,
                            evt._Job.Priority,
                            evt._Job.TotalCost,
                            evt._Job.TravelCost,
                            evt._Job.HandlingCost,
                            evt._Job.DndCost
                        )
                    );
                }

            }

            out1_csvWriter.SaveFile();
            out2_csvWriter.SaveFile();

        }

    }

}
