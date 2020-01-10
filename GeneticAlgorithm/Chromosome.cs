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
            
            for (int i = 0; i < Data.numMachines; i++)
            {
                for (int j = 0; j  < Data.numJobs; j ++)
                {
                    slotPosition = Data.numJobs * i + j;
                    if (genes[slotPosition] <= 100)
                    {
                        tmp.Add(genes[slotPosition]);
                    }
                }

                for (int j = 0; j < Data.numJobs; j++)
                {
                    slotPosition = Data.numJobs * i + j;
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
            //Console.WriteLine("chrmsm:     [ {0} ]", string.Join(",\t", genes));
            //Printer printer = new Printer();
            //printer.PrintSchedule(chrmsm.schedule);
            schedule.genes = genes;
            schedule.genesToSchedule();
            schedule.GetSchedule();

            Data.objetiveFunction objetive = (Data.objetiveFunction)Data.objectiveCase;

            switch (objetive)
            {
                case Data.objetiveFunction.TotalCostWithPriority:
                    fitness = schedule.totalCost;
                    break;
                case Data.objetiveFunction.TotalCostNoPriority:
                    fitness = schedule.totalCost;
                    break;
                case Data.objetiveFunction.DemurrageDespatchCost:
                    fitness = schedule.dndCost;
                    break;
                case Data.objetiveFunction.SumLateStart:
                    fitness = schedule.sumLateStart;
                    break;
                case Data.objetiveFunction.Makespan:
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
