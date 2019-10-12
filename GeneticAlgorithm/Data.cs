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
        public static int populationSize = 200;
        public static float mutationRate = 0.05f;
        public static int elitism = 5;
        public static int crossoverMethod = 0;
        public enum objetiveFunction { TotalCost, MakeSpan, Combined }
        public static int objectiveCase = 1;

        public static List<Machine> InitMachines()
        {
            List<Machine> machines = new List<Machine>();

            Object[,] mat = new Object[,] {
                { 0, 0.00, 5.00 },
                { 1, 340.00, 7.00 },
                { 2, 910.00, 12.00 },
                { 3, 340.00, 6.00 },
                { 4, 400.00, 14.00 }
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
                { 0, 0.00, 150.00 },
                { 1, 130.00, 530.00 },
                { 2, 1090.00, 380.00 },
                { 3, 1670.00, 470.00 },
                { 4, 1690.00, 110.00 },
                { 5, 2650.00, 90.00 },
                { 6, 2820.00, 130.00 },
                { 7, 3270.00, 690.00 },
                { 8, 4070.00, 630.00 },
                { 9, 5010.00, 30.00 },
                { 10, 5580.00, 470.00 },
                { 11, 5710.00, 350.00 },
                { 12, 6130.00, 210.00 },
                { 13, 6730.00, 920.00 },
                { 14, 7070.00, 540.00 }
            };

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                jobs.Add(new Job(index: (int)mat[i, 0], readyTime: (double)mat[i, 1], quantity: (double)mat[i, 2]));
            }

            return jobs;
        }

    }
}

   
