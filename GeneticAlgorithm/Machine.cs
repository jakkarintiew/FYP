using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Machine
    {
        public int index { get; private set; }
        public double readyTime { get; set; }
        public double procRate { get; private set; }
        public List<Job> assignedJobs { get; set; }


        // Construtor
        public Machine(int index, double readyTime, double procRate)
        {
            this.index = index;
            this.readyTime = readyTime;
            this.procRate = procRate;
            assignedJobs = new List<Job>();
        }
    }
}
