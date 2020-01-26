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
        public double rentalCost { get; set; }

        public double latestPosition { get; set; }
        public double accumDistance { get; set; }
        public double latestReadyTime { get; set; }
        public List<Job> assignedJobs { get; set; }
        public List<Stoppage> stoppages { get; private set; }
        public List<Event> scheduledEvents { get; set; }


        // Construtor
        public Machine()
        {
            index = -1;
            //position = -1;
            //readyTime = -1;
            //procRate = -1;
            //loadingUnitCost = -1;
            //isGearAccepting = false;
            //isDedicated = false;
            //isThirdParty = false;
            //isCompulsary = false;
            //rentalCost = 0;
            //dedicatedCustomer = "";
        }
        public void Init()
        {
            assignedJobs = new List<Job>();
            stoppages = new List<Stoppage>();
            scheduledEvents = new List<Event>();

            if (!isThirdParty)
            {
                foreach (Stoppage stoppage in Data.AllStoppages) if (stoppage.index == index)
                    {
                        stoppages.Add(stoppage);
                        scheduledEvents.Add(new Event(
                            type: "Stoppage",
                            startTime: stoppage.start,
                            endTime: stoppage.end,
                            job: new Job()
                            ));
                    }
            }
            else
            {
                position = Data.ThirdMachinePosition;
                readyTime = Data.ThirdMachineReadyTime;
                procRate = Data.ThirdMachineProcRate;
                rentalCost = Data.ThirdMachineRentalCost;
                isDedicated = false;
                dedicatedCustomer = "notdedicated";
                isGearAccepting = true;
            }

            latestPosition = position;
            latestReadyTime = readyTime;

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
                rentalCost = this.rentalCost,
                assignedJobs = new List<Job>(),
                stoppages = this.stoppages,
                scheduledEvents = new List<Event>(),
        };
        }
    }
}
