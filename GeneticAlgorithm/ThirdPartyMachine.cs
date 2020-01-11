using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class ThirdPartyMachine
    {
        public int index { get; set; }
        public bool isCompulsary { get; set; }
        public double rentalUnitCost { get; set; }
        public List<Job> assignedJobs { get; set; }
        public List<Event> scheduledEvents { get; set; }


        // Construtor
        public ThirdPartyMachine()
        {

        }
    }
}
