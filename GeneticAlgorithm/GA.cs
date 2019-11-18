using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;
using MathNet.Numerics.LinearAlgebra;

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
        private Mcg59 random;
        //private random random;
        private int chromoSize;
        


        // Constructor
        public GA()
        {
            Generation = 1;
            Elitism = Data.elitism;
            MutationRate = Data.mutationRate;
            Population = new List<Chromosome>(Data.populationSize);
            newPopulation = new List<Chromosome>(Data.populationSize);

            int someRobustSeed = RandomSeed.Robust();
            random = new Mcg59(someRobustSeed);

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
                    Chromosome child = Crossover(parent1, parent2);

                    child = Mutate(child, MutationRate);

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


        private int ProbabilityMachineSelection(Vector<double> target_column)
        {
            var machine_index = Enumerable.Range(0, Data.num_machines).ToList();
            Vector<double> transform_column = target_column.Sum() / target_column;
            Vector<double> prob_vector  = transform_column / transform_column.Sum();
            Dictionary<int, double>  dict = machine_index.Zip(prob_vector, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            double randDouble = random.NextDouble();
            double cumulative = 0.0;

            for (int i = 0; i < dict.Count; i++)
            {
                cumulative += dict[dict.Keys.ElementAt(i)];
                if (randDouble < cumulative)
                {
                    int selectedElement = dict.Keys.ElementAt(i);
                    //Console.WriteLine("randDouble: {0}", randDouble);
                    return selectedElement;
                }
            }

            return -1;
        }

        private List<int> GetRandomGenes()
        {

            // Array of n jobs, each element could be a worker index, i
            List<int> assignment = new List<int>(new int[Data.num_jobs]);
            Scheduler schedule = new Scheduler();

            Data.objetiveFunction objetiveFunction = (Data.objetiveFunction)Data.objectiveCase;
            Data.dedicationType dedicationType = (Data.dedicationType)Data.dedicationCase;

            Vector<double> target_column = Vector<double>.Build.Dense(Data.num_machines);
            assignment.Clear();

            for (int j = 0; j < Data.num_jobs; j++)
            {
                switch (objetiveFunction)
                {
                    case Data.objetiveFunction.TotalCost:
                        target_column = Data.cost_mat.Column(j);
                        break;
                    case Data.objetiveFunction.Makespan:
                        target_column = Vector<double>.Abs(Vector<double>.Build.Dense(Data.machines.Select(x => x.readyTime).ToArray()) - Data.jobs[j].readyTime);
                        break;
                }

                bool isFeasible = false;
                while (!isFeasible)
                {
                    int indexSelectedMachine = ProbabilityMachineSelection(target_column);

                    if (schedule.IsFeasible(schedule.machines[indexSelectedMachine], schedule.jobs[j]))
                    {

                    }

                    if (schedule.IsGearFeasible(schedule.machines[indexSelectedMachine], schedule.jobs[j]))
                    {
                        switch (dedicationType)
                        {
                            case Data.dedicationType.Felxible:
                                if (schedule.IsFlexDedicationFeasible(schedule.machines[indexSelectedMachine], schedule.jobs[j]))
                                {
                                    assignment.Add(indexSelectedMachine);
                                    isFeasible = true;
                                }
                                break;
                            case Data.dedicationType.Strict:
                                if (schedule.IsStrictDedicationFeasible(schedule.machines[indexSelectedMachine], schedule.jobs[j]))
                                {
                                    assignment.Add(indexSelectedMachine);
                                    isFeasible = true;
                                }
                                break;
                        }

                    }
                }
            }

            schedule.assignment = assignment;
            schedule.GetSchedule(assignment);
            return assignment;
        }

        private double FitnessFunction(int index)
        {
            double fitness = 0;
            Chromosome chrmsm = Population[index];
            Scheduler schedule = new Scheduler();
            schedule.GetSchedule(chrmsm.Genes);

            Data.objetiveFunction objetive = (Data.objetiveFunction) Data.objectiveCase;

            switch (objetive)
            {
                case Data.objetiveFunction.TotalCost:
                    fitness = schedule.cost;
                    break;
                case Data.objetiveFunction.Makespan:
                    fitness = schedule.makespan;
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

                if (Population[i].Fitness <= best.Fitness)
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

        // Crossover function
        public Chromosome Crossover(Chromosome firstParent, Chromosome secondParent)
        {
            // create new child Chromosome with same gene array size as parent; improve performance by setting shouldInitGenes: false
            Chromosome child = new Chromosome(firstParent.Genes.Count, random, GetRandomGenes, FitnessFunction, shouldInitGenes: false);

            Data.crossoverFunction crossoverMethod = (Data.crossoverFunction)Data.crossoverMethod;

            if (firstParent.Genes == secondParent.Genes)
            {
                child.Genes = firstParent.Genes;
            }
            else
            {
                switch (crossoverMethod)
                {
                    case Data.crossoverFunction.Uniform:
                        double prob;
                        for (int i = 0; i < firstParent.Genes.Count; i++)
                        {
                            if (firstParent.Fitness < secondParent.Fitness)
                            {
                                // If parent 1 has better fitness   
                                // Higher probability to take gene from parent 1
                                prob = secondParent.Fitness / (firstParent.Fitness + secondParent.Fitness);
                                child.Genes[i] = random.NextDouble() < prob ? firstParent.Genes[i] : secondParent.Genes[i];
                            }
                            else
                            {
                                prob = firstParent.Fitness / (firstParent.Fitness + secondParent.Fitness);
                                child.Genes[i] = random.NextDouble() < prob ? firstParent.Genes[i] : secondParent.Genes[i];
                            }
                        }
                        break;

                    case Data.crossoverFunction.SinglePoint:
                        if (firstParent.Fitness < secondParent.Fitness)
                        {
                            int crossOverPoint = random.Next(firstParent.Genes.Count / 2);
                            child.Genes = firstParent.Genes.Take(crossOverPoint).Concat(secondParent.Genes.Skip(crossOverPoint)).ToList<int>();
                        }
                        else
                        {
                            int crossOverPoint = random.Next(firstParent.Genes.Count / 2, firstParent.Genes.Count - 1);
                            child.Genes = firstParent.Genes.Take(crossOverPoint).Concat(secondParent.Genes.Skip(crossOverPoint)).ToList<int>();
                        }
                        break;
                    case Data.crossoverFunction.TwoPoint:
                        int firstCrossOverPoint = random.Next(firstParent.Genes.Count / 2);
                        int secondCrossOverPoint = random.Next(firstCrossOverPoint, firstParent.Genes.Count);
                        child.Genes = firstParent.Genes.Take(firstCrossOverPoint).Concat(secondParent.Genes.Skip(firstCrossOverPoint).Take(secondCrossOverPoint - firstCrossOverPoint)).Concat(firstParent.Genes.Skip(secondCrossOverPoint)).ToList<int>();
                        break;
                }

            }

            child.schedule = new Scheduler();
            child.schedule.GetSchedule(child.Genes);
            return child;
        }

        // Mutation function: simply get a random new gene
        public Chromosome Mutate(Chromosome chromosome, float mutationRate)
        {
            if (random.NextDouble() < mutationRate)
            {
                // Generate a random gene
                chromosome.Genes = GetRandomGenes();
            }

            return chromosome;

        }
    }

}
