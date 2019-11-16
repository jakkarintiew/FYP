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
            //Console.WriteLine("Costs matrix: ");
            //Print2DArray(Data.cost_data);
            //Console.WriteLine("\n");
            Console.WriteLine("Number of workers: {0}", Data.num_machines);
            Console.WriteLine("Number of jobs: {0}", Data.num_jobs);
            Console.WriteLine("==============================================================================");
        }

        public void PrintCurrentGen(GA ga)
        {
            // Prevent cursor flickering
            Console.CursorVisible = false;
            Schedule bestSchedule = new Schedule();
            bestSchedule.GetSchedule(ga.BestGenes);

            // Print current best genes
            sb = new StringBuilder();
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendFormat("Current Generation:        {0}     \n", ga.Generation);
            sb.AppendFormat("Current Assignment:        [ {0} ] \n", string.Join(", ", ga.BestGenes));
            sb.AppendFormat("Fitness Fucntion:          {0}     \n", Enum.GetName(typeof(Data.objetiveFunction), Data.objectiveCase));
            sb.AppendFormat("Crossover Fucntion:        {0}     \n", Enum.GetName(typeof(Data.crossoverFunction), Data.crossoverMethod));
            sb.AppendFormat("Current Best Fitness:      {0}     \n", ga.BestFitness);
            sb.AppendFormat("Current Best Cost:         {0}     \n", bestSchedule.cost);
            sb.AppendFormat("Current Best Makespan:     {0}     \n", bestSchedule.makespan);
            sb.AppendFormat("Current Best Feasibility:  {0}     \n", bestSchedule.isFeasible);
            sb.AppendFormat("Population Size:           {0}     \n", ga.Population.Count);
            sb.AppendFormat("Mutation Rate:             {0}     \n", Data.mutationRate);
            sb.AppendLine("------------------------------------------------------------------------");
            Console.Write(sb);
            Console.SetCursorPosition(0, Console.CursorTop - sb.ToString().Count(c => c == '\n'));
        }
            
        public void PrintResult(GA ga, string elapsedTime)
        {
            Schedule bestSchedule = new Schedule();
            bestSchedule.GetSchedule(ga.BestGenes);

            // Print solution and run time
            Console.SetCursorPosition(0, Console.CursorTop + sb.ToString().Count(c => c == '\n') + 2);
            Console.WriteLine("=================================== Result ===================================");
            Console.WriteLine("Generation:          {0}",   ga.Generation);
            Console.WriteLine("Assignment:          [ {0} ]", String.Join(", ", ga.BestGenes));
            Console.WriteLine("Best Fitness:        {0}",   ga.BestFitness);
            Console.WriteLine("Cost:                {0}",   bestSchedule.cost);
            Console.WriteLine("Makespan:            {0}",   bestSchedule.makespan);
            Console.WriteLine("Feasibility:         {0}",   bestSchedule.isFeasible);
            Console.WriteLine("Population Size:     {0}",   ga.Population.Count);
            Console.WriteLine("RunTime:             {0}",   elapsedTime);
            PrintSchedule(bestSchedule);
            Console.WriteLine("==============================================================================\n");
        }

        public void PrintSchedule(Schedule schedule)
        {
            Console.WriteLine("------------------------------- Schedule -------------------------------");
            for (int i = 0; i < schedule.machines.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Machine {0}", schedule.machines[i].index);
                PrintColourMessage(ConsoleColor.Cyan, sb.ToString());


                Console.WriteLine("Down times:"); Print2DArray( schedule.machines[i].downTimes, "  ->  ");
                Console.WriteLine("Accepting geared job: {0}",  schedule.machines[i].isGearAccepting);
                Console.WriteLine("Dedicated shipper: {0}",     schedule.machines[i].dedicatedCustomer);
                Console.WriteLine("Job list: [ {0} ]", string.Join(",", schedule.machines[i].assignedJobs.Select(x => x.index)));

                Console.WriteLine("{0, -10} {1, -10} {2, -10} {3, -10} {4, -10} {5, -15} {6, -15} {7, -15}",
                    "Index",
                    "Geared",
                    "Dedicated",
                    "Shipper",
                    "Priority",
                    "Ready Time",
                    "Start Time",
                    "Complete Time");

                for (int j = 0; j < schedule.machines[i].assignedJobs.Count; j++)
                {
                    Console.WriteLine("{0, -10} {1, -10} {2, -10} {3, -10} {4, -10} {5, -15} {6, -15} {7, -15}",
                        schedule.machines[i].assignedJobs[j].index,
                        schedule.machines[i].assignedJobs[j].isGeared,
                        schedule.machines[i].assignedJobs[j].isDedicated,
                        schedule.machines[i].assignedJobs[j].shipper,
                        schedule.machines[i].assignedJobs[j].priority,
                        schedule.machines[i].assignedJobs[j].readyTime,
                        schedule.machines[i].assignedJobs[j].startTime,
                        schedule.machines[i].assignedJobs[j].completeTime);
                }
                Console.WriteLine(" ");
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
