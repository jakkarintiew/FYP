using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;

namespace GeneticAlgorithm
{
    public class Chromosome
    {
        // Gene is type int List
        public List<int> Genes { get; set; }
        public string ReadableGenes { get; set; }
        public Scheduler Schedule { get; set; }
        public double Fitness { get; set; }
        Mcg59 Random = new Mcg59(RandomSeed.Robust());


        // Construtor
        public Chromosome(bool shouldInitGenes = true)
        {
            // Create the Gene array with size
            Genes = new List<int>(Enumerable.Repeat(-1, Settings.NumAllMachines * Settings.NumJobs));
            ReadableGenes = "";
            Schedule = new Scheduler();

            if (shouldInitGenes)
            {
                Genes = GetRandomGenes();
                CalculateFitness();
            }
        }

        private int ProbabilityMachineSelection(List<double> randSelectionColumn)
        {
            List<int> machineIndeces = Enumerable.Range(0, Settings.NumAllMachines).ToList();
            List<double> tmp = randSelectionColumn.ToList();
            List<double> infeasibles = new List<double>();

            //Console.WriteLine("randSelectionColumn:     {0:0.00}", string.Join(",", randSelectionColumn));

            for (int i = 0; i < randSelectionColumn.Count; i++)
            {
                if (randSelectionColumn[i] >= float.MaxValue)
                {
                    infeasibles.Add(i);
                    tmp.Remove(randSelectionColumn[i]);
                }
            }

            int maxIndex;
            int minIndex;
            List<double> transformedColumn = new List<double>(randSelectionColumn.Count);
            List<double> probabilityVector = new List<double>(randSelectionColumn.Count);
            Dictionary<int, double> dict;

            maxIndex = randSelectionColumn.IndexOf(tmp.Max());
            minIndex = randSelectionColumn.IndexOf(tmp.Min());

            if (randSelectionColumn[maxIndex] <= 0)
            {
                for (int i = 0; i < randSelectionColumn.Count; i++)
                {
                    randSelectionColumn[i] += Math.Abs(randSelectionColumn[maxIndex]) + Math.Abs(randSelectionColumn[maxIndex] - randSelectionColumn[minIndex]);
                }
            }

            if (randSelectionColumn[maxIndex] - randSelectionColumn[minIndex] == 0)
            {
                for (int i = 0; i < randSelectionColumn.Count; i++)
                {
                    randSelectionColumn[i] += 1;
                }
            }

            transformedColumn = randSelectionColumn.ToList();

            for (int i = 0; i < transformedColumn.Count; i++)
            {
                if (i != maxIndex)
                {
                    transformedColumn[i] = randSelectionColumn[maxIndex] + Math.Abs(randSelectionColumn[maxIndex] - randSelectionColumn[i]);
                }
            }

            for (int i = 0; i < transformedColumn.Count; i++)
            {
                if (infeasibles.Contains(i))
                {
                    transformedColumn[i] = 0;
                }
            }

            double sum = transformedColumn.Sum();
            for (int i = 0; i < transformedColumn.Count; i++)
            {
                probabilityVector.Add(transformedColumn[i] / sum);
            }

            dict = machineIndeces.Zip(probabilityVector, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            double randDouble = Random.NextDouble();
            double cumulative = 0.0;
            //Console.WriteLine("randSelectionColumn:     {0:0.00}", string.Join(",", randSelectionColumn));
            //Console.WriteLine("distance_column:         {0:0.00}", string.Join(",", transformedColumn));

            for (int i = 0; i < dict.Count; i++)
            {
                cumulative += dict[dict.Keys.ElementAt(i)];
                if (randDouble < cumulative)
                {
                    int selectedElement = dict.Keys.ElementAt(i);
                    //Console.WriteLine("prob_vector: {0}", string.Join(",", probabilityVector));
                    //Console.WriteLine("randDouble: {0}", randDouble);
                    //Console.WriteLine("selectedElement: {0}", selectedElement);
                    return selectedElement;
                }
            }

            return -1;
        }

        public List<int> GetRandomGenes()
        {
        Restart:
            Schedule = new Scheduler();
            Schedule.InitObjects();
            bool isFeasible;
            int counter;
            int randJobIndex;

            // If IsAllMachinesUtilized, randomly assigned one job to each compulsary machine
            if (Settings.IsAllMachinesUtilized)
            {
                foreach (Machine machine in Schedule.Machines.Where(mc => !mc.IsThirdParty || mc.IsCompulsary))
                {
                    randJobIndex = Random.Next(Schedule.Jobs.Count);
                    while (Schedule.Jobs[randJobIndex].AssignedMachine != null)
                    {
                        randJobIndex = Random.Next(Schedule.Jobs.Count);
                    }

                    isFeasible = false;
                    counter = 0;

                    while (!isFeasible)
                    {
                        if (Schedule.IsFeasible(machine, Schedule.Jobs[randJobIndex]))
                        {
                            Schedule.Assign(machine, Schedule.Jobs[randJobIndex]);
                            isFeasible = true;
                        }
                        else
                        {
                            counter++;
                            if (counter > 20)
                            {
                                //Console.WriteLine(counter);
                                goto Restart;
                            }
                        }
                    }
                }
            }

            int numRemainingJobs = Schedule.Jobs.Where(x => x.AssignedMachine == null).Count();

            for (int j = 0; j < numRemainingJobs; j++)
            {
                randJobIndex = Random.Next(Schedule.Jobs.Count);
                while (Schedule.Jobs[randJobIndex].AssignedMachine != null)
                {
                    randJobIndex = Random.Next(Schedule.Jobs.Count);
                }


                if (Schedule.Jobs[randJobIndex].AssignedMachine == null)
                {
                    isFeasible = false;
                    counter = 0;
                    int selectedMachine;
                    List<double> randSelectionColumn = new List<double>();

                    for (int i = 0; i < Schedule.Machines.Count; i++)
                    {
                        randSelectionColumn.Add(Schedule.CalculateIncrementalFitness(Schedule.Machines[i], Schedule.Jobs[randJobIndex]));
                    }

                    while (!isFeasible)
                    {
                        selectedMachine = ProbabilityMachineSelection(randSelectionColumn);
                        if (Schedule.IsFeasible(Schedule.Machines[selectedMachine], Schedule.Jobs[randJobIndex]))
                        {
                            Schedule.Assign(Schedule.Machines[selectedMachine], Schedule.Jobs[randJobIndex]);
                            //Console.WriteLine("job.Index: {0}", Schedule.Jobs[randJobIndex].Index);
                            //Console.WriteLine("machine.Index: {0}", Schedule.Machines[selectedMachine].Index);
                            //Console.WriteLine("machine.Name: {0}", Schedule.Machines[selectedMachine].Name);
                            //Console.WriteLine("selectedMachine: " + selectedMachine);
                            //Console.WriteLine();
                            isFeasible = true;
                        }
                        else
                        {
                            counter++;
                            randSelectionColumn[selectedMachine] = float.MaxValue;
                        }
                    }
                }
            }

            Schedule.ScheduleToGenes();
            Genes = Schedule.Genes.ToList();
            //Console.WriteLine("Schedule.IsOverallFeasible:  {0}", Schedule.IsOverallFeasible());
            //Console.WriteLine("Genes:  {0}", GetReadableGenes());
            //Console.WriteLine("========================");
            return Schedule.Genes;
        }
        public void MakeProperGenes()
        {
            List<int> tmp = new List<int>();
            int slotPosition;
            
            for (int i = 0; i < Settings.NumAllMachines; i++)
            {
                for (int j = 0; j  < Settings.NumJobs; j ++)
                {
                    slotPosition = Settings.NumJobs * i + j;
                    if (Genes[slotPosition] < 100)
                    {
                        tmp.Add(Genes[slotPosition]);
                    }
                }

                for (int j = 0; j < Settings.NumJobs; j++)
                {
                    slotPosition = Settings.NumJobs * i + j;
                    if (Genes[slotPosition] >= 100)
                    {
                        tmp.Add(Genes[slotPosition]);
                    }
                }
            }

            Genes = tmp;
        }

        public double CalculateFitness()
        {
            MakeProperGenes();
            Schedule = new Scheduler();
            Schedule.Genes = Genes;
            Schedule.GenesToSchedule();
            Schedule.GetOverallSchedule();

            switch ((Settings.ObjetiveFunction)Settings.ObjectiveCase)
            {
                case Settings.ObjetiveFunction.TotalCostWithPriority:
                case Settings.ObjetiveFunction.TotalCostNoPriority:
                    Fitness = Schedule.TotalCost;
                    break;
                case Settings.ObjetiveFunction.DemurrageDespatchCost:
                    Fitness = Schedule.DndCost;
                    break;
                case Settings.ObjetiveFunction.SumLateStart:
                    Fitness = Schedule.SumLateStart;
                    break;
                case Settings.ObjetiveFunction.Makespan:
                    Fitness = Schedule.Makespan;
                    break;
            }

            if (!Schedule.IsOverallFeasible())
            {
                Fitness = double.MaxValue;
            }

            return Fitness;
        }

        public string GetReadableGenes()
        {
            ReadableGenes = "";

            for (int i = 0; i < Settings.NumAllMachines; i++)
            {
                ReadableGenes += "|";
                for (int j = 0; j  < Settings.NumJobs; j ++)
                {
                    if (Genes[i * Settings.NumJobs + j] < 100 && Genes[i * Settings.NumJobs + j] >= 0)
                    {
                        ReadableGenes += Genes[i * Settings.NumJobs + j].ToString() + " ";
                    }
                    else
                    {
                        ReadableGenes += ".";
                    }
                }
            }
            return ReadableGenes;
        }

    }

}
