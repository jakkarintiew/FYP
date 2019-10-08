using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Job
    {
        public int index { get; private set; }
        public double readyTime { get; private set; }
        public double quantity { get; private set; }
        public Machine assignedMachine { get; set; }
        public double startTime { get; set; }
        public double completeTime { get; set; }

        // Construtor
        public Job(int index, double readyTime, double quantity)
        {
            this.index = index;
            this.readyTime = readyTime;
            this.quantity = quantity;
        }
    }
}
