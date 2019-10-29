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
        public double[,] downTimes { get; set; }

        // Construtor
        public Machine(int index, double readyTime, double procRate, double[,] downTimes)
        {
            this.index = index;
            this.readyTime = readyTime;
            this.procRate = procRate;
            this.downTimes = downTimes;
            assignedJobs = new List<Job>();
        }
    }
}
