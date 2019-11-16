using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace GeneticAlgorithm
{
    public static class Data
    {
        // Problem static data
        public static double[,] cost_data = 
        {
            { 55, 225, 195, 125, 125, 175, 140, 105, 105, 210, 85, 90, 220, 145, 245 },
            { 215, 55, 100, 175, 225, 125, 105, 140, 250, 60, 235, 250, 120, 210, 120 },
            { 65, 120, 115, 245, 45, 175, 90, 190, 230, 45, 180, 100, 180, 200, 145 },
            { 175, 40, 150, 65, 145, 195, 125, 190, 80, 160, 45, 195, 150, 70, 200 }
        };

        public static Matrix<double> cost_mat = Matrix<double>.Build.DenseOfArray(cost_data);

        public static Object[,] machine_data = new Object[,] {
            { 0, 150.745, 3.926, true, true, "shipper1", new double[,] { { 3099.885, 85930.178 },{ 95559.275, 180312.269 },{ 184097.686, 266858.210 }, }, },
            { 1, 120.512, 3.366, true, true, "shipper4", new double[,] { { 2916.794, 82975.707 },{ 66913.276, 151804.031 },{ 125923.664, 208509.097 }, }, },
            { 2, 200.876, 3.156, true, false, "notdedicated", new double[,] { { 4941.350, 86907.137 },{ 98184.366, 180862.197 },{ 173789.128, 257985.301 }, }, },
            { 3, 10.962, 4.563, false, false, "notdedicated", new double[,] { { 3952.166, 85535.972 },{ 96250.919, 176489.293 },{ 185258.489, 267339.117 }, }, },

        };

        public static int num_machines = machine_data.GetLength(0);
        public static List<Machine> machines = InitMachines();

        public static Object[,] job_data = new Object[,] {
        { 0, 1178.551, 8039.669, true, false, "shipper2", 3 },
        { 1, 16780.551, 1331.752, false, false, "shipper5", 5 },
        { 2, 30207.551, 6913.044, false, true, "shipper4", 2 },
        { 3, 39577.551, 7169.225, true, true, "shipper4", 1 },
        { 4, 83968.551, 1129.289, false, true, "shipper4", 2 },
        { 5, 94519.551, 9131.787, true, true, "shipper1", 8 },
        { 6, 104016.551, 1462.044, false, true, "shipper1", 9 },
        { 7, 136882.551, 1363.404, false, false, "shipper2", 3 },
        { 8, 164387.551, 8561.048, false, false, "shipper3", 6 },
        { 9, 195519.551, 8583.183, true, true, "shipper1", 7 },
        { 10, 228817.551, 8325.638, true, false, "shipper0", 1 },
        { 11, 246801.551, 6256.804, false, false, "shipper4", 9 },
        { 12, 252034.551, 8684.176, false, false, "shipper5", 5 },
        { 13, 289566.551, 5296.405, true, false, "shipper2", 6 },
        { 14, 305336.551, 2686.404, false, false, "shipper0", 2 },

        };
        public static int num_jobs = job_data.GetLength(0);
        public static List<Job> jobs = InitJobs();

        // GA parameters
        public static int populationSize = 200;
        public static int numNewDNA = 40;
        public static float mutationRate = 0.05f;
        public static int elitism = 10;

        public enum crossoverFunction { Uniform, SinglePoint, TwoPoint}
        public static int crossoverMethod = 0;

        public enum objetiveFunction { TotalCost, Makespan }
        public static int objectiveCase = 1;

        public enum dedicationType { Felxible, Strict}
        public static int dedicationCase = 0;

        // Set stopping conditions
        public static double solution = 0;
        public static int max_repeated_generations = 100;
        public static int max_generations = 200;

        public static List<Machine> InitMachines()
        {
            List<Machine> machines = new List<Machine>();
            for (int i = 0; i < machine_data.GetLength(0); i++)
            {
                machines.Add(new Machine(
                    index: (int)machine_data[i, 0],
                    readyTime: (double)machine_data[i, 1],
                    procRate: (double)machine_data[i, 2],
                    isGearAccepting: (bool)machine_data[i, 3],
                    isDedicated: (bool)machine_data[i, 4],
                    dedicatedCustomer: (string)machine_data[i, 5],
                    downTimes: (double[,])machine_data[i, 6]
                    ));
            }
            return machines;
        }

        public static List<Job> InitJobs()
        {
            List<Job> jobs = new List<Job>();
            for (int i = 0; i < job_data.GetLength(0); i++)
            {
                jobs.Add(new Job(
                    index: (int)job_data[i, 0], 
                    readyTime: (double)job_data[i, 1], 
                    quantity: (double)job_data[i, 2],
                    isGeared: (bool)job_data[i, 3],
                    isDedicated: (bool)job_data[i, 4],
                    shipper: (string)job_data[i, 5],
                    priority: (int)job_data[i, 6]
                    ));
            }
            return jobs.OrderBy(o => o.readyTime).ToList(); ;
        }

    }
}

   
