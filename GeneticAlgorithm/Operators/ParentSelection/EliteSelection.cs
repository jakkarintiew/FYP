using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Operators.ParentSelection
{
    class EliteSelection : SelectionBase
    {
        public EliteSelection()
        {
            SelectionName = "EliteSelection";
        }
        public override Chromosome PerformSelection(List<Chromosome> population)
        {
            Chromosome best = null;
            int eliteGroupSize = 20;
            best = population.OrderBy(chrmsm => chrmsm.Fitness).Take(eliteGroupSize).ToList()[Random.Next(eliteGroupSize)];
            return best;
        }

    }
}
