using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Operators.Mutations
{
    class RandomMutation : MutationBase
    {
        public RandomMutation()
        {
            MutationName = "RandomMutation";
        }

        public void Shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public override Chromosome PerformMutation(Chromosome chromosome) {
            Chromosome mutated = new Chromosome(chromosome.Genes.Count, shouldInitGenes: false);
            mutated.Genes = new List<int>(chromosome.Genes);

            if (Random.NextDouble() < Settings.MutationRate)
            {
                // Generate a random gene
                //mutated.GetRandomGenes();
                Shuffle(mutated.Genes);
                mutated.MakeProperGenes();
                //Console.WriteLine("{0}", string.Join(",", chromosome.GetReadableGenes()));
                //Console.WriteLine("{0}\n", string.Join(",", mutated.GetReadableGenes()));

                mutated.CalculateFitness();
            }

            return mutated;
        }
    }
}
