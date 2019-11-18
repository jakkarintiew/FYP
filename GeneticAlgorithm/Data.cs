using System;
using System.Collections.Generic;
using System.Linq;
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
            { 65, 120, 65, 245, 45, 175, 90, 190, 230, 45, 180, 45, 180, 200, 145 },
            { 175, 40, 150, 65, 145, 195, 125, 190, 80, 160, 45, 195, 150, 70, 200 }
        };

        public static Matrix<double> cost_mat = Matrix<double>.Build.DenseOfArray(cost_data);

        public static Object[,] machine_data = new Object[,] {
            { 0, 150.745, 1.926, true,  true,  "shipper1",      new double[,] { { 3099.885, 85930.178 },{ 95559.275, 180312.269 },{ 214097.686, 296858.210 }, }, },
            { 1, 120.512, 2.366, true,  true,  "shipper4",      new double[,] { { 2916.794, 32975.707 },{ 95923.664, 128509.097 },{ 213789.128, 257985.301 }, } },
            { 2, 200.876, 2.156, false, false, "notdedicated",  new double[,] { { 4941.350, 86907.137 },{ 98184.366, 180862.197 } }, },
            { 3, 10.962, 2.563,  true,  false, "notdedicated",  new double[,] { { 10952.166, 85535.972 },{ 158184.366, 240862.197} } }
        };

        public static int num_machines = machine_data.GetLength(0);
        public static List<Machine> machines = InitMachines();

        public static Object[,] job_data = new Object[,] {
        { 0,  1178.512,   8039.669, true,  true, "shipper1", 3 },
        { 1,  6780.412,   1331.752, false, true, "shipper1", 5 },
        { 2,  10207.541,  6913.044, false, true, "shipper4", 2 },
        { 3,  19577.121,  7169.225, true,  false, "shipper2", 1 },
        { 4,  29968.311,  1129.289, false, true, "shipper4", 2 },
        { 5,  30519.511,  9131.787, true,  false, "shipper2", 8 },
        { 6,  34016.101,  1462.044, false, false, "shipper0", 9 },
        { 7,  36882.100, 1363.404, false, true, "shipper1", 8 },
        { 8,  137387.012, 8561.048, false, true, "shipper1", 6 },
        { 9,  145519.530, 8583.183, true,  true, "shipper4", 7 },
        { 10, 145817.994, 8325.638, true,  false, "shipper5", 1 },
        { 11, 154801.123, 6256.804, false, false, "shipper0", 9 },
        { 12, 165034.541, 8684.176, false, false, "shipper5", 5 },
        { 13, 165566.233, 5296.405, true,  false, "shipper2", 6 },
        { 14, 165936.874, 2686.404, false, false, "shipper0", 9 },
                                          
        };
        public static int num_jobs = job_data.GetLength(0);
        public static List<Job> jobs = InitJobs();

        // GA parameters
        public static int populationSize = 100;
        public static int numNewDNA = 20;
        public static float mutationRate = 0.05f;
        public static int elitism = 5;

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
            return jobs.OrderBy(o => o.readyTime).ToList();
        }

    }
}

   
