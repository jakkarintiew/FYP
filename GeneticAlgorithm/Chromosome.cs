using System;
using System.Collections.Generic;


namespace GeneticAlgorithm
{
    public class Chromosome
    {
        // Gene is type int List
        public List<int> Genes { get; private set; }
        public Schedule schedule { get; private set; }

        // Fitness of each individual
        public int Fitness { get; private set; }

        private Random random;
        private Func<List<int>> getRandomGenes;
        private Func<int, int> fitnessFunction;

        // Construtor
        public Chromosome(
            int size,
            Random random,
            Func<List<int>> getRandomGenes,
            Func<int, int> fitnessFunction,
            bool shouldInitGenes = true)
        {
            // Create the Gene array with size
            Genes = new List<int>(size);
            this.random = random;
            this.getRandomGenes = getRandomGenes;
            this.fitnessFunction = fitnessFunction;

            if (shouldInitGenes)
            {
                Genes = getRandomGenes();
            }

        }

        public double CalculateFitness(int index)
        {
            Fitness = fitnessFunction(index);
            return Fitness;
        }

        // Crossover function
        public Chromosome Crossover(Chromosome otherParent)
        {
            // create new child Chromosome with same gene array size as parent; improve performance by setting shouldInitGenes: false
            Chromosome child = new Chromosome(Genes.Count, random, getRandomGenes, fitnessFunction, shouldInitGenes: true);

            double prob;
            int counter = 0;

            if (Genes == otherParent.Genes)
            {
                // might get stuck here as all individuals in population become the same
                child.Genes = Genes;
            }
            else
            //TODO: Ensure feasibility after crossover
            {
                for (int i = 0; i < Genes.Count; i++)
                {
                    if (Fitness < otherParent.Fitness)
                    {
                        // If parent 1 has better fitness   
                        // Higher probability to take gene from parent 1
                        prob = (double)otherParent.Fitness / (Fitness + otherParent.Fitness);
                        child.Genes[i] = random.NextDouble() < prob ? Genes[i] : otherParent.Genes[i];
                    }
                    else
                    {
                        prob = (double)Fitness / (Fitness + otherParent.Fitness);
                        child.Genes[i] = random.NextDouble() < prob ? Genes[i] : otherParent.Genes[i];
                    }
                }
            }

            schedule = new Schedule(child.Genes);

            while (!schedule.isFeasible)
            {
                if (Genes == otherParent.Genes)
                {
                    // might get stuck here as all individuals in population become the same
                    child.Genes = Genes;
                }
                else
                //TODO: Ensure feasibility after crossover
                {
                    for (int i = 0; i < Genes.Count; i++)
                    {
                        if (Fitness < otherParent.Fitness)
                        {
                            // If parent 1 has better fitness   
                            // Higher probability to take gene from parent 1
                            prob = (double)otherParent.Fitness / (Fitness + otherParent.Fitness);
                            child.Genes[i] = random.NextDouble() < prob ? Genes[i] : otherParent.Genes[i];
                        }
                        else
                        {
                            prob = (double)Fitness / (Fitness + otherParent.Fitness);
                            child.Genes[i] = random.NextDouble() < prob ? Genes[i] : otherParent.Genes[i];
                        }
                    }
                }
                
                schedule = new Schedule(child.Genes);
                counter++;

            }


            return child;
        }

        // Mutation function: simply get a random new gene
        public void Mutate(float mutationRate)
        {

            if (random.NextDouble() < mutationRate)
            {
                // Generate a random gene
                Genes = getRandomGenes();
            }

        }
    }

}
