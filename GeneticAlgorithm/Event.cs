using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Event
    {
        public int index { get; set; }
        public string type { get; private set; }
        public double startTime { get; private set; }
        public double endTime { get; private set; }
        public Job job { get; private set; }


        // Constructor
        public Event(string type, double startTime, double endTime, Job job)
        {
            this.type = type;
            this.startTime = startTime;
            this.endTime = endTime;
            this.job = job;
        }
    }
}
