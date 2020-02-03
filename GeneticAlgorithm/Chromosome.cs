using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;

namespace GeneticAlgorithm
{
    public class Chromosome
    {
        // Gene is type int List
        public List<int> Genes { get; set; }
        public String ReadableGenes { get; set; }
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

        private int ProbabilityMachineSelection(Vector<double> randSelectionColumn)
        {
            var machine_index = Enumerable.Range(0, Settings.NumAllMachines).ToList();
            Vector<double> transform_column = randSelectionColumn.Sum() / (randSelectionColumn + 0.01);
            Vector<double> prob_vector = transform_column / transform_column.Sum();
            Dictionary<int, double> dict = machine_index.Zip(prob_vector, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            double randDouble = Random.NextDouble();
            double cumulative = 0.0;

            for (int i = 0; i < dict.Count; i++)
            {
                cumulative += dict[dict.Keys.ElementAt(i)];
                if (randDouble < cumulative)
                {
                    int selectedElement = dict.Keys.ElementAt(i);
                    //Console.WriteLine("randSelectionColumn: {0:0.00}", string.Join(",", randSelectionColumn));
                    //Console.WriteLine("transform_column: {0}", string.Join(",", transform_column));
                    //Console.WriteLine("prob_vector: {0}", string.Join(",", prob_vector));
                    //Console.WriteLine("randDouble: {0}", randDouble);
                    //Console.WriteLine("selectedElement: {0}\n", selectedElement);
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
            Vector<double> randSelectionColumn = Vector<double>.Build.Dense(Schedule.Machines.Count);
            bool isFeasible;
            int counter;

            // If IsAllMachinesUtilized, randomly assigned one job to each compulsary machine
            if (Settings.IsAllMachinesUtilized)
            {
                foreach (Machine machine in Schedule.Machines.Where(mc => !mc.IsThirdParty || mc.IsCompulsary))
                {
                    int randJobIndex = Random.Next(Settings.NumJobs);
                    while (Schedule.Jobs[randJobIndex].AssignedMachine != null)
                    {
                        randJobIndex = Random.Next(Settings.NumJobs);
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

            foreach (Job job in Schedule.Jobs)
            {
                if (job.AssignedMachine == null)
                {
                    isFeasible = false;
                    int selectedMachine;

                    for (int i = 0; i < Schedule.Machines.Count; i++)
                    {
                        randSelectionColumn[i] = Schedule.CalculateIncrementalFitness(Schedule.Machines[i], job);
                    }

                    while (!isFeasible)
                    {
                        selectedMachine = ProbabilityMachineSelection(randSelectionColumn);
                        if (Schedule.IsFeasible(Schedule.Machines[selectedMachine], job))
                        {
                            Schedule.Assign(Schedule.Machines[selectedMachine], job);
                            isFeasible = true;
                        }
                        else
                        {
                            randSelectionColumn[selectedMachine] = float.MaxValue;
                        }
                    }
                }
            }

            Schedule.ScheduleToGenes();
            //Console.WriteLine("Schedule.IsOverallFeasible:  {0}", Schedule.IsOverallFeasible());
            //Console.WriteLine("Genes:  {0}", GetReadableGenes());
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
                //ReadableGenes += "|";
                for (int j = 0; j  < Settings.NumJobs; j ++)
                {
                    if (Genes[i * Settings.NumJobs + j] < 100 && Genes[i * Settings.NumJobs + j] >= 0)
                    {
                        ReadableGenes += Genes[i * Settings.NumJobs + j].ToString();
                        ReadableGenes += " ";
                    }
                    else
                    {
                        //ReadableGenes += Genes[i * Settings.NumJobs + j].ToString();
                        //ReadableGenes += " ";
                        ReadableGenes += ". ";
                    }
                }
            }


            return ReadableGenes;
        }

    }

}
