using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;
using GeneticAlgorithm;

namespace GeneticAlgorithm
{
    public abstract class CrossoverBase : ICrossover
    {
        // Properties
        public Mcg59 Random = new Mcg59(RandomSeed.Robust());
        public string CrossoverName { get; set; }
        // Constructors
        protected CrossoverBase()
        {
        }
        public int NextUnrepeatedElem(List<int> firstList, List<int> secondList)
        {
            int idx = 0;
            if (secondList.Count == 0)
            {
                return -1;
            }

            if (firstList.Count == 0)
            {
                return secondList[0];
            }

            while (firstList.Contains(secondList[idx]))
            {
                idx++;
                if (idx >= secondList.Count)
                {
                    return -1;
                }
            }

            return secondList[idx];
        }

        // Methods        
        public abstract Chromosome PerformCrossover(Chromosome firstParent, Chromosome secondParent);

    }
}
