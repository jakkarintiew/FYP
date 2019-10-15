using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine("====== Assignment Problem Description ======");
            Console.WriteLine("Costs matrix: ");
            Print2DArray(Data.costs);
            Console.WriteLine("\n");
            Console.WriteLine("Number of workers: {0}", Data.num_machines);
            Console.WriteLine("Number of jobs: {0}", Data.num_jobs);
            Console.WriteLine("============================================");
        }

        public void PrintCurrentGen(GA ga)
        {
            // Prevent cursor flickering
            Console.CursorVisible = false;
            Schedule bestSchedule = new Schedule(ga.BestGenes);

            // Print current best genes
            sb = new StringBuilder();
            sb.AppendLine("-------------------------------");
            sb.AppendFormat("Current Generation:    {0}     \n", ga.Generation);
            sb.AppendFormat("Current Assignment:    [ {0} ] \n", string.Join(", ", ga.BestGenes));
            sb.AppendFormat("Fitness Fucntion:      {0}     \n", Enum.GetName(typeof(Data.objetiveFunction), Data.objectiveCase));
            sb.AppendFormat("Current Best Fitness:  {0}     \n", ga.BestFitness);
            sb.AppendFormat("Current Cost:          {0}     \n", bestSchedule.cost);
            sb.AppendFormat("Current Makespan:      {0}     \n", bestSchedule.makespan);
            sb.AppendFormat("Population Size:       {0}     \n", ga.Population.Count);
            sb.AppendFormat("Mutation Rate:         {0}     \n", Data.mutationRate);
            sb.AppendLine("-------------------------------");
            Console.Write(sb);
            Console.SetCursorPosition(0, Console.CursorTop - sb.ToString().Count(c => c == '\n'));
        }
            
        public void PrintResult(GA ga, string elapsedTime)
        {
            Schedule bestSchedule = new Schedule(ga.BestGenes);

            // Print solution and run time
            Console.SetCursorPosition(0, Console.CursorTop + sb.ToString().Count(c => c == '\n') + 2);
            Console.WriteLine("========= Result =========");
            Console.WriteLine("Generation:          {0}",            ga.Generation);
            Console.WriteLine("Assignment:          [ {0} ]",        String.Join(", ", ga.BestGenes));
            Console.WriteLine("Best Fitness:        {0}",          ga.BestFitness);
            Console.WriteLine("Cost:                {0}",                  bestSchedule.cost);
            Console.WriteLine("Makespan:            {0}",              bestSchedule.makespan);
            Console.WriteLine("Population Size:     {0}",  ga.Population.Count);
            Console.WriteLine("RunTime:             {0}",               elapsedTime);
            PrintSchedule(bestSchedule);
            Console.WriteLine("==========================\n");
        }

        public void PrintSchedule(Schedule schedule)
        {
            Console.WriteLine("------- Schedule -------");
            for (int i = 0; i < schedule.machines.Count; i++)
            {
                Console.WriteLine("Machine {0} Job list: [ {1} ]", schedule.machines[i].index, string.Join(",", schedule.machines[i].assignedJobs.Select(x => x.index)));
                for (int j = 0; j < schedule.machines[i].assignedJobs.Count; j++)
                {
                    Console.WriteLine("Job {0,-5} Ready Time = {1,-10} Start Time = {2,-10} Complete Time = {3,-10}",
                        schedule.machines[i].assignedJobs[j].index + ":",
                        schedule.machines[i].assignedJobs[j].readyTime,
                        schedule.machines[i].assignedJobs[j].startTime,
                        schedule.machines[i].assignedJobs[j].completeTime);
                }
            }
            Console.WriteLine("------------------------");
        }

        private void Print2DArray<T>(T[,] matrix)
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
