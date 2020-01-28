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
            Console.WriteLine("Number of workers: {0}", Settings.NumAllMachines);
            Console.WriteLine("Number of jobs: {0}", Settings.NumJobs);
            Console.WriteLine("==============================================================================");
        }

        public void PrintCurrentGen(GA ga)
        {
            // Prevent cursor flickering
            Console.CursorVisible = false;
            Scheduler bestSchedule = ga.BestChromosome.Schedule;

            // Print current best genes
            sb = new StringBuilder();
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendFormat("Current Generation:        {0}     \n", ga.Generation);
            sb.AppendFormat("Current Assignment:        [ {0} ] \n", ga.BestChromosome.GetReadableGenes());
            sb.AppendFormat("Population Size:           {0}     \n", ga.Population.Count);
            sb.AppendFormat("Fitness Fucntion:          {0}     \n", Enum.GetName(typeof(Settings.ObjetiveFunction), Settings.ObjectiveCase));
            sb.AppendFormat("Selection Operator:        {0}     \n", Settings.SelectionOperator.SelectionName);
            sb.AppendFormat("Crossover Operator:        {0}     \n", Settings.CrossoverOperator.CrossoverName);
            sb.AppendFormat("Mutation Operator:         {0}     \n", Settings.MutationOperator.MutationName);
            sb.AppendFormat("Mutation Rate:             {0}     \n", Settings.MutationRate);
            sb.AppendFormat("Dedication Rule:           {0}     \n", Enum.GetName(typeof(Settings.DedicationType), Settings.DedicationCase));
            sb.AppendFormat("All Utilized:              {0}     \n", Settings.IsAllMachinesUtilized);
            sb.AppendFormat("Current Best Fitness:      {0:0.00}\n", ga.BestFitness);
            sb.AppendFormat("Current Best Total Cost:   {0:0.00}\n", bestSchedule.TotalCost);
            sb.AppendFormat("Travelling Cost:           {0:0.00}\n", bestSchedule.TravelCost);
            sb.AppendFormat("Handling Cost:             {0:0.00}\n", bestSchedule.HandlingCost);
            sb.AppendFormat("Rental Cost:               {0:0.00}\n", bestSchedule.RentalCost);
            sb.AppendFormat("D&D Cost:                  {0:0.00}\n", bestSchedule.DndCost);
            sb.AppendFormat("Rental Cost:               {0:0.00}\n", bestSchedule.RentalCost);
            sb.AppendFormat("Current Best SumLateStart: {0:0.00}\n", bestSchedule.SumLateStart);
            sb.AppendFormat("Current Best Makespan:     {0:0.00}\n", bestSchedule.Makespan);
            sb.AppendLine("------------------------------------------------------------------------");
            Console.Write(sb);
            Console.SetCursorPosition(0, Console.CursorTop - sb.ToString().Count(c => c == '\n'));
        }
            
        public void PrintResult(GA ga, string elapsedTime)
        {
            Scheduler bestSchedule = ga.BestChromosome.Schedule;

            // Print solution and run time
            Console.SetCursorPosition(0, Console.CursorTop + sb.ToString().Count(c => c == '\n') + 2);
            Console.WriteLine("=================================== Result ===================================");
            Console.WriteLine("Generation:          {0}",       ga.Generation);
            Console.WriteLine("Genes:               [ {0} ]",   ga.BestChromosome.GetReadableGenes());
            Console.WriteLine("Population Size:     {0}",       ga.Population.Count);
            Console.WriteLine("Fitness Fucntion:    {0}",       Enum.GetName(typeof(Settings.ObjetiveFunction), Settings.ObjectiveCase));
            Console.WriteLine("Selection Operator:  {0}",       Settings.SelectionOperator.SelectionName);
            Console.WriteLine("Crossover Operator:  {0}",       Settings.CrossoverOperator.CrossoverName);
            Console.WriteLine("Mutation Operator:   {0}",       Settings.MutationOperator.MutationName);
            Console.WriteLine("Mutation Rate:       {0}",       Settings.MutationRate);
            Console.WriteLine("Dedication Rule:     {0}",       Enum.GetName(typeof(Settings.DedicationType), Settings.DedicationCase));
            Console.WriteLine("All Utilized:        {0}",       Settings.IsAllMachinesUtilized);
            Console.WriteLine("Best Fitness:        {0:0.00}",  ga.BestFitness);
            Console.WriteLine("Total Cost:          {0:0.00}",  bestSchedule.TotalCost);
            Console.WriteLine("Travelling Cost:     {0:0.00}",  bestSchedule.TravelCost);
            Console.WriteLine("Handling Cost:       {0:0.00}",  bestSchedule.HandlingCost);
            Console.WriteLine("Rental Cost:         {0:0.00}",  bestSchedule.RentalCost);
            Console.WriteLine("D&D Cost:            {0:0.00}",  bestSchedule.DndCost);
            Console.WriteLine("SumLateStart:        {0:0.00}",  bestSchedule.SumLateStart);
            Console.WriteLine("Makespan:            {0:0.00}",  bestSchedule.Makespan);
            Console.WriteLine("RunTime:             {0}",       elapsedTime);
            PrintSchedule(bestSchedule);
            Console.WriteLine("==============================================================================\n");
        }

        public void PrintSchedule(Scheduler schedule)
        {
            Console.WriteLine("------------------------------- Schedule -------------------------------");
            for (int i = 0; i < schedule.Machines.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Machine {0}", schedule.Machines[i].Index);
                PrintColourMessage(ConsoleColor.Yellow, sb.ToString());
                Console.WriteLine("Is Third-Party: {0}, Compulsary: {1}",   schedule.Machines[i].IsThirdParty, schedule.Machines[i].IsCompulsary);
                Console.WriteLine("Accepting geared job: {0}",              schedule.Machines[i].IsGearAccepting);
                Console.WriteLine("Dedicated shipper: {0}",                 schedule.Machines[i].DedicatedCustomer);
                Console.WriteLine("Total Cost: {0}",                        schedule.Machines[i].TotalCost);

                //Console.WriteLine("Job list: [ {0} ]", string.Join(",", schedule.machines[i].assignedJobs.Select(x => x.index)));

                Console.WriteLine(
                    "{0, -10} {1, -10} {2, -15} {3, -15} {4, -15} {5, -10} {6, -10} {7, -10} {8, -10} {9, -10} {10, -10} {11, -10} {12, -10}",
                    "ID",
                    "Type",
                    "JobReadyTime",
                    "Start",
                    "End",
                    "Job ID",
                    "Priority",
                    "Geared",
                    "Dedicated",
                    "Shipper",
                    "OGV",
                    "Unload ID",
                    "Barge ID"
                    );

                for (int k = 0; k < schedule.Machines[i].ScheduledEvents.Count; k++)
                {
                    if (schedule.Machines[i].ScheduledEvents[k].Type != "Job")
                    {
                        Console.WriteLine(
                        "{0, -10} {1, -10} {2, -15} {3, -15} {4, -15} {5, -10} {6, -10} {7, -10} {8, -10} {9, -10} {10, -10} {11, -10} {12, -10}",
                        schedule.Machines[i].ScheduledEvents[k].Index,
                        schedule.Machines[i].ScheduledEvents[k].Type,
                        " ",
                        string.Format("{0:.##}", schedule.Machines[i].ScheduledEvents[k].StartTime),
                        string.Format("{0:.##}", schedule.Machines[i].ScheduledEvents[k].EndTime),
                        " ",
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
                        "{0, -10} {1, -10} {2, -15} {3, -15} {4, -15} {5, -10} {6, -10} {7, -10} {8, -10} {9, -10} {10, -10} {11, -10} {12, -10}",
                        schedule.Machines[i].ScheduledEvents[k].Index,
                        schedule.Machines[i].ScheduledEvents[k].Type,
                        string.Format("{0:.##}", schedule.Machines[i].ScheduledEvents[k]._Job.ReadyTime),
                        string.Format("{0:.##}", schedule.Machines[i].ScheduledEvents[k].StartTime),
                        string.Format("{0:.##}", schedule.Machines[i].ScheduledEvents[k].EndTime),
                        schedule.Machines[i].ScheduledEvents[k]._Job.Index,
                        schedule.Machines[i].ScheduledEvents[k]._Job.Priority,
                        schedule.Machines[i].ScheduledEvents[k]._Job.IsGeared,
                        schedule.Machines[i].ScheduledEvents[k]._Job.IsDedicated,
                        schedule.Machines[i].ScheduledEvents[k]._Job.Shipper,
                        schedule.Machines[i].ScheduledEvents[k]._Job.Ogv.Index,
                        schedule.Machines[i].ScheduledEvents[k]._Job.MachineIdUnload,
                        schedule.Machines[i].ScheduledEvents[k]._Job.MachineIdBarge
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
