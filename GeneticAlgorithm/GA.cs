using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;
using MathNet.Numerics.LinearAlgebra;
using GeneticAlgorithm.Operators.Crossovers;
using GeneticAlgorithm.Operators.Mutations;
using GeneticAlgorithm.Operators.ParentSelection;

namespace GeneticAlgorithm
{
    public class GA
    {
        public List<Chromosome> Population { get; private set; }
        public SelectionBase Selection { get; set; }
        public CrossoverBase Crossover { get; set; }
        public MutationBase Mutation { get; set; }
        public int Generation { get; private set; }
        public double BestFitness { get; private set; }
        public List<int> BestGenes { get; private set; }
        public Chromosome BestChromosome { get; private set; }

        public int Elitism;
        public double MutationRate;
        private Mcg59 Random;
        private int ChromoSize;

        // Constructor
        public GA(SelectionBase selection, CrossoverBase crossover, MutationBase mutation)
        {
            Generation = 1;
            Elitism = Settings.Elitism;
            MutationRate = Settings.MutationRate;
            Population = new List<Chromosome>(Settings.PopulationSize);
            Random = new Mcg59(RandomSeed.Robust());
            ChromoSize = Settings.NumAllMachines;
            BestGenes = new List<int>(ChromoSize);
            Selection = selection;
            Crossover = crossover;
            Mutation = mutation;

            for (int i = 0; i < Settings.PopulationSize; i++)
            {
                Population.Add(new Chromosome(ChromoSize, shouldInitGenes: true));
            }

        }

        public void NewGeneration()
        {
            int finalCount = Settings.PopulationSize + Settings.NumNewDNA;
            List<Chromosome> newPopulation = new List<Chromosome>();

            if (finalCount <= 0)
            {
                return;
            }

            if (Population.Count > 0)
            {
                CalculateAllFitness();
                Population = Population.OrderBy(chrmsm => chrmsm.Fitness).ToList();
            }

            for (int i = 0; i < finalCount; i++)
            {
                // Keep only top individuals of the previous generation
                if (i < Elitism && i < Settings.PopulationSize)
                {
                    newPopulation.Add(Population[i]);
                    //Console.WriteLine("Generation: " + Generation);
                    //Console.WriteLine("/n");
                    //Console.WriteLine("genes: [{0}]", string.Join(",\t", Population[i].genes));
                    //Console.WriteLine(Population[i].fitness);
                }
                else if (i < Settings.PopulationSize)
                {

                    Chromosome parent1 = Selection.PerformSelection(Population);
                    Chromosome parent2 = Selection.PerformSelection(Population);
                    Chromosome child = Crossover.PerformCrossover (parent1, parent2);
                    child = Mutation.PerformMutation(child);
                    newPopulation.Add(child);

                }
                else if (Settings.EnableCrossoverNewDNA)
                {
                    newPopulation.Add(new Chromosome(ChromoSize, shouldInitGenes: true));
                }
            }

            Population = newPopulation;
            Generation++;
        }

        public void CalculateAllFitness()
        {
            // initialize best individual to be the first element of the Population
            Chromosome best = Population[0];
            best.CalculateFitness();

            for (int i = 0; i < Population.Count; i++)
            {
                Population[i].CalculateFitness();

                if (Population[i].Fitness <= best.Fitness)
                {
                    best = Population[i];
                }
            }

            BestFitness = best.Fitness;
            BestGenes = best.Genes;
            BestChromosome = best;
        }
    }

}
