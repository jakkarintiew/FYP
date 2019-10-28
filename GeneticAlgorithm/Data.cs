using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public static class Data
    {
        // Problem static data
        public static int[,] costs = 
        {
            { 100, 55, 155, 160, 195, 175, 130, 35, 185, 245, 95, 195, 125, 145, 45 },
            { 40, 25, 65, 160, 175, 245, 35, 230, 205, 245, 55, 50, 235, 30, 35 },
            { 210, 65, 245, 100, 25, 80, 190, 180, 80, 120, 135, 110, 155, 190, 25 },
            { 200, 35, 175, 205, 150, 140, 85, 235, 235, 175, 200, 120, 235, 150, 195 },
            { 165, 90, 165, 135, 65, 185, 40, 145, 185, 215, 235, 250, 215, 250, 190 }
        };

        public static int num_machines = costs.GetLength(0);
        public static int num_jobs = costs.GetLength(1);
        public static List<Machine> machines = InitMachines();
        public static List<Job> jobs = InitJobs();

        // GA parameters
        public static int populationSize = 200;
        public static int numNewDNA = 40;
        public static float mutationRate = 0.10f;
        public static int elitism = 10;

        public enum crossoverFunction { Uniform, SinglePoint, TwoPoint}
        public static int crossoverMethod = 0;
        public enum objetiveFunction { TotalCost, Makespan, Combined }
        public static int objectiveCase = 0;

        // Set stopping conditions
        public static int solution = 0;
        public static int max_repeated_generations = 100;
        public static int max_generations = 200;

        public static List<Machine> InitMachines()
        {
            List<Machine> machines = new List<Machine>();

            Object[,] mat = new Object[,] {
                { 0, 220.716, 6.462 },
                { 1, 310.092, 10.059 },
                { 2, 620.403, 11.543 },
                { 3, 600.032, 11.756 },
                { 4, 320.725, 14.207 }
            };

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                machines.Add(new Machine(index: (int)mat[i, 0], readyTime: (double)mat[i, 1], procRate: (double)mat[i, 2]));
            }

            return machines;
        }

        public static List<Job> InitJobs()
        {
            List<Job> jobs = new List<Job>();

            Object[,] mat = new Object[,] {
                { 0, 561.257, 458.844 },
                { 1, 89.907, 546.079 },
                { 2, 61.677, 871.440 },
                { 3, 183.935, 882.870 },
                { 4, 675.026, 293.000 },
                { 5, 604.260, 538.616 },
                { 6, 738.316, 862.209 },
                { 7, 343.078, 586.738 },
                { 8, 144.777, 301.003 },
                { 9, 892.413, 680.100 },
                { 10, 361.408, 537.685 },
                { 11, 412.792, 120.888 },
                { 12, 257.113, 419.330 },
                { 13, 961.996, 501.080 },
                { 14, 879.388, 877.565 }
            };

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                jobs.Add(new Job(index: (int)mat[i, 0], readyTime: (double)mat[i, 1], quantity: (double)mat[i, 2]));
            }

            return jobs.OrderBy(o => o.readyTime).ToList(); ;
        }

    }
}

   
