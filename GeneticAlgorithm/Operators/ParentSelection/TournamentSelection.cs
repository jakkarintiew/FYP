using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class TournamentSelection : SelectionBase
    {
        public TournamentSelection()
        {
            SelectionName = "TournamentSelection";
        }
        public override Chromosome PerformSelection(List<Chromosome> population)
        {
            // Tournament selection
            int numCompetitors = 2; // Binary tournament
            Chromosome randSelection;
            Chromosome best = null;

            for (int i = 0; i < numCompetitors; i++)
            {
                randSelection = population[Random.Next(0, population.Count)];

                if (best == null || randSelection.Fitness < best.Fitness)
                {
                    best = randSelection;
                }
            }

            return best;
        }

    }
}
