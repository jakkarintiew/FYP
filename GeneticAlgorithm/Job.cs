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
        public bool isGeared { get; private set; }
        public bool isDedicated { get; private set; }
        public string shipper { get; private set; }
        public Machine assignedMachine { get; set; }
        public double startTime { get; set; }
        public double completeTime { get; set; }

        // Construtor
        public Job(int index, double readyTime, double quantity, bool isGeared, bool isDedicated, string shipper)
        {
            this.index = index;
            this.readyTime = readyTime;
            this.quantity = quantity;
            this.isGeared = isGeared;
            this.isDedicated = isDedicated;
            this.shipper = shipper;
        }
    }
}
