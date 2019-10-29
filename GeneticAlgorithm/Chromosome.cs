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
        public Schedule schedule { get; set; }

        // Fitness of each individual
        public double Fitness { get; private set; }

        private Mcg59 random;
        private Func<List<int>> getRandomGenes;
        private Func<int, double> fitnessFunction;

        // Construtor
        public Chromosome(
            int size,
            Mcg59 random,
            Func<List<int>> getRandomGenes,
            Func<int, double> fitnessFunction,
            bool shouldInitGenes = true)
        {
            // Create the Gene array with size
            Genes = new List<int>(new int[size]);

            this.random = random;
            this.getRandomGenes = getRandomGenes;
            this.fitnessFunction = fitnessFunction;

            if (shouldInitGenes)
            {
                Genes = getRandomGenes();
                schedule = new Schedule();
                schedule.GetSchedule(Genes);
            }

        }

        public double CalculateFitness(int index)
        {
            Fitness = fitnessFunction(index);
            return Fitness;
        }
     
    }

}
