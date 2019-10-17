using System;
using System.Collections.Generic;
using System.Linq;


namespace GeneticAlgorithm
{
    public class Chromosome
    {
        // Gene is type int List
        public List<int> Genes { get; private set; }
        public Schedule schedule { get; private set; }

        // Fitness of each individual
        public double Fitness { get; private set; }

        private Random random;
        private Func<List<int>> getRandomGenes;
        private Func<int, double> fitnessFunction;

        // Construtor
        public Chromosome(
            int size,
            Random random,
            Func<List<int>> getRandomGenes,
            Func<int, double> fitnessFunction,
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
            Data.crossoverFunction crossoverMethod = (Data.crossoverFunction)Data.crossoverMethod;

            if (Genes == otherParent.Genes)
            {
                child.Genes = Genes;
            }
            else
            {
                switch (crossoverMethod)
                {
                    case Data.crossoverFunction.Uniform:
                        double prob;
                        for (int i = 0; i < Genes.Count; i++)
                        {
                            if (Fitness < otherParent.Fitness)
                            {
                                // If parent 1 has better fitness   
                                // Higher probability to take gene from parent 1
                                prob = otherParent.Fitness / (Fitness + otherParent.Fitness);
                                child.Genes[i] = random.NextDouble() < prob ? Genes[i] : otherParent.Genes[i];
                            }
                            else
                            {
                                prob = Fitness / (Fitness + otherParent.Fitness);
                                child.Genes[i] = random.NextDouble() < prob ? Genes[i] : otherParent.Genes[i];
                            }
                        }
                        break;
                    case Data.crossoverFunction.SinglePoint:
                        if (Fitness < otherParent.Fitness)
                        {
                            int crossOverPoint = random.Next(Genes.Count / 2);
                            child.Genes = Genes.Take(crossOverPoint).Concat(otherParent.Genes.Skip(crossOverPoint)).ToList<int>();
                        }
                        else
                        {
                            int crossOverPoint = random.Next(Genes.Count / 2, Genes.Count - 1);
                            child.Genes = Genes.Take(crossOverPoint).Concat(otherParent.Genes.Skip(crossOverPoint)).ToList<int>();
                        }
                        break;
                    case Data.crossoverFunction.TwoPoint:
                        int firstCrossOverPoint = random.Next(Genes.Count / 2);
                        int secondCrossOverPoint = random.Next(firstCrossOverPoint, Genes.Count);
                        child.Genes = Genes.Take(firstCrossOverPoint).Concat(otherParent.Genes.Skip(firstCrossOverPoint).Take(secondCrossOverPoint- firstCrossOverPoint)).Concat(Genes.Skip(secondCrossOverPoint)).ToList<int>();
                        break;
                }

            }
 
            schedule = new Schedule(child.Genes);
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
