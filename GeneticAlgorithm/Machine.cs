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
        public bool isGearAccepting { get; private set; }
        public bool isDedicated { get; private set; }
        public string dedicated { get; private set; }
        public double[,] downTimes { get; set; }
        public List<Job> assignedJobs { get; set; }

       
        // Construtor
        public Machine(
            int index, 
            double readyTime, 
            double procRate, 
            bool isGearAccepting, 
            bool isDedicated, 
            string dedicated, 
            double[,] downTimes)
        {
            this.index = index;
            this.readyTime = readyTime;
            this.procRate = procRate;
            this.isGearAccepting = isGearAccepting;
            this.isDedicated = isDedicated;
            this.dedicated = dedicated;
            this.downTimes = downTimes;
            assignedJobs = new List<Job>();
        }
    }
}
