using System;
using System.Collections.Generic;


namespace GeneticAlgorithm
{
    public class GA
    {
        public List<Chromosome> Population { get; private set; }
        public int Generation { get; private set; }
        public int BestFitness { get; private set; }
        public List<int> BestGenes { get; private set; }

        public int Elitism;
        public float MutationRate;

        private List<Chromosome> newPopulation;
        private Random random;
        private int chromoSize;
        private Func<List<int>> getRandomGenes;
        private Func<int, int> fitnessFunction;

        // Constructor
        public GA(
            int populationSize,
            int chromoSize,
            Random random,
            Func<List<int>> getRandomGenes,
            Func<int, int> fitnessFunction,
            int elitism,
            float mutationRate
            )
        {
            Generation = 1;
            Elitism = elitism;
            MutationRate = mutationRate;
            Population = new List<Chromosome>(populationSize);
            newPopulation = new List<Chromosome>(populationSize);
            this.random = random;
            this.chromoSize = chromoSize;
            this.getRandomGenes = getRandomGenes;
            this.fitnessFunction = fitnessFunction;

            BestGenes = new List<int>(chromoSize);

            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new Chromosome(chromoSize, random, getRandomGenes, fitnessFunction, shouldInitGenes: true));
            }
        }

        public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
        {
            int finalCount = Population.Count + numNewDNA;
            
            if (finalCount <= 0)
            {
                return;
            }

            if (Population.Count > 0)
            {
                CalculateFitness();
                Population.Sort(CompareFitness);
                //Console.WriteLine("Best: " + Population[0].Fitness);
                //Console.WriteLine("Worst: " + Population[Population.Count - 1].Fitness);
                //for (int i = 0; i < Population.Count; i++)
                //{
                //    Console.WriteLine("Fitness = " + Population[i].Fitness);
                //}

            }

            // Deleted memory for eliminated individuals with poor fitness
            newPopulation.Clear();

            for (int i = 0; i < Population.Count; i++)
            {
                // Keep only top individuals of the previous generation
                if (i < Elitism && i < Population.Count)
                {
                    newPopulation.Add(Population[i]);
                }
                else if (i < Population.Count || crossoverNewDNA)
                {

                    Chromosome parent1 = ChooseParent();
                    Chromosome parent2 = ChooseParent();
                    Chromosome child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new Chromosome(chromoSize, random, getRandomGenes, fitnessFunction, shouldInitGenes: true));
                }
            }

            List<Chromosome> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;

            Generation++;
        }

        // TODO: implement minimize or maximize boolean argument
        private int CompareFitness(Chromosome a, Chromosome b)
        {
            // -1: second arg is greater
            //  1: first arg is greater
            //  0: equal

            if (a.Fitness < b.Fitness)
            {
                return -1;
            }
            else if (a.Fitness > b.Fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void CalculateFitness()
        {
            // initialize best individual to be the first element of the Population
            Chromosome best = Population[0];
            best.CalculateFitness(0);

            for (int i = 0; i < Population.Count; i++)
            {
                Population[i].CalculateFitness(i);

                if (Population[i].Fitness < best.Fitness)
                {
                    best = Population[i];
                }
            }

            BestFitness = best.Fitness;
            BestGenes = best.Genes;
        }

        private Chromosome ChooseParent()
        {
            // Tournoment selection
            int num_competitors = 2; // Binary tournoment
            Chromosome rand_selection;
            Chromosome best = null;

            for (int i = 0; i < num_competitors; i++)
            {
                rand_selection = Population[random.Next(0, Population.Count)];

                if (best == null || rand_selection.Fitness < best.Fitness)
                {
                    best = rand_selection;
                }
            }

            return best;
        }
    }

}
