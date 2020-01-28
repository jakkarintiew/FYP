using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Event
    {
        public int Index { get; set; }
        public string Type { get; private set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public Job _Job { get; private set; }


        // Constructor
        public Event(string type, double startTime, double endTime, Job job)
        {
            Type = type;
            StartTime = startTime;
            EndTime = endTime;
            _Job = job;
        }
    }
}
