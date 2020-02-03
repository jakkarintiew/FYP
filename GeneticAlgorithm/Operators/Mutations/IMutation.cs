namespace GeneticAlgorithm
{
    public interface IMutation
    {
        // Properties
        string MutationName { get; set; }

        // Methods        
        Chromosome PerformMutation(Chromosome chromosome);
    }
}
