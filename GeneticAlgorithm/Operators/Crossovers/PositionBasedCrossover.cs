using System.Collections.Generic;
using System.Linq;


namespace GeneticAlgorithm
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

            Chromosome child = new Chromosome(shouldInitGenes: false);

            if (firstParent.Genes.SequenceEqual(secondParent.Genes))
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
                    Chromosome firstChild = new Chromosome(shouldInitGenes: false);
                    Chromosome secondChild = new Chromosome(shouldInitGenes: false);

                    counter++;

                    if (counter > 20)
                    {
                        //Console.WriteLine(counter);
                        child.Genes = child.GetRandomGenes();
                        child.CalculateFitness();
                        break;
                    }

                    //Console.WriteLine("firstChild:      {0}", firstChild.GetReadableGenes());
                    //Console.WriteLine("secondChild:     {0}", secondChild.GetReadableGenes());


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

                    //Console.WriteLine("positionProfile: {0}", string.Join(" ", positionProfile));
                    //Console.WriteLine("firstParent:     {0}", firstParent.GetReadableGenes());
                    //Console.WriteLine("secondParent:    {0}", secondParent.GetReadableGenes());
                    //Console.WriteLine("firstChild:      {0}", firstChild.GetReadableGenes());
                    //Console.WriteLine("secondChild:     {0}", secondChild.GetReadableGenes());

                    for (int i = 0; i < firstChild.Genes.Count; i++)
                    {
                        if (firstChild.Genes[i] == -1)
                        {
                            firstChild.Genes[i] = NextUnrepeatedElem(firstChild.Genes, secondParent.Genes);
                        }
                        else if (secondChild.Genes[i] == -1)
                        {
                            secondChild.Genes[i] = NextUnrepeatedElem(secondChild.Genes, firstParent.Genes);
                        }
                    }

                    //Console.WriteLine("firstChild:      {0}", firstChild.GetReadableGenes());
                    //Console.WriteLine("secondChild:     {0}\n", secondChild.GetReadableGenes());

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

                    isFeasible = child.Schedule.IsOverallFeasible() && child.Fitness <= (firstParent.Fitness + secondParent.Fitness) / 2;

                    //if (isFeasible)
                    //{
                    //    Console.WriteLine("positionProfile: {0}", string.Join(" ", positionProfile));
                    //    Console.WriteLine("firstParent:     {0}", firstParent.GetReadableGenes());
                    //    Console.WriteLine("secondParent:    {0}", secondParent.GetReadableGenes());
                    //    Console.WriteLine("firstChild:      {0}", firstChild.GetReadableGenes());
                    //    Console.WriteLine("secondChild:     {0}", secondChild.GetReadableGenes());
                    //    Console.WriteLine("child.Fitness:   {0}\n", child.Fitness);
                    //    Console.WriteLine("if child Feasible {0}\n", isFeasible);
                    //    Console.WriteLine(counter);
                    //}

                }

                //Console.WriteLine(counter);

            }
            return child;
        }

    }
}
