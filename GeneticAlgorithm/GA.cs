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
        public Chromosome BestChromosome { get; private set; }

        public int Elitism;
        public float MutationRate;
        private Mcg59 random;
        private int chromoSize;

        // Constructor
        public GA()
        {
            Generation = 1;
            Elitism = Data.Elitism;
            MutationRate = Data.MutationRate;
            Population = new List<Chromosome>(Data.PopulationSize);
            random = new Mcg59(RandomSeed.Robust());
            chromoSize = Data.NumAllMachines;
            BestGenes = new List<int>(chromoSize);


            for (int i = 0; i < Data.PopulationSize; i++)
            {
                //Console.WriteLine("IM HERE");
                Population.Add(new Chromosome(chromoSize, GetRandomGenes, shouldInitGenes: true));
            }

        }

        public void NewGeneration()
        {
            int numNewDNA = Data.NumNewDNA;
            bool crossoverNewDNA = true;
            int finalCount = Data.PopulationSize + numNewDNA;
            List<Chromosome> newPopulation = new List<Chromosome>();

            if (finalCount <= 0)
            {
                return;
            }

            if (Population.Count > 0)
            {
                CalculateAllFitness();
                Population.Sort(CompareFitness);
            }

            for (int i = 0; i < finalCount; i++)
            {
                // Keep only top individuals of the previous generation
                if (i < Elitism && i < Data.PopulationSize)
                {
                    newPopulation.Add(Population[i]);
                    //Console.WriteLine("Generation: " + Generation);
                    //Console.WriteLine("/n");
                    //Console.WriteLine("genes: [{0}]", string.Join(",\t", Population[i].genes));
                    //Console.WriteLine(Population[i].fitness);
                }
                else if (i < Data.PopulationSize || crossoverNewDNA)
                {

                    Chromosome parent1 = ChooseParent();
                    Chromosome parent2 = ChooseParent();
                    Chromosome child = Crossover(parent1, parent2);
                    child = Mutate(child, MutationRate);

                    newPopulation.Add(child);

                }
                else
                {
                    newPopulation.Add(new Chromosome(chromoSize, GetRandomGenes, shouldInitGenes: true));
                }
            }

            Population = newPopulation;
            Generation++;
        }


        private int ProbabilityMachineSelection(Vector<double> randSelectionColumn)
        {
            var machine_index = Enumerable.Range(0, Data.NumAllMachines).ToList();
            Vector<double> transform_column = randSelectionColumn.Sum() / (randSelectionColumn + 1);
            Vector<double> prob_vector = transform_column / transform_column.Sum();
            Dictionary<int, double> dict = machine_index.Zip(prob_vector, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            double randDouble = random.NextDouble();
            double cumulative = 0.0;


            for (int i = 0; i < dict.Count; i++)
            {
                cumulative += dict[dict.Keys.ElementAt(i)];
                if (randDouble < cumulative)
                {
                    int selectedElement = dict.Keys.ElementAt(i);
                    //Console.WriteLine("selectedElement: {0}\n", selectedElement);
                    return selectedElement;
                }
            }

            return -1;
        }

        private List<int> GetRandomGenes()
        {
            Restart:
            Scheduler schedule = new Scheduler();

            Vector<double> randSelectionColumn = Vector<double>.Build.Dense(Data.NumAllMachines);
            bool isFeasible = false;
            int counter = 0;

            // If IsAllMachinesUtilized, randomly assigned one job to each compulsary machine
            if (Data.IsAllMachinesUtilized)
            {
                foreach (Machine machine in schedule.machines) 
                {
                    if (!machine.isThirdParty || machine.isCompulsary)
                    {
                        
                        int randJobIndex = random.Next(Data.NumJobs);
                        if (schedule.jobs[randJobIndex].assignedMachine == null)
                        {
                            isFeasible = false;
                            counter = 0;
                            while (!isFeasible)
                            {
                                if (schedule.IsFeasible(machine, schedule.jobs[randJobIndex]))
                                {
                                    schedule.Assign(machine, schedule.jobs[randJobIndex]);
                                    schedule.ScheduleToGenes();
                                    isFeasible = true;

                                }
                                else
                                {
                                    counter++;
                                    if (counter > 50)
                                    {
                                        goto Restart;
                                    }
                                }
                            }
                        }
                    }
                    //Console.WriteLine(counter);
                    //Console.WriteLine(isFeasible);
                }
            }

            for (int j = 0; j < schedule.jobs.Count; j++)
            {
                if (schedule.jobs[j].assignedMachine == null)
                {
                    isFeasible = false;
                    counter = 0;

                    for (int i = 0; i < schedule.machines.Count; i++)
                    {
                        randSelectionColumn[i] = schedule.CalculateIncrementalFitness(schedule.machines[i], schedule.jobs[j]);
                    }

                    while (!isFeasible)
                    {

                        int indexSelectedMachine = ProbabilityMachineSelection(randSelectionColumn);

                        if (!schedule.IsFeasible(schedule.machines[indexSelectedMachine], schedule.jobs[j]))
                        {
                            counter++;
                            randSelectionColumn[indexSelectedMachine] = int.MaxValue;
                        }
                        else
                        {
                            schedule.Assign(schedule.machines[indexSelectedMachine], schedule.jobs[j]);
                            isFeasible = true;
                        }
                    }

                    //Console.WriteLine(counter);
                    //Console.WriteLine(isFeasible);
                }
            }

            schedule.ScheduleToGenes();
            //Console.WriteLine(schedule.IsOverallFeasible());
            //Console.WriteLine("scheduleToGenes: [ {0} ]", string.Join(",", schedule.genes));

            return schedule.genes;
        }

        // TODO: implement minimize or maximize boolean argument
        private int CompareFitness(Chromosome a, Chromosome b)
        {
            // -1: second arg is greater
            //  1: first arg is greater
            //  0: equal

            if (a.fitness < b.fitness)
            {
                return -1;
            }
            else if (a.fitness > b.fitness)
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
            best.CalculateFitness();

            for (int i = 0; i < Population.Count; i++)
            {
                Population[i].CalculateFitness();

                if (Population[i].fitness <= best.fitness)
                {
                    best = Population[i];
                }
            }

            BestFitness = best.fitness;
            BestGenes = best.genes;
            BestChromosome = best;
        }

        private Chromosome ChooseParent()
        {
            // Tournament selection
            int numCompetitors = 2; // Binary tournament
            Chromosome randSelection;
            Chromosome best = null;

            for (int i = 0; i < numCompetitors; i++)
            {
                randSelection = Population[random.Next(0, Population.Count)];

                if (best == null || randSelection.fitness < best.fitness)
                {
                    best = randSelection;
                }
            }

            return best;
        }

        public int NextUnrepeatedElem(List<int> firstList, List<int> secondList)
        {
            int idx = 0;
            while (firstList.Contains(secondList[idx]))
            {
                idx++;
            }
            return secondList[idx];
        }


        // Crossover function
        public Chromosome Crossover(Chromosome firstParent, Chromosome secondParent)
        {

            Chromosome child = new Chromosome(firstParent.genes.Count, GetRandomGenes, shouldInitGenes: false);

            Data.CrossoverFunction crossoverMethod = (Data.CrossoverFunction)Data.CrossoverMethod;

            if (firstParent.genes == secondParent.genes)
            {
                child.genes = firstParent.genes;
            }
            else
            {
                bool isFeasible = false;
                int counter = 0;
                while (!isFeasible)
                {
                    // create new child Chromosome with same gene array size as parent; improve performance by setting shouldInitGenes: false
                    Chromosome firstChild = new Chromosome(firstParent.genes.Count, GetRandomGenes, shouldInitGenes: false);
                    Chromosome secondChild = new Chromosome(firstParent.genes.Count, GetRandomGenes, shouldInitGenes: false);

                    counter++;

                    if (counter > 100)
                    {
                        //Console.WriteLine("wooow");
                        child.genes = GetRandomGenes();
                        child.CalculateFitness();
                        break;
                    }

                    // Position Based Crossover (PBC)
                    List<int> positionProfile = new List<int>(new int[firstParent.genes.Count]);
                    double prob;
                    for (int i = 0; i < firstParent.genes.Count; i++)
                    {
                        prob = random.NextDouble();
                        if (prob < secondParent.fitness / (firstParent.fitness + secondParent.fitness))
                        {
                            positionProfile[i] = 1;
                        }
                    }

                    for (int i = 0; i < positionProfile.Count; i++)
                    {
                        if (positionProfile[i] == 1)
                        {
                            firstChild.genes[i] = firstParent.genes[i];
                        }
                        else
                        {
                            secondChild.genes[i] = secondParent.genes[i];
                        }
                    }

                    for (int i = 0; i < positionProfile.Count; i++)
                    {
                        if (positionProfile[i] == 1)
                        {
                            secondChild.genes[i] = NextUnrepeatedElem(secondChild.genes, firstParent.genes);
                        }
                        else
                        {
                            firstChild.genes[i] = NextUnrepeatedElem(firstChild.genes, secondParent.genes);
                        }
                    }

                    firstChild.MakeProperGenes();
                    secondChild.MakeProperGenes();
                    firstChild.CalculateFitness();
                    secondChild.CalculateFitness();

                    if (CompareFitness(firstChild, secondChild) == 1)
                    {
                        child = secondChild;
                    }
                    else
                    {
                        child = firstChild;
                    }

                    isFeasible = child.schedule.IsOverallFeasible();

                }

            }

            return child;
        }

        // Mutation function: simply get a random new gene
        public Chromosome Mutate(Chromosome chromosome, float mutationRate)
        {

            Chromosome mutated = new Chromosome(chromosome.genes.Count, GetRandomGenes, shouldInitGenes: true);
            mutated.genes = chromosome.genes;
            mutated.CalculateFitness();

            //if (random.NextDouble() < mutationRate)
            //{
            //    while (!isFeasible)
            //    {
            //        // Generate a random gene
            //        mutated.genes = GetRandomGenes();
            //        mutated.MakeProperGenes();
            //        mutated.CalculateFitness();
            //        isFeasible = mutated.schedule.IsOverallFeasible();
            //    }
            //}

            //if (random.NextDouble() < mutationRate)
            //{
            //    while (!isFeasible)
            //    {
            //        int randJob = random.Next(0, mutated.schedule.jobs.Count);
            //        int randPosition = random.Next(0, mutated.genes.Count);

            //        mutated.genes.Remove(randJob);
            //        mutated.genes.Insert(randPosition, randJob);
            //        mutated.MakeProperGenes();
            //        mutated.CalculateFitness();
            //        isFeasible = mutated.schedule.IsOverallFeasible();

            //        //if (mutated.schedule.IsOverallFeasible())
            //        //{
            //        //    Console.WriteLine(mutated.schedule.IsOverallFeasible());
            //        //    Console.WriteLine("before mutation genes:  [ {0} ]", string.Join(",", chromosome.genes));
            //        //    Console.WriteLine("after mutation genes :  [ {0} ]\n\n", string.Join(",", mutated.genes));

            //        //    Printer printer = new Printer();
            //        //    printer.PrintSchedule(mutated.schedule);
            //        //}
            //    }

            //}

            if (random.NextDouble() <= 1)
            {
                int randPosition = random.Next(0, mutated.genes.Count);
                int randMachine = random.Next(0, mutated.schedule.machines.Count);

                if (mutated.genes[randPosition] > 100)
                {
                    return chromosome;
                }
                else
                {
                    int randJob = mutated.genes[randPosition];
                    int randAssignedJobsPoisition = random.Next(0, mutated.schedule.machines[randMachine].assignedJobs.Count + 1);
                    int shiftingEmptySlot = mutated.genes[randMachine * mutated.schedule.jobs.Count + mutated.schedule.machines[randMachine].assignedJobs.Count];
                    mutated.genes.Remove(randJob);
                    mutated.genes.Insert(randMachine * mutated.schedule.jobs.Count + randAssignedJobsPoisition, randJob);
                    mutated.genes.Remove(shiftingEmptySlot);
                    mutated.genes.Insert(randPosition, shiftingEmptySlot);
                    mutated.MakeProperGenes();
                    mutated.CalculateFitness();
                }

            }
            return mutated;
        }
    }

}
