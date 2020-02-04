using System;
using System.Collections.Generic;
using System.Linq;
using Ganss.Excel;


namespace GeneticAlgorithm
{
    public static class Settings
    {
        // Static data
        public static List<Stoppage> AllStoppages = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Stoppage>("Stoppages").ToList();
        public static List<OGV> OGVs = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<OGV>("OGVs").ToList();
        public static List<Machine> Machines = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Machine>("Machines").ToList();
        public static List<Job> Jobs = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Job>("Jobs").ToList().OrderBy(o => o.ReadyTime).ToList();
        public static int NumMachines = Machines.Count;
        public static int NumAllMachines;
        public static int NumJobs = Jobs.Count;

        public static void Init3rdMachines()
        {
            Machines = new ExcelMapper(@"..\..\..\in.xlsx").Fetch<Machine>("machines").ToList();

            int Num3rdMachiens = NumCom3rdMachines + NumOpt3rdMachines;

            if (NumJobs < NumMachines)
            {
                IsAllMachinesUtilized = false;
                NumCom3rdMachines = 0;
                NumOpt3rdMachines = 0;
            } else if ((NumJobs - NumMachines) < NumCom3rdMachines)
            {
                NumOpt3rdMachines += NumCom3rdMachines - (NumJobs - NumMachines);
                NumCom3rdMachines = NumJobs - NumMachines;
            }

            for (int i = 0; i < NumCom3rdMachines; i++)
            {
                Machine new3rdMachine = new Machine();
                new3rdMachine.Index = Machines.Count;
                new3rdMachine.IsThirdParty = true;
                new3rdMachine.IsCompulsary = true;
                Machines.Add((Machine)new3rdMachine.Clone());
            }

            for (int i = 0; i < NumOpt3rdMachines; i++)
            {
                Machine new3rdMachine = new Machine();
                new3rdMachine.Index = Machines.Count;
                new3rdMachine.IsThirdParty = true;
                new3rdMachine.IsCompulsary = false;
                Machines.Add((Machine)new3rdMachine.Clone());
            }

            NumAllMachines = Machines.Count;
        }

        // GA hyper-parameters
        public static int PopulationSize = 200;
        public static bool EnableCrossoverNewDNA = true;
        public static int NumNewDNA = (int)Math.Round(0.2 * PopulationSize);
        public static double MutationRate = 0.05;
        public static int Elitism = 5;
        public static ICrossover CrossoverOperator = new PositionBasedCrossover();
        public static IMutation MutationOperator = new InsertionMutation();
        public static ISelection SelectionOperator = new TournamentSelection();
        public enum ObjetiveFunction { TotalCostNoPriority, TotalCostWithPriority, DemurrageDespatchCost, SumLateStart, Makespan }
        public static int ObjectiveCase = 1;
        public enum DedicationType { Flexible, Strict, None }
        public static int DedicationCase = 0;
        public static double PriorityGapTime = 10800.00;
        public static double InterrupedSetUpTime = 800.00;
        public static double AnchorageTime = 1800.00;
        public static double CastingOffTime = 1800.00;
        public static bool IsAllMachinesUtilized = true;
        public static int NumCom3rdMachines = 1;
        public static int NumOpt3rdMachines = 1;
        //public static double ThirdMachinePosition = 0.00;
        public static double ThirdMachineLatitude = 0.00;
        public static double ThirdMachineLongitude = 0.00;
        public static double ThirdMachineSpeed = 0.20;
        public static double ThirdMachineReadyTime = 0.00;
        public static double ThirdMachineProcRate = 12.00;
        public static double ThirdMachineRentalCost = 1.00;

        // Stopping conditions
        public static double Solution = double.MinValue;
        public static int MaxRepeatedGenerations = 100;
        public static int MaxGenerations = 500;
    }
}

   
