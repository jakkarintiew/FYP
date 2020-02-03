using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{

    class SinglePointCrossover : CrossoverBase
    {
        public SinglePointCrossover()
        {
            CrossoverName = "SinglePointCrossover";
        }

        // Crossover function
        public override Chromosome PerformCrossover(Chromosome firstParent, Chromosome secondParent)
        {

            Chromosome child = new Chromosome(shouldInitGenes: false);

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
                    Chromosome firstChild = new Chromosome(shouldInitGenes: false);
                    Chromosome secondChild = new Chromosome(shouldInitGenes: false);
                    counter++;

                    if (counter > 50)
                    {
                        //Console.WriteLine(counter);
                        //if (Random.NextDouble() > 0.5)
                        if (firstParent.Fitness > secondParent.Fitness)
                        {
                            child = secondParent;
                        }
                        else
                        {
                            child = firstParent;
                        }
                        break;
                    }

                    for (int i = 0; i < firstParent.Schedule.Machines.Count; i++)
                    {
                        if (firstParent.Schedule.Machines[i].AssignedJobIds.Count == 0)
                        {
                            firstChild.Schedule.Machines[i].AssignedJobIds = new List<int>(firstParent.Schedule.Machines[i].AssignedJobIds);
                            secondChild.Schedule.Machines[i].AssignedJobIds = new List<int>(firstParent.Schedule.Machines[i].AssignedJobIds);
                        }
                        else
                        {
                            int crossoverPoint = Random.Next(0, firstParent.Schedule.Machines[i].AssignedJobIds.Count);

                            List<int> firstParentLeft = firstParent.Schedule.Machines[i].AssignedJobIds.Take(crossoverPoint).ToList();
                            List<int> firstParentRight = firstParent.Schedule.Machines[i].AssignedJobIds.Skip(crossoverPoint).ToList();
                            firstChild.Schedule.Machines[i].AssignedJobIds.AddRange(firstParentLeft);
                            secondChild.Schedule.Machines[i].AssignedJobIds.AddRange(firstParentRight);

                            //Console.WriteLine("crossoverPoint: " + crossoverPoint);
                            //Console.WriteLine("firstParent machine i:   {0}", string.Join(",", firstParent.Schedule.Machines[i].AssignedJobIds));
                            //Console.WriteLine("firstParentLeft:         {0}", string.Join(",", firstParentLeft));
                            //Console.WriteLine("firstParentRight:        {0}", string.Join(",", firstParentRight));
                            //Console.WriteLine("firstChild machine i:    {0}", string.Join(",", firstChild.Schedule.Machines[i].AssignedJobIds));
                            //Console.WriteLine("secondChild machine i:   {0}\n", string.Join(",", secondChild.Schedule.Machines[i].AssignedJobIds));

                        }
                    }

                    firstChild.Schedule.MachineJobListToGenes();
                    secondChild.Schedule.MachineJobListToGenes();

                    for (int i = 0; i < firstParent.Schedule.Machines.Count; i++)
                    {
                        //Console.WriteLine("secondParent machine i:   {0}", string.Join(",", secondParent.Schedule.Machines[i].AssignedJobIds));
                        //Console.WriteLine("firstChild machine i:    {0}", string.Join(",", firstChild.Schedule.Machines[i].AssignedJobIds));
                        //Console.WriteLine("secondChild machine i:   {0}\n", string.Join(",", secondChild.Schedule.Machines[i].AssignedJobIds));

                        while (NextUnrepeatedElem(firstChild.Schedule.Genes, secondParent.Schedule.Machines[i].AssignedJobIds) != -1)
                        {
                            firstChild.Schedule.Machines[i].AssignedJobIds.Add(NextUnrepeatedElem(firstChild.Schedule.Genes, secondParent.Schedule.Machines[i].AssignedJobIds));
                            firstChild.Schedule.MachineJobListToGenes();
                        }

                        while (NextUnrepeatedElem(secondChild.Schedule.Genes, secondParent.Schedule.Machines[i].AssignedJobIds) != -1)
                        {
                            secondChild.Schedule.Machines[i].AssignedJobIds.Add(NextUnrepeatedElem(secondChild.Schedule.Genes, secondParent.Schedule.Machines[i].AssignedJobIds));
                            secondChild.Schedule.MachineJobListToGenes();
                        }

                        //Console.WriteLine("firstChild machine i:    {0}", string.Join(",", firstChild.Schedule.Machines[i].AssignedJobIds));
                        //Console.WriteLine("secondChild machine i:   {0}\n", string.Join(",", secondChild.Schedule.Machines[i].AssignedJobIds));
                    }


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
                   
                    isFeasible = child.Fitness <= (firstParent.Fitness + secondParent.Fitness)/2;


                    //if (isFeasible)
                    //{
                    //    Console.WriteLine("firstParent:   {0}", string.Join(",", firstParent.GetReadableGenes()));
                    //    Console.WriteLine("secondParent:  {0}", string.Join(",", secondParent.GetReadableGenes()));
                    //    Console.WriteLine("firstChild:    {0}", string.Join(",", firstChild.GetReadableGenes()));
                    //    Console.WriteLine("secondChild:   {0}", string.Join(",", secondChild.GetReadableGenes()));
                    //    Console.WriteLine("if child Feasible {0}\n", isFeasible);
                    //}
                }

                //Console.WriteLine(counter);

            }
            return child;
        }

    }
}
