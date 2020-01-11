using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace GeneticAlgorithm
{
    public class Printer
    {
        private StringBuilder sb;
        public Printer()
        {

        }
        public void PrintProblemDescription()
        {
            // Print problem description
            Console.WriteLine("======================= Assignment Problem Description =======================");
            Console.WriteLine("Number of workers: {0}", Data.numMachines);
            Console.WriteLine("Number of jobs: {0}", Data.numJobs);
            Console.WriteLine("==============================================================================");
        }

        public void PrintCurrentGen(GA ga)
        {
            // Prevent cursor flickering
            Console.CursorVisible = false;
            Scheduler bestSchedule = ga.BestChromosome.schedule;

            // Print current best genes
            sb = new StringBuilder();
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendFormat("Current Generation:        {0}     \n", ga.Generation);
            //sb.AppendFormat("Current Assignment:        [ {0} ] \n", string.Join(",", ga.BestGenes.Select(x => x <= 100)));
            sb.AppendFormat("Fitness Fucntion:          {0}     \n", Enum.GetName(typeof(Data.objetiveFunction), Data.objectiveCase));
            sb.AppendFormat("Crossover Fucntion:        {0}     \n", Enum.GetName(typeof(Data.crossoverFunction), Data.crossoverMethod));
            sb.AppendFormat("Dedication Rule:           {0}     \n", Enum.GetName(typeof(Data.dedicationType), Data.dedicationCase));
            sb.AppendFormat("All Utilized:              {0}     \n", Data.isAllMachinesUtilized);
            sb.AppendFormat("Current Best Fitness:      {0:0.00}\n", ga.BestFitness);
            sb.AppendFormat("Current Best Total Cost:   {0:0.00}\n", bestSchedule.totalCost);
            sb.AppendFormat("Travelling Cost:           {0:0.00}\n", bestSchedule.travelCost);
            sb.AppendFormat("Handling Cost:             {0:0.00}\n", bestSchedule.handlingCost);
            sb.AppendFormat("D&D Cost:                  {0:0.00}\n", bestSchedule.dndCost);
            sb.AppendFormat("Current Best SumLateStart: {0:0.00}\n", bestSchedule.sumLateStart);
            sb.AppendFormat("Current Best Makespan:     {0:0.00}\n", bestSchedule.makespan);
            sb.AppendFormat("Population Size:           {0}     \n", ga.Population.Count);
            sb.AppendFormat("Mutation Rate:             {0}     \n", Data.mutationRate);
            sb.AppendLine("------------------------------------------------------------------------");
            Console.Write(sb);
            Console.SetCursorPosition(0, Console.CursorTop - sb.ToString().Count(c => c == '\n'));
        }
            
        public void PrintResult(GA ga, string elapsedTime)
        {
            Scheduler bestSchedule = ga.BestChromosome.schedule;

            // Print solution and run time
            Console.SetCursorPosition(0, Console.CursorTop + sb.ToString().Count(c => c == '\n') + 2);
            Console.WriteLine("=================================== Result ===================================");
            Console.WriteLine("Generation:          {0}",       ga.Generation);
            Console.WriteLine("Genes:               [ {0} ]",   String.Join(", ", ga.BestGenes));
            Console.WriteLine("Fitness Fucntion:    {0}",       Enum.GetName(typeof(Data.objetiveFunction), Data.objectiveCase));
            Console.WriteLine("Crossover Fucntion:  {0}",       Enum.GetName(typeof(Data.crossoverFunction), Data.crossoverMethod));
            Console.WriteLine("Dedication Rule:     {0}",       Enum.GetName(typeof(Data.dedicationType), Data.dedicationCase));
            Console.WriteLine("All Utilized:        {0}",       Data.isAllMachinesUtilized);
            Console.WriteLine("Best Fitness:        {0:0.00}",  ga.BestFitness);
            Console.WriteLine("Total Cost:          {0:0.00}",  bestSchedule.totalCost);
            Console.WriteLine("Travelling Cost:     {0:0.00}",  bestSchedule.travelCost);
            Console.WriteLine("Handling Cost:       {0:0.00}",  bestSchedule.handlingCost);
            Console.WriteLine("D&D Cost:            {0:0.00}",  bestSchedule.dndCost);
            Console.WriteLine("SumLateStart:        {0:0.00}",  bestSchedule.sumLateStart);
            Console.WriteLine("Makespan:            {0:0.00}",  bestSchedule.makespan);
            Console.WriteLine("Population Size:     {0}",       ga.Population.Count);
            Console.WriteLine("RunTime:             {0}",       elapsedTime);
            PrintSchedule(bestSchedule);
            Console.WriteLine("==============================================================================\n");
        }

        public void PrintSchedule(Scheduler schedule)
        {
            Console.WriteLine("------------------------------- Schedule -------------------------------");
            for (int i = 0; i < schedule.machines.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Machine {0}", schedule.machines[i].index);
                PrintColourMessage(ConsoleColor.Yellow, sb.ToString());
                Console.WriteLine("Is Third-Party: {0}, Compulsary: {1}", schedule.machines[i].isThirdParty, schedule.machines[i].isCompulsary);
                Console.WriteLine("Accepting geared job: {0}",  schedule.machines[i].isGearAccepting);
                Console.WriteLine("Dedicated shipper: {0}",     schedule.machines[i].dedicatedCustomer);

                //Console.WriteLine("Job list: [ {0} ]", string.Join(",", schedule.machines[i].assignedJobs.Select(x => x.index)));

                Console.WriteLine(
                    "{0, -10} {1, -10} {2, -15} {3, -15} {4, -15} {5, -10} {6, -10} {7, -10} {8, -10} {9, -10} {10, -10} {11, -10}",
                    "id",
                    "type",
                    "jobReadyTime",
                    "start",
                    "end",
                    "jobId",
                    "priority",
                    "geared",
                    "dedicated",
                    "shipper",
                    "idUnload",
                    "idBarge"
                    );

                for (int k = 0; k < schedule.machines[i].scheduledEvents.Count; k++)
                {
                    if (schedule.machines[i].scheduledEvents[k].type == "Stoppage")
                    {
                        Console.WriteLine(
                        "{0, -10} {1, -10} {2, -15} {3, -15} {4, -15} {5, -10} {6, -10} {7, -10} {8, -10} {9, -10} {10, -10} {11, -10}",
                        schedule.machines[i].scheduledEvents[k].index,
                        schedule.machines[i].scheduledEvents[k].type,
                        " ",
                        schedule.machines[i].scheduledEvents[k].startTime,
                        schedule.machines[i].scheduledEvents[k].endTime,
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " ",
                        " "
                        );
                    }
                    else
                    {
                        Console.WriteLine(
                        "{0, -10} {1, -10} {2, -15} {3, -15} {4, -15} {5, -10} {6, -10} {7, -10} {8, -10} {9, -10} {10, -10} {11, -10}",
                        schedule.machines[i].scheduledEvents[k].index,
                        schedule.machines[i].scheduledEvents[k].type,
                        schedule.machines[i].scheduledEvents[k].job.readyTime,
                        schedule.machines[i].scheduledEvents[k].startTime,
                        schedule.machines[i].scheduledEvents[k].endTime,
                        schedule.machines[i].scheduledEvents[k].job.index,
                        schedule.machines[i].scheduledEvents[k].job.priority,
                        schedule.machines[i].scheduledEvents[k].job.isGeared,
                        schedule.machines[i].scheduledEvents[k].job.isDedicated,
                        schedule.machines[i].scheduledEvents[k].job.shipper,
                        schedule.machines[i].scheduledEvents[k].job.machineIdUnload,
                        schedule.machines[i].scheduledEvents[k].job.machineIdBarge
                        );
                    }
                    
                }
                Console.WriteLine("\n");
            }

            Console.WriteLine("------------------------------------------------------------------------");
        }


        public void Print2DArray<T>(T[,] matrix, string delim)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write("{0,-6}", matrix[i, j]);
                    if (j < matrix.GetLength(1)-1)
                    {
                        Console.Write(delim);
                    }
                }
                Console.WriteLine();
            }
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
