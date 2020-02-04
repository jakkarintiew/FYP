using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    public class Machine: ICloneable
    {
        public int Index { get; set; }
        //public double Position { get; set; }
        public double OriginalLatitude { get; set; }
        public double OriginalLongitude { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double OriginalReadyTime { get; set; }
        public double ProcRate { get; set; }
        public double LoadingUnitCost { get; set; }
        public bool IsGearAccepting { get; set; }
        public bool IsDedicated { get; set; }
        public string DedicatedCustomer { get; set; }

        // third-party prop
        public bool IsThirdParty { get; set; }
        public bool IsCompulsary { get; set; }
        public double RentalUnitCost { get; set; }

        public double Speed { get; set; }
        public double TravelTime { get; set; }
        public double CumulatedTravelTime { get; set; }

        //public double LatestPosition { get; set; }
        public double LatestLatitude { get; set; }
        public double LatestLongitude { get; set; }
        public double CumulatedDistance { get; set; }
        public double LatestReadyTime { get; set; }
        public List<Job> AssignedJobs { get; set; }
        public List<Stoppage> Stoppages { get; private set; }
        public List<Event> ScheduledEvents { get; set; }

        public double TotalCost { get; set; }
        public double TravelCost { get; set; }
        public double HandlingCost { get; set; }
        public double RentalCost { get; set; }
        public double DndCost { get; set; }
        public double Makespan { get; set; }

        // Construtor
        public Machine()
        {
            Index = -1;
        }
        public void Init()
        {
            AssignedJobs = new List<Job>();
            Stoppages = new List<Stoppage>();
            ScheduledEvents = new List<Event>();

            if (!IsThirdParty)
            {
                foreach (Stoppage stoppage in Settings.AllStoppages) if (stoppage.Index == Index)
                    {
                        Stoppages.Add(stoppage);
                        ScheduledEvents.Add(new Event(
                            type: "Stoppage",
                            startTime: stoppage.StartTime,
                            endTime: stoppage.EndTime,
                            job: new Job()
                            ));;
                    }
            }
            else
            {
                OriginalLatitude = Settings.ThirdMachineLatitude;
                OriginalLongitude = Settings.ThirdMachineLongitude;
                Speed = Settings.ThirdMachineSpeed;
                OriginalReadyTime = Settings.ThirdMachineReadyTime;
                ProcRate = Settings.ThirdMachineProcRate;
                RentalUnitCost = Settings.ThirdMachineRentalCost;
                IsDedicated = false;
                DedicatedCustomer = "notdedicated";
                IsGearAccepting = true;
            }

            Latitude = OriginalLatitude;
            Longitude = OriginalLongitude;
            LatestReadyTime = OriginalReadyTime;

        }

        public object Clone()
        {
            return new Machine
            {
                Index = this.Index,
                OriginalLatitude = this.OriginalLatitude,
                OriginalLongitude = this.OriginalLongitude,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                CumulatedDistance = this.CumulatedDistance,
                Speed = this.Speed,
                TravelTime = this.TravelTime,
                CumulatedTravelTime = this.CumulatedTravelTime,
                OriginalReadyTime = this.OriginalReadyTime,
                LatestReadyTime = this.LatestReadyTime,
                ProcRate = this.ProcRate,
                LoadingUnitCost = this.LoadingUnitCost,
                IsGearAccepting = this.IsGearAccepting,
                IsDedicated = this.IsDedicated,
                DedicatedCustomer = this.DedicatedCustomer,
                IsThirdParty = this.IsThirdParty,
                IsCompulsary = this.IsCompulsary,
                RentalUnitCost = this.RentalUnitCost,
                AssignedJobs = new List<Job>(),
                Stoppages = this.Stoppages,
                ScheduledEvents = new List<Event>(),
                Makespan = this.Makespan,
        };
        }
    }
}
