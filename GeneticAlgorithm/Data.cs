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
            { 100, 55, 155, 160, 195, 175, 130, 35, 185, 245, 95, 195, 125, 145, 45 },
            { 40, 25, 65, 160, 175, 245, 35, 230, 205, 245, 55, 50, 235, 30, 35 },
            { 210, 65, 245, 100, 25, 80, 190, 180, 80, 120, 135, 110, 155, 190, 25 },
            { 200, 35, 175, 205, 150, 140, 85, 235, 235, 175, 200, 120, 235, 150, 195 },
            { 165, 90, 165, 135, 65, 185, 40, 145, 185, 215, 235, 250, 215, 250, 190 }
        };

        public static Matrix<double> cost_mat = Matrix<double>.Build.DenseOfArray(cost_data);

        public static Object[,] machine_data = new Object[,] {
            { 0, 460.000, 9.655,  true,  false, "notdedicated", new double[,] { { 90.307, 103.063 }, }, },
            { 1, 300.741, 14.544, true,  true,  "shipper1",     new double[,] { { 1388.712, 1820.366 },{ 241.304, 259.060 },{ 272.367, 291.568 }, }, },
            { 2, 440.405, 5.228,  true,  true,  "shipper2",     new double[,] { { 4151.996, 4257.232 },{ 538.355, 554.802 }, }, },
            { 3, 650.204, 12.630, true,  false, "notdedicated", new double[,] { { 6111.380, 6701.792 }, }, },
            { 4, 700.747, 5.113,  false, false, "notdedicated", new double[,] { { 9875.418, 10861.447 },{ 376.868, 393.852 }, }, }
        };

        public static int num_machines = machine_data.GetLength(0);
        public static List<Machine> machines = InitMachines();

        public static Object[,] job_data = new Object[,] {
            { 0, 709.932, 967.432,  true,   false,  "shipper3" },
            { 1, 541.507, 147.008,  true,   false,   "shipper0" },
            { 2, 483.067, 852.524,  true,   false,   "shipper0" },
            { 3, 167.112, 424.952,  false,  false,   "shipper0" },
            { 4, 348.245, 401.542,  true,   true,   "shipper2" },
            { 5, 526.744, 326.555,  false,  false,  "shipper3" },
            { 6, 109.975, 99.873,   true,   false,  "shipper3" },
            { 7, 293.098, 784.406,  true,   true,   "shipper1" },
            { 8, 674.455, 685.312,  true,   true,   "shipper2" },
            { 9, 912.865, 275.880,  false,  true,   "shipper1" },
            { 10, 725.525, 289.031, false,  true,   "shipper2" },
            { 11, 672.642, 690.041, true,   false,  "shipper4" },
            { 12, 266.193, 995.992, true,   false,  "shipper4" },
            { 13, 976.884, 87.941,  true,   false,  "shipper4" },
            { 14, 452.973, 393.811, false,  false,  "shipper3" },
        };
        public static int num_jobs = job_data.GetLength(0);
        public static List<Job> jobs = InitJobs();

        // GA parameters
        public static int populationSize = 500;
        public static int numNewDNA = 100;
        public static float mutationRate = 0.05f;
        public static int elitism = 10;

        public enum crossoverFunction { Uniform, SinglePoint, TwoPoint}
        public static int crossoverMethod = 0;
        public enum objetiveFunction { TotalCost, Makespan }
        public static int objectiveCase = 1;

        // Set stopping conditions
        public static double solution = 890;
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
                    dedicated: (string)machine_data[i, 5],
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
                    shipper: (string)job_data[i, 5]
                    ));
            }
            return jobs.OrderBy(o => o.readyTime).ToList(); ;
        }

    }
}

   
