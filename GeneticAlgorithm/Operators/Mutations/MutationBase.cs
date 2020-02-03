using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace GeneticAlgorithm
{
    public abstract class MutationBase : IMutation
    {
        // Properties
        public Mcg59 Random = new Mcg59(RandomSeed.Robust());
        public string MutationName { get; set; }
        // Constructors
        protected MutationBase()
        {
        }

        // Methods        
        public abstract Chromosome PerformMutation(Chromosome chromosome);

    }
}
