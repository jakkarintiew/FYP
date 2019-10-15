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
            { 55, 225, 165, 125, 30, 175, 35, 105, 105, 210, 85, 90, 85, 145, 75 },
            { 215, 55, 100, 50, 225, 125, 105, 140, 250, 60, 235, 30, 120, 210, 120 },
            { 65, 120, 115, 245, 215, 175, 90, 190, 230, 45, 180, 100, 180, 200, 145 },
            { 175, 40, 150, 65, 145, 195, 125, 190, 80, 160, 190, 195, 150, 70, 200 },
            { 90, 190, 220, 90, 60, 145, 110, 25, 190, 165, 70, 140, 80, 225, 160 }
        };

        public static int num_machines = costs.GetLength(0);
        public static int num_jobs = costs.GetLength(1);
        public static List<Machine> machines = InitMachines();
        public static List<Job> jobs = InitJobs();

        // GA parameters
        public static int populationSize = 500;
        public static int numNewDNA = 50;
        public static float mutationRate = 0.05f;
        public static int elitism = 5;
        public static int crossoverMethod = 0;
        public enum objetiveFunction { TotalCost, Makespan, Combined }
        public static int objectiveCase = 1;

        // Set stopping conditions
        public static int solution = 0;
        public static int max_repeated_generations = 2000;
        public static int max_generations = 10000;

        public static List<Machine> InitMachines()
        {
            List<Machine> machines = new List<Machine>();

            Object[,] mat = new Object[,] {
                { 0, 30.278, 10.244 },
                { 1, 240.070, 9.007 },
                { 2, 50.247, 12.931 },
                { 3, 820.422, 15.663 },
                { 4, 700.663, 13.016 }
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
                { 0, 530.998, 999.757 },
                { 1, 549.879, 583.877 },
                { 2, 510.308, 405.793 },
                { 3, 24.755, 138.812 },
                { 4, 521.801, 113.318 },
                { 5, 183.044, 691.478 },
                { 6, 605.105, 281.543 },
                { 7, 936.982, 691.739 },
                { 8, 543.221, 424.241 },
                { 9, 257.745, 265.042 },
                { 10, 874.987, 463.953 },
                { 11, 176.743, 351.042 },
                { 12, 171.077, 289.984 },
                { 13, 494.485, 43.011 },
                { 14, 143.002, 459.483 }
            };

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                jobs.Add(new Job(index: (int)mat[i, 0], readyTime: (double)mat[i, 1], quantity: (double)mat[i, 2]));
            }

            return jobs.OrderBy(o => o.readyTime).ToList(); ;
        }

    }
}

   
