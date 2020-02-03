using System.Collections.Generic;

namespace GeneticAlgorithm
{
    public interface ISelection
    {
        // Properties
        string SelectionName { get; set; }

        // Methods        
        Chromosome PerformSelection(List<Chromosome> population);
    }
}
