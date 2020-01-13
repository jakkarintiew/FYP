using System;
using System.Collections.Generic;
using System.Linq;
using Ganss.Excel;

namespace GeneticAlgorithm
{
    public static class Data
    {
        // Problem static data
        public static List<Stoppage> AllStoppages = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Stoppage>("stoppages").ToList();
        public static List<OGV> OGVs = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<OGV>("ogvs").ToList();
        public static List<Machine> Machines = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Machine>("machines").ToList();
        public static List<Job> Jobs = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Job>("jobs").ToList().OrderBy(o => o.readyTime).ToList();
        public static int NumMachines = Machines.Count;
        public static int NumJobs = Jobs.Count;

        // GA hyper-parameters
        public static int PopulationSize = 200;
        public static int NumNewDNA = (int)0.2 * PopulationSize;
        public static float MutationRate = 0.05f;
        public static int Elitism = 5;
        public enum CrossoverFunction { Uniform, SinglePoint, TwoPoint}
        public static int CrossoverMethod = 0;
        public enum ObjetiveFunction { TotalCostNoPriority, TotalCostWithPriority, DemurrageDespatchCost, SumLateStart, Makespan }
        public static int objectiveCase = 1;
        public enum DedicationType { Flexible, Strict }
        public static int DedicationCase = 0;
        public static bool IsAllMachinesUtilized = true;
        public static double PriorityGapTime = 10800.00;
        public static double InterrupedSetUpTime = 10800.00;

        // Stopping conditions
        public static double Solution = double.MinValue;
        public static int MaxRepeatedGenerations = 100;
        public static int MaxGenerations = 500;
    }
}

   
