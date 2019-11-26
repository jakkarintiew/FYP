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
            { 100, 205, 55, 160, 110, 235, 170, 150, 135, 100, 150, 220, 230, 155, 120, 195, 215, 25, 115, 130, 35, 30, 80, 85, 160, 80 },
            { 105, 245, 95, 225, 50, 30, 50, 185, 155, 235, 75, 230, 245, 200, 200, 250, 250, 220, 55, 25, 160, 50, 45, 140, 195, 95 },
            { 245, 95, 245, 125, 45, 80, 80, 60, 95, 195, 235, 180, 75, 250, 145, 115, 40, 70, 85, 195, 190, 195, 190, 185, 25, 30 },
            { 110, 140, 60, 210, 240, 175, 165, 220, 245, 195, 240, 220, 150, 35, 250, 205, 140, 215, 205, 50, 75, 165, 55, 45, 170, 185 },
            { 195, 200, 65, 210, 160, 70, 110, 250, 165, 30, 225, 160, 90, 125, 90, 45, 95, 125, 55, 100, 195, 85, 190, 165, 155, 175 }
        };

        public static Matrix<double> cost_mat = Matrix<double>.Build.DenseOfArray(cost_data);

        public static Object[,] machine_data = new Object[,] {
            { 0, 1021.765, 1.555, true, true, "shipper0", new double[,] { { 3183.101, 71082.842 },{ 113199.575, 192563.412 },{ 239971.291, 295233.494 }, }, },
            { 1, 1539.084, 1.296, true, true, "shipper1", new double[,] { { 3638.096, 75675.530 },{ 116222.010, 192507.980 },{ 224177.830, 305650.504 }, }, },
            { 2, 1597.688, 1.630, false, false, "notdedicated", new double[,] { { 4208.856, 74002.344 },{ 106314.299, 170138.492 },{ 216447.080, 280459.280 }, }, },
            { 3, 140.404, 2.037, false, false, "notdedicated", new double[,] { { 8654.554, 94711.791 },{ 126011.164, 179124.776 },{ 217330.221, 286487.334 }, }, },
            { 4, 442.710, 1.148, true, false, "notdedicated", new double[,] { { 11071.280, 90114.595 },{ 122957.872, 196411.766 },{ 236782.199, 310384.148 }, }, }
        };

        public static int num_machines = machine_data.GetLength(0);
        public static List<Machine> machines = InitMachines();

        public static Object[,] job_data = new Object[,] {
            { 0, 1287.025, 4253.472, false, false, "shipper4", 6 },
            { 1, 4346.025, 7404.464, true, true, "shipper1", 9 },
            { 2, 8350.025, 6058.133, false, false, "shipper4", 2 },
            { 3, 8594.025, 9386.501, true, false, "shipper4", 5 },
            { 4, 14682.025, 6744.874, true, false, "shipper4", 4 },
            { 5, 15902.025, 6345.223, true, true, "shipper1", 8 },
            { 6, 23279.025, 2281.944, false, false, "shipper2", 5 },
            { 7, 24265.025, 6954.949, false, false, "shipper3", 9 },
            { 8, 33002.025, 4619.411, false, false, "shipper5", 7 },
            { 9, 33626.025, 8943.464, true, true, "shipper0", 8 },
            { 10, 36473.025, 8976.287, false, false, "shipper4", 3 },
            { 11, 41583.025, 4018.562, false, false, "shipper3", 2 },
            { 12, 45087.025, 3621.942, true, true, "shipper0", 9 },
            { 13, 50700.025, 5122.337, true, false, "shipper2", 8 },
            { 14, 60305.025, 2900.655, true, true, "shipper0", 4 },
            { 15, 60429.025, 4741.652, false, false, "shipper3", 7 },
            { 16, 61706.025, 1995.294, true, false, "shipper4", 4 },
            { 17, 67136.025, 2493.787, false, false, "shipper3", 2 },
            { 18, 76834.025, 4208.187, true, true, "shipper1", 8 },
            { 19, 79821.025, 7113.741, false, false, "shipper2", 5 },
            { 20, 86155.025, 7706.675, true, true, "shipper1", 4 },
            { 21, 86592.025, 9914.908, false, false, "shipper3", 6 },
            { 22, 93973.025, 1635.131, false, false, "shipper4", 6 },
            { 23, 102380.025, 9630.437, false, false, "shipper4", 9 },
            { 24, 105650.025, 3152.833, false, false, "shipper4", 1 },
            { 25, 111888.025, 7543.468, false, false, "shipper5", 5 }
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

   
