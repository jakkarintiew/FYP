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
        public double readyTime { get; private set; }
        public double latestReadyTime { get; set; }
        public double procRate { get; private set; }
        public bool isGearAccepting { get; private set; }
        public bool isDedicated { get; private set; }
        public string dedicatedCustomer { get; private set; }
        public double[,] downTimes { get; set; }
        public List<Job> assignedJobs { get; set; }
        public List<Event> scheduledEvents { get; set; }


        // Construtor
        public Machine(
            int index, 
            double readyTime, 
            double procRate, 
            bool isGearAccepting, 
            bool isDedicated, 
            string dedicatedCustomer, 
            double[,] downTimes)
        {
            this.index = index;
            latestReadyTime = readyTime;
            this.readyTime = readyTime;
            this.procRate = procRate;
            this.isGearAccepting = isGearAccepting;
            this.isDedicated = isDedicated;
            this.dedicatedCustomer = dedicatedCustomer;
            this.downTimes = downTimes;
            assignedJobs = new List<Job>();
            scheduledEvents = new List<Event>();

            // Downtimes consideration
            for (int i = 0; i < downTimes.GetLength(0); i++)
            {
                scheduledEvents.Add(new Event(
                   type: "DownTime",
                   startTime: downTimes[i, 0],
                   endTime: downTimes[i, 1],
                   job: new Job(-1, -1, -1, false, false, "0", -1)
                   ));
            }
        }
    }
}
