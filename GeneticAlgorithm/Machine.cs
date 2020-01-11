using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Machine: ICloneable
    {
        public int index { get; set; }
        public double position { get; set; }
        public double readyTime { get; set; }
        public double procRate { get; set; }
        public double loadingUnitCost { get; set; }
        public bool isGearAccepting { get; set; }
        public bool isDedicated { get; set; }
        public string dedicatedCustomer { get; set; }

        // third-party prop
        public bool isThirdParty { get; set; }
        public bool isCompulsary { get; set; }
         

        public double latestPosition { get; set; }
        public double accumDistance { get; set; }
        public double latestReadyTime { get; set; }
        public List<Job> assignedJobs { get; set; }
        public List<Stoppage> stoppages { get; private set; }
        public List<Event> scheduledEvents { get; set; }


        // Construtor
        public Machine()
        {
           
        }
        public void Init()
        {
            latestPosition = position;
            latestReadyTime = readyTime;
            assignedJobs = new List<Job>();
            stoppages = new List<Stoppage>();
            scheduledEvents = new List<Event>();

            foreach (Stoppage stoppage in Data.all_stoppages)
            {
                if (stoppage.index == index)
                {
                    //Console.WriteLine("{0}: {1} -- {2}", stoppage.index, stoppage.start, stoppage.end);
                    stoppages.Add(stoppage);
                    scheduledEvents.Add(new Event(
                        type: "Stoppage",
                        startTime: stoppage.start,
                        endTime: stoppage.end,
                        job: new Job()
                        ));
                }
            }
        }

        public object Clone()
        {
            return new Machine
            {
                index = this.index,
                position = this.position,
                latestPosition = this.latestPosition,
                accumDistance = this.accumDistance,
                readyTime = this.readyTime,
                latestReadyTime = this.latestReadyTime,
                procRate = this.procRate,
                loadingUnitCost = this.loadingUnitCost,
                isGearAccepting = this.isGearAccepting,
                isDedicated = this.isDedicated,
                dedicatedCustomer = this.dedicatedCustomer,
                isThirdParty = this.isThirdParty,
                isCompulsary = this.isCompulsary,
                assignedJobs = new List<Job>(),
                stoppages = this.stoppages,
                scheduledEvents = new List<Event>(),
        };
        }
    }
}
