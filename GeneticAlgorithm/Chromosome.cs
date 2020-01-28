using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using System.Text;



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
        public Chromosome(
            int size,
            bool shouldInitGenes = true
            )
        {
            // Create the Gene array with size
            Genes = new List<int>(Enumerable.Repeat(-1, Settings.NumAllMachines * Settings.NumJobs));
            ReadableGenes = "";
            Schedule = new Scheduler(Genes);

            if (shouldInitGenes)
            {
                Genes = GetRandomGenes();
                //Console.WriteLine("chrmsm: [ {0} ]", string.Join(",  ", genes));
                CalculateFitness();
            }
        }

        private int ProbabilityMachineSelection(Vector<double> randSelectionColumn)
        {
            var machine_index = Enumerable.Range(0, Settings.NumAllMachines).ToList();
            Vector<double> transform_column = randSelectionColumn.Sum() / (randSelectionColumn + 0.1);
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
                    //Console.WriteLine("randSelectionColumn: {0}", string.Join(",", randSelectionColumn));
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

            Vector<double> randSelectionColumn = Vector<double>.Build.Dense(Settings.NumAllMachines);
            bool isFeasible = false;
            int counter = 0;

            // If IsAllMachinesUtilized, randomly assigned one job to each compulsary machine
            if (Settings.IsAllMachinesUtilized)
            {
                foreach (Machine machine in Schedule.Machines)
                {
                    if (!machine.IsThirdParty || machine.IsCompulsary)
                    {

                        int randJobIndex = Random.Next(Settings.NumJobs);
                        if (Schedule.Jobs[randJobIndex].AssignedMachine == null)
                        {
                            isFeasible = false;
                            counter = 0;
                            while (!isFeasible)
                            {
                                if (Schedule.IsFeasible(machine, Schedule.Jobs[randJobIndex]))
                                {
                                    Schedule.Assign(machine, Schedule.Jobs[randJobIndex]);
                                    Schedule.ScheduleToGenes();
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

            for (int j = 0; j < Schedule.Jobs.Count; j++)
            {
                if (Schedule.Jobs[j].AssignedMachine == null)
                {
                    isFeasible = false;
                    counter = 0;

                    for (int i = 0; i < Schedule.Machines.Count; i++)
                    {
                        randSelectionColumn[i] = Schedule.CalculateIncrementalFitness(Schedule.Machines[i], Schedule.Jobs[j]);
                    }

                    while (!isFeasible)
                    {

                        int selectedMachine = ProbabilityMachineSelection(randSelectionColumn);

                        if (!Schedule.IsFeasible(Schedule.Machines[selectedMachine], Schedule.Jobs[j]))
                        {
                            counter++;
                            randSelectionColumn[selectedMachine] = float.MaxValue;
                        }
                        else
                        {
                            Schedule.Assign(Schedule.Machines[selectedMachine], Schedule.Jobs[j]);
                            isFeasible = true;
                        }
                    }

                    //Console.WriteLine(counter);
                    //Console.WriteLine(isFeasible);
                }
            }

            Schedule.ScheduleToGenes();
            //Console.WriteLine(schedule.IsOverallFeasible());
            //Console.WriteLine("scheduleToGenes: [ {0} ]", string.Join(",", schedule.genes));
            Genes = Schedule.Genes;
            MakeProperGenes();
            return Genes;
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
            Schedule = new Scheduler(Genes);
            //Schedule.Genes = Genes;
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
                        ReadableGenes += Genes[i * Settings.NumJobs + j].ToString();
                        ReadableGenes += " ";
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
