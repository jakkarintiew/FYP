using System;
using System.Collections.Generic;
using System.Linq;
using Ganss.Excel;

namespace GeneticAlgorithm
{
    public static class Data
    {
        // Problem static data
        public static List<Stoppage> all_stoppages = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Stoppage>("stoppages").ToList();
        public static List<Machine> machines = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Machine>("machines").ToList();
        public static List<Job> jobs = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Job>("jobs").ToList().OrderBy(o => o.readyTime).ToList();
        public static int numMachines = machines.Count;
        public static int numJobs = jobs.Count;

        // GA parameters
        public static int populationSize = 200;
        public static int numNewDNA = (int)0.2 * populationSize;
        public static float mutationRate = 0.05f;
        public static int elitism = 5;

        public enum crossoverFunction { Uniform, SinglePoint, TwoPoint}
        public static int crossoverMethod = 0;

        public enum objetiveFunction { TotalCostNoPriority, TotalCostWithPriority, DemurrageDespatchCost, SumLateStart, Makespan }
        public static int objectiveCase = 1;

        public enum dedicationType { Flexible, Strict }
        public static int dedicationCase = 0;

        // Set stopping conditions
        public static double solution = double.MinValue;
        public static int maxRepeatedGenerations = 100;
        public static int maxGenerations = 1000;
    }
}

   
