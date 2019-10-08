using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Assignment
    {

        public int[,] costs;
        public int num_machines { get; private set; }
        public int num_jobs { get; private set; }

        private List<int> assignment;
        private Schedule schedule;

        public int populationSize = 200;
        public float mutationRate = 0.05f;
        public int elitism = 5;

        // Create an instance of GeneticAlgorithm class
        public GA ga;
        private Random random;

        // Constructor
        public Assignment(int[,] costs)
        {
            this.costs = costs;
            num_machines = costs.GetLength(0);
            num_jobs = costs.GetLength(1);
        }

        public void Start()
        {
            random = new Random();
            // Initialize ga instance
            ga = new GA(populationSize, num_jobs, random, GetRandomAssignment, FitnessFunction, elitism, mutationRate);
        }

        public void Update()
        {
            ga.NewGeneration();
        }

        private List<int> GetRandomAssignment()
        {

            // Array of n jobs, each element could be a worker index, i
            assignment = new List<int>(num_jobs);

            for (int j = 0; j < num_jobs; j++)
            {
                assignment.Add(random.Next(0, num_machines));
            }
            schedule = new Schedule(assignment);

            while (!schedule.isFeasible)
            {
                assignment.Clear();
                for (int j = 0; j < num_jobs; j++)
                {
                    assignment.Add(random.Next(0, num_machines));
                }
                schedule = new Schedule(assignment);
            }

            return schedule.assignment;
        }

        private int FitnessFunction(int index)
        {
            int fitness = 0;
            Chromosome chrmsm = ga.Population[index];

            // Calculate total cost
            for (int i = 0; i < num_jobs; i++)
            {
                fitness += costs[chrmsm.Genes[i], i];
            }

            return fitness;
        }


    }

}
