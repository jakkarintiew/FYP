﻿using System;
using System.Collections.Generic;


namespace GeneticAlgorithm
{
    public class GA
    {
        public List<Chromosome> Population { get; private set; }
        public int Generation { get; private set; }
        public double BestFitness { get; private set; }
        public List<int> BestGenes { get; private set; }

        public int Elitism;
        public float MutationRate;

        private List<Chromosome> newPopulation;
        private Random random;
        private int chromoSize;
        


        // Constructor
        public GA()
        {
            Generation = 1;
            Elitism = Data.elitism;
            MutationRate = Data.mutationRate;
            Population = new List<Chromosome>(Data.populationSize);
            newPopulation = new List<Chromosome>(Data.populationSize);
            random = new Random();
            chromoSize = Data.num_jobs;

            BestGenes = new List<int>(chromoSize);

            for (int i = 0; i < Data.populationSize; i++)
            {
                Population.Add(new Chromosome(chromoSize, random, GetRandomGenes, FitnessFunction, shouldInitGenes: true));
            }
        }

        public void NewGeneration()
        {
            int numNewDNA = Data.numNewDNA;
            bool crossoverNewDNA = false;
            int finalCount = Data.populationSize + numNewDNA;
            
            if (finalCount <= 0)
            {
                return;
            }

            if (Population.Count > 0)
            {
                CalculateAllFitness();
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

            for (int i = 0; i < finalCount; i++)
            {
                // Keep only top individuals of the previous generation
                if (i < Elitism && i < Data.populationSize)
                {
                    newPopulation.Add(Population[i]);
                }
                else if (i < Data.populationSize || crossoverNewDNA)
                {

                    Chromosome parent1 = ChooseParent();
                    Chromosome parent2 = ChooseParent();
                    Chromosome child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new Chromosome(chromoSize, random, GetRandomGenes, FitnessFunction, shouldInitGenes: true));
                }
            }

            List<Chromosome> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;

            Generation++;
        }

        private List<int> GetRandomGenes()
        {

            // Array of n jobs, each element could be a worker index, i
            List<int> assignment = new List<int>(chromoSize);
            Schedule schedule = null;

            while (schedule == null || !schedule.isFeasible)
            {
                assignment.Clear();
                for (int j = 0; j < chromoSize; j++)
                {
                    assignment.Add(random.Next(0, Data.num_machines));
                }
                schedule = new Schedule(assignment);
                //Console.WriteLine(schedule.isFeasible);
            }

            return schedule.assignment;
        }

        private double FitnessFunction(int index)
        {
            double fitness = 0;
            Chromosome chrmsm = Population[index];
            Schedule schedule = new Schedule(chrmsm.Genes);

            Data.objetiveFunction objetive = (Data.objetiveFunction) Data.objectiveCase;

            switch (objetive)
            {
                case Data.objetiveFunction.TotalCost:
                    fitness = schedule.cost;
                    break;
                case Data.objetiveFunction.Makespan:
                    fitness = schedule.makespan;
                    break;
                case Data.objetiveFunction.Combined:
                    fitness = Math.Pow(schedule.cost, 3) + Math.Pow(schedule.makespan, 3);
                    break;
            }

            return fitness;
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

        public void CalculateAllFitness()
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
