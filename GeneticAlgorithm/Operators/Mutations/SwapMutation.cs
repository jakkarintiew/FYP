using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Operators.Mutations
{
    class SwapMutation : MutationBase
    {
        public SwapMutation()
        {
            MutationName = "SwapMutation";
        }

        public override Chromosome PerformMutation(Chromosome chromosome)
        {
            Chromosome mutated = new Chromosome(chromosome.Genes.Count, shouldInitGenes: false);
            mutated.Genes = new List<int>(chromosome.Genes);
            List<int> listJobs = mutated.Genes.Where(x => x < 100).ToList();

            if (Random.NextDouble() < Settings.MutationRate)
            {
                int randJob1 = listJobs[Random.Next(0, listJobs.Count)];
                int randJob2 = listJobs[Random.Next(0, listJobs.Count)];
                int temp = randJob2;
                mutated.Genes[mutated.Genes.IndexOf(randJob2)] = randJob1;
                mutated.Genes[mutated.Genes.IndexOf(randJob1)] = temp;
                //Console.WriteLine("{0}", string.Join(",", chromosome.GetReadableGenes()));
                //Console.WriteLine("{0}\n", string.Join(",", mutated.GetReadableGenes()));
            }

            return mutated;
        }
    }
}
