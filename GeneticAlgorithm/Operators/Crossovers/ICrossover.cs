namespace GeneticAlgorithm
{
    public interface ICrossover
    {
        // Properties
        string CrossoverName { get; set; }

        // Methods        
        Chromosome PerformCrossover(Chromosome firstParent, Chromosome secondParent);
    }
}
