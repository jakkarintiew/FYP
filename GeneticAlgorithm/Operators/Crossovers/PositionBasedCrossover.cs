using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Operators.Crossovers
{

    class PositionBasedCrossover : CrossoverBase
    {
        public PositionBasedCrossover()
        {
            CrossoverName = "PositionBasedCrossover";
        }

        // Crossover function
        public override Chromosome PerformCrossover(Chromosome firstParent, Chromosome secondParent)
        {

            Chromosome child = new Chromosome(firstParent.Genes.Count, shouldInitGenes: false);

            if (firstParent.Genes == secondParent.Genes)
            {
                child.Genes = firstParent.Genes;
            }
            else
            {
                bool isFeasible = false;
                int counter = 0;
                while (!isFeasible)
                {
                    // create new child Chromosome with same gene array size as parent; improve performance by setting shouldInitGenes: false
                    Chromosome firstChild = new Chromosome(firstParent.Genes.Count, shouldInitGenes: false);
                    Chromosome secondChild = new Chromosome(firstParent.Genes.Count, shouldInitGenes: false);

                    counter++;

                    if (counter > 50)
                    {
                        //Console.WriteLine(counter);
                        child.GetRandomGenes();
                        child.CalculateFitness();
                        break;
                    }

                    List<int> positionProfile = new List<int>(new int[firstParent.Genes.Count]);
                    double prob;
                    for (int i = 0; i < firstParent.Genes.Count; i++)
                    {
                        prob = Random.NextDouble();
                        if (prob < secondParent.Fitness / (firstParent.Fitness + secondParent.Fitness))
                        {
                            positionProfile[i] = 1;
                        }
                    }

                    for (int i = 0; i < positionProfile.Count; i++)
                    {
                        if (positionProfile[i] == 1)
                        {
                            firstChild.Genes[i] = firstParent.Genes[i];
                        }
                        else
                        {
                            secondChild.Genes[i] = secondParent.Genes[i];
                        }
                    }
                    //Console.WriteLine("positionProfile: {0}", string.Join(",", positionProfile));
                    //Console.WriteLine("firstParent:     {0}", string.Join(",", firstParent.GetReadableGenes()));
                    //Console.WriteLine("secondParent:    {0}", string.Join(",", secondParent.GetReadableGenes()));
                    //Console.WriteLine("firstChild:      {0}", string.Join(",", firstChild.GetReadableGenes()));
                    //Console.WriteLine("secondChild:     {0}", string.Join(",", secondChild.GetReadableGenes()));

                    for (int i = 0; i < positionProfile.Count; i++)
                    {
                        if (positionProfile[i] == 1)
                        {
                            secondChild.Genes[i] = NextUnrepeatedElem(secondChild.Genes, firstParent.Genes);
                        }
                        else
                        {
                            firstChild.Genes[i] = NextUnrepeatedElem(firstChild.Genes, secondParent.Genes);
                        }
                    }

                    //Console.WriteLine("firstChild:      {0}", string.Join(",", firstChild.GetReadableGenes()));
                    //Console.WriteLine("secondChild:     {0}\n", string.Join(",", secondChild.GetReadableGenes()));

                    firstChild.MakeProperGenes();
                    secondChild.MakeProperGenes();
                    firstChild.CalculateFitness();
                    secondChild.CalculateFitness();

                    if (firstChild.Fitness > secondChild.Fitness)
                    {
                        child = secondChild;
                    }
                    else
                    {
                        child = firstChild;
                    }

                    isFeasible = child.Schedule.IsOverallFeasible();

                    //if (isFeasible)
                    //{
                    //    Console.WriteLine("firstParent:   {0}", string.Join(",", firstParent.GetReadableGenes()));
                    //    Console.WriteLine("secondParent:  {0}", string.Join(",", secondParent.GetReadableGenes()));
                    //    Console.WriteLine("firstChild:    {0}", string.Join(",", firstChild.GetReadableGenes()));
                    //    Console.WriteLine("secondChild:   {0}", string.Join(",", secondChild.GetReadableGenes()));
                    //    Console.WriteLine("if child Feasible {0}\n", isFeasible);
                    //}

                    //isFeasible = true;

                }

                //Console.WriteLine(counter);

            }



            return child;
        }

    }
}
