using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Operators.Mutations
{
    class InsertionMutation : MutationBase
    {
        public InsertionMutation()
        {
            MutationName = "InsertionMutation";
        }

        public override Chromosome PerformMutation(Chromosome chromosome)
        {
            Chromosome mutated = new Chromosome(chromosome.Genes.Count, shouldInitGenes: false);
            mutated.Genes = new List<int>(chromosome.Genes);
            List<int> listJobs = mutated.Genes.Where(x => x < 100).ToList();

            if (Random.NextDouble() < Settings.MutationRate)
            {
                int randJob = listJobs[Random.Next(0, listJobs.Count)];
                int randDestination = listJobs[Random.Next(0, listJobs.Count)];
                int randJobPosition = mutated.Genes.FindLastIndex(x => x == randJob);
                mutated.Genes.Remove(randJob);
                mutated.Genes.Insert(randDestination, randJob);
                mutated.MakeProperGenes();
                //Console.WriteLine("{0}", string.Join(",", chromosome.GetReadableGenes()));
                //Console.WriteLine("{0}\n", string.Join(",", mutated.GetReadableGenes()));
            }

            return mutated;
        }
    }
}
