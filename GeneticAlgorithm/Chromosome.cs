using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;



namespace GeneticAlgorithm
{
    public class Chromosome
    {
        // Gene is type int List
        public List<int> genes { get; set; }
        public Scheduler schedule { get; set; }
        public double fitness { get; set; }
        private Func<List<int>> getRandomGenes;

        // Construtor
        public Chromosome(
            int size,
            Func<List<int>> getRandomGenes,
            bool shouldInitGenes = true)
        {


            // Create the Gene array with size
            genes = new List<int>(Enumerable.Repeat(-1, size));

            schedule = new Scheduler();
            this.getRandomGenes = getRandomGenes;

            if (shouldInitGenes)
            {
                genes = getRandomGenes();
                //Console.WriteLine("chrmsm: [ {0} ]", string.Join(",  ", genes));
                CalculateFitness();
            }



        }

        public void MakeProperGenes()
        {
            List<int> tmp = new List<int>();
            int slotPosition;
            
            for (int i = 0; i < Data.NumAllMachines; i++)
            {
                for (int j = 0; j  < Data.NumJobs; j ++)
                {
                    slotPosition = Data.NumJobs * i + j;
                    if (genes[slotPosition] <= 100)
                    {
                        tmp.Add(genes[slotPosition]);
                    }
                }

                for (int j = 0; j < Data.NumJobs; j++)
                {
                    slotPosition = Data.NumJobs * i + j;
                    if (genes[slotPosition] > 100)
                    {
                        tmp.Add(genes[slotPosition]);
                    }
                }
            }

            genes = tmp;
        }

        public double CalculateFitness()
        {
            //Console.WriteLine("chrmsm:     [ {0} ]", string.Join(",", genes));
            //Printer printer = new Printer();
            //printer.PrintSchedule(chrmsm.schedule);
            schedule = new Scheduler();
            schedule.genes = genes;
            schedule.GenesToSchedule();
            schedule.GetSchedule();

            switch ((Data.ObjetiveFunction)Data.ObjectiveCase)
            {
                case Data.ObjetiveFunction.TotalCostWithPriority:
                case Data.ObjetiveFunction.TotalCostNoPriority:
                    fitness = schedule.totalCost;
                    break;
                case Data.ObjetiveFunction.DemurrageDespatchCost:
                    fitness = schedule.dndCost;
                    break;
                case Data.ObjetiveFunction.SumLateStart:
                    fitness = schedule.sumLateStart;
                    break;
                case Data.ObjetiveFunction.Makespan:
                    fitness = schedule.makespan;
                    break;
            }

            if (!schedule.IsOverallFeasible())
            {
                fitness = double.MaxValue;
            }

            return fitness;
        }

    }

}
