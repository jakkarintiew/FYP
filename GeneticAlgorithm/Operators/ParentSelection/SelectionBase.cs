using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace GeneticAlgorithm.Operators.ParentSelection
{
    public abstract class SelectionBase
    {
        // Properties
        public Mcg59 Random = new Mcg59(RandomSeed.Robust());
        public string SelectionName { get; set; }

        // Constructors
        protected SelectionBase()
        {
        }

        // Methods        
        public abstract Chromosome PerformSelection(List<Chromosome> population);

    }
}

