using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public class Scheduler
    {
        public List<int> Genes { get; set; }
        public List<Machine> Machines { get; set; }
        public List<Job> Jobs { get; set; }
        public List<OGV> OGVs { get; set; }

        public double TotalCost { get; set; }
        public double TravelCost { get; set; }
        public double HandlingCost { get; set; }
        public double RentalCost { get; set; }
        public double DndCost { get; set; }
        public double SumLateStart { get; set; }
        public double Makespan { get; set; }
        public bool GearFeasible { get; set; }
        public bool DedicationFeasible { get; set; }


        // Construtor
        public Scheduler()
        {
            InitObjects();
            Genes = new List<int>(Enumerable.Repeat(-1, Settings.NumAllMachines * Settings.NumJobs));
            //Genes = genes;            
        }
        
        public void InitObjects()
        {   
            Machines = new List<Machine>();
            Jobs = new List<Job>();
            OGVs = new List<OGV>();

            Machines = Settings.Machines.Select(x => (Machine)x.Clone()).ToList();
            Jobs = Settings.Jobs.Select(x => (Job)x.Clone()).ToList();
            OGVs = Settings.OGVs.Select(x => (OGV)x.Clone()).ToList();
            foreach (Machine machine in Machines) { machine.Init(); }
            foreach (Job job in Jobs) { job.Init(OGVs); }
        }

        public void Assign(Machine machine, Job job)
        {
            job.AssignedMachine = machine;
            job.Ogv.AssignedMachine = machine;
            machine.AssignedJobs.Add(job);
            machine.AssignedJobIds.Add(job.Index);
            UpdateSchedule(machine, job);
        }

        public void UpdateSchedule(Machine machine, Job job)
        {
            // Travelling
            machine.TravelTime = CalculateDistance(job.Latitude, job.Latitude, machine.Latitude, machine.Longitude) / machine.Speed;
            machine.Latitude = job.Latitude;
            machine.Longitude = job.Longitude;
            machine.CumulatedTravelTime += machine.TravelTime;
            machine.CumulatedDistance += CalculateDistance(job.Latitude, job.Latitude, machine.Latitude, machine.Longitude);

            Event travel = new Event(
                            type: "Travel",
                            startTime: machine.LatestReadyTime,
                            endTime: machine.LatestReadyTime + machine.TravelTime,
                            job: new Job()
                            );

            // Downtimes consideration
            foreach (Stoppage stoppage in machine.Stoppages)
            {
                if (travel.StartTime < stoppage.EndTime && stoppage.StartTime < travel.EndTime)
                {
                    travel.StartTime = stoppage.EndTime;
                    travel.EndTime = travel.StartTime + machine.TravelTime;
                }
            }
            machine.ScheduledEvents.Add(travel);
            machine.LatestReadyTime = travel.EndTime;

            // Anchorage
            Event anchorage = new Event(
                           type: "Anchorage",
                           startTime: machine.LatestReadyTime,
                           endTime: machine.LatestReadyTime + Settings.AnchorageTime,
                           job: new Job()
                           );

            machine.ScheduledEvents.Add(anchorage);

            machine.LatestReadyTime = anchorage.EndTime;

            job.StartTime = Math.Max(machine.LatestReadyTime, job.ReadyTime);
            job.ProcTime = job.Quantity * machine.ProcRate;
            job.CompleteTime = job.StartTime + job.ProcTime;
            
            // Downtimes consideration
            foreach (Stoppage stoppage in machine.Stoppages)
            {
                if (job.StartTime <= stoppage.StartTime)
                {
                    if (job.CompleteTime >= stoppage.StartTime)
                    {
                        job.ProcTime = job.ProcTime + stoppage.EndTime - stoppage.StartTime;
                    }
                }
                else if (job.StartTime >= stoppage.StartTime && job.StartTime <= stoppage.EndTime)
                {
                    job.StartTime = stoppage.EndTime;
                }
            }

            job.CompleteTime = job.StartTime + job.ProcTime;
            job.LateStartTime = job.StartTime - job.ReadyTime;

            if (job.Demurrage > 0 && job.IsOutOfLaycan == false)
            {
                job.DndTime = job.CompleteTime - (job.ReadyTime + job.RequestedProcRate * job.Quantity);
            }
            else
            {
                job.DndTime = job.ProcTime - (job.RequestedProcRate * job.Quantity);
            }

            job.TravelCost = CalculateDistance(job.Latitude, job.Latitude, machine.Latitude, machine.Longitude);
            job.HandlingCost = machine.LoadingUnitCost * job.Quantity;
            job.RentalCost = machine.RentalUnitCost * job.Quantity;
            if (job.IsLateComplete())
            {
                job.DndCost = job.Demurrage * job.DndTime;
            }
            else
            {
                job.DndCost = job.Despatch * job.DndTime;
            }

            //Console.WriteLine(job.DndCost);

            job.TotalCost = job.TravelCost + job.HandlingCost + job.RentalCost + job.DndCost;

            // Casting Off
            Event castingOff = new Event(
                            type: "CastingOff",
                            startTime: job.CompleteTime,
                            endTime: job.CompleteTime + Settings.CastingOffTime,
                            job: new Job()
                            );
            machine.ScheduledEvents.Add(castingOff);
            machine.LatestReadyTime = castingOff.EndTime;
        }

        public double CalculateDistance(double x1, double y1, double x2, double y2) => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        public double CalculateIncrementalFitness(Machine machine, Job job)
        {
            Settings.ObjetiveFunction objetiveFunction = (Settings.ObjetiveFunction)Settings.ObjectiveCase;

            double incrementalFitness = 0;
            double travelCost;
            double handlingCost;
            double rentalCost;
            double dndCost;
            double startTime;
            double completeTime;
            double lateStartTime;
            double increasedMakespan;

            switch (objetiveFunction)
            {
                case Settings.ObjetiveFunction.TotalCostWithPriority:
                case Settings.ObjetiveFunction.TotalCostNoPriority:
                    travelCost = CalculateDistance(job.Latitude, job.Latitude, machine.Latitude, machine.Longitude);
                    handlingCost = machine.LoadingUnitCost * job.Quantity;
                    rentalCost = machine.RentalUnitCost * job.Quantity;
                    if (job.IsLateComplete())
                    {
                        dndCost = job.Demurrage * job.DndTime;
                    }
                    else
                    {
                        dndCost = job.Despatch * job.DndTime;
                    }

                    TotalCost = travelCost + handlingCost + rentalCost + dndCost;
                    incrementalFitness = TotalCost;
                    break;

                case Settings.ObjetiveFunction.DemurrageDespatchCost:
                    if (job.IsLateComplete())
                    {
                        dndCost = job.Demurrage * job.DndTime;
                    }
                    else
                    {
                        dndCost = 1;
                    }

                    incrementalFitness = dndCost;
                    break;

                case Settings.ObjetiveFunction.SumLateStart:
                    startTime = GetJobStartCompleteTime(machine, job).Item1;
                    lateStartTime = startTime - job.ReadyTime;
                    incrementalFitness = lateStartTime;
                    break;
                case Settings.ObjetiveFunction.Makespan:
                    completeTime = GetJobStartCompleteTime(machine, job).Item2;
                    increasedMakespan = Math.Max(0, completeTime - Jobs.Select(x => x.CompleteTime).Max());
                    incrementalFitness = increasedMakespan;
                    break;
            }

            return incrementalFitness;
        }

        public void MachineJobListToGenes()
        {
            int nullCounter = 100;

            for (int i = 0; i < Machines.Count; i++)
            {
                for (int j = 0; j < Jobs.Count; j++)
                {
                    int slotPosition = Jobs.Count * i + j;

                    if (j < Machines[i].AssignedJobIds.Count)
                    {
                        Genes[slotPosition] = Machines[i].AssignedJobIds[j];
                    }
                    else
                    {
                        nullCounter += 1;
                        Genes[slotPosition] = nullCounter;
                    }
                }
            }
        }

        public void GenesToSchedule()
        {
            InitObjects();

            for (int i = 0; i < Genes.Count; i++)
            {
                if (Genes[i] < 100)
                {
                    int machineId = i / Jobs.Count;
                    Assign(Machines[machineId], Jobs.Find(x => x.Index == Genes[i]));
                }
            }

        }

        public void ScheduleToGenes()
        {
            int nullCounter = 100;

            for (int i = 0; i < Machines.Count; i++)
            {
                for (int j = 0; j < Jobs.Count; j++)
                {
                    int slotPosition = Jobs.Count * i + j;

                    if (j < Machines[i].AssignedJobs.Count)
                    {
                        Genes[slotPosition] = Machines[i].AssignedJobs[j].Index;
                    }
                    else
                    {
                        nullCounter += 1;

                        //Console.WriteLine(nullCounter);
                        //Console.WriteLine(slotPosition);
                        //Console.WriteLine(string.Join(",", Genes));
                        Genes[slotPosition] = nullCounter;
                    }
                }
            }

        }

        public void SortByPriority()
        {
            for (int i = 0; i < Machines.Count; i++)
            {
                if (Machines[i].AssignedJobs.Count > 1)
                {
                    for (int j = 1; j < Machines[i].AssignedJobs.Count; j++)
                    {
                        if (Math.Abs(Machines[i].AssignedJobs[j].ReadyTime - Machines[i].AssignedJobs[j - 1].ReadyTime) < Settings.PriorityGapTime)
                        {
                            if (Machines[i].AssignedJobs[j].Priority > Machines[i].AssignedJobs[j - 1].Priority)
                            {
                                if (!Machines[i].AssignedJobs[j].IsOutOfLaycan && Machines[i].AssignedJobs[j].Demurrage != 0)
                                {
                                    //Console.WriteLine("SWAP");
                                    Job tmp = Machines[i].AssignedJobs[j];
                                    Machines[i].AssignedJobs[j] = Machines[i].AssignedJobs[j - 1];
                                    Machines[i].AssignedJobs[j - 1] = tmp;
                                    j = 1;
                                }
                            }
                        }
                    }
                }
            }

            ScheduleToGenes();
            GenesToSchedule();
        }

        public void SortByReadyTime()
        {
            for (int i = 0; i < Machines.Count; i++)
            {
                Machines[i].AssignedJobs = Machines[i].AssignedJobs.OrderBy(o => o.ReadyTime).ToList();
            }

            ScheduleToGenes();
            GenesToSchedule();
        }

        public void GetEventSchedule()
        {
            for (int i = 0; i < Machines.Count; i++)
            {
                for (int j = 0; j < Machines[i].AssignedJobs.Count; j++)
                {
                    Machines[i].ScheduledEvents.Add(new Event(
                        type: "Job",
                        startTime: Machines[i].AssignedJobs[j].StartTime,
                        endTime: Machines[i].AssignedJobs[j].CompleteTime,
                        job: Machines[i].AssignedJobs[j]
                    ));
                }
                Machines[i].ScheduledEvents = Machines[i].ScheduledEvents.OrderBy(o => o.StartTime).ToList();
                for (int k = 0; k < Machines[i].ScheduledEvents.Count; k++)
                {
                    Machines[i].ScheduledEvents[k].Index = k;
                }
            }

        }

        public void GetOverallSchedule()
        {
            if (Settings.ObjectiveCase == 1)
            {
                SortByPriority();
            }

            TravelCost = Machines.Sum(m => m.AssignedJobs.Sum(j => j.TravelCost));
            HandlingCost = Machines.Sum(m => m.AssignedJobs.Sum(j => j.HandlingCost));
            RentalCost = Machines.Sum(m => m.AssignedJobs.Sum(j => j.RentalCost));
            DndCost = Machines.Sum(m => m.AssignedJobs.Sum(j => j.DndCost));
            TotalCost = Machines.Sum(m => m.AssignedJobs.Sum(j => j.TotalCost));
            SumLateStart = Machines.Sum(m => m.AssignedJobs.Sum(j => j.LateStartTime));
            Makespan = Jobs.Select(x => x.CompleteTime).Max();

            for (int i = 0; i < Machines.Count; i++)
            {
                Machines[i].TravelCost = Machines[i].AssignedJobs.Sum(x => x.TravelCost);
                Machines[i].HandlingCost = Machines[i].AssignedJobs.Sum(x => x.HandlingCost);
                Machines[i].RentalCost = Machines[i].AssignedJobs.Sum(x => x.RentalCost);
                Machines[i].DndCost = Machines[i].AssignedJobs.Sum(x => x.DndCost);
                Machines[i].TotalCost = Machines[i].AssignedJobs.Sum(x => x.TotalCost);
            }

            GetEventSchedule();
        }

        public bool IsOverallFeasible()
        {
            bool isFeasible = true;

            foreach (Machine machine in Machines)
            {

                if (!machine.IsThirdParty)
                {
                    if (Settings.IsAllMachinesUtilized && machine.AssignedJobs.Count == 0)
                    {
                        return false;
                    }
                }
                else if (machine.IsCompulsary && machine.AssignedJobs.Count == 0)
                {
                    return false;
                }

                foreach (Job job in machine.AssignedJobs)
                {
                    if (IsFeasible(machine, job))
                    {
                        isFeasible = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return isFeasible;
        }

        public bool IsFeasible(Machine machine, Job job)
        {
            Settings.DedicationType dedicationType = (Settings.DedicationType)Settings.DedicationCase;


            //if (!IsGearFeasible(machine, job)) { Console.WriteLine("FAILED GEAR"); }
            //if (!IsUnloadingFeasible(machine, job)) { Console.WriteLine("FAILED UNLOADING"); }
            //if (!IsBargeFeasible(machine, job)) { Console.WriteLine("FAILED BARGE"); }
            //if (!IsOGVFeasible(machine, job)) { Console.WriteLine("FAILED OGV"); }
            //if (!IsFlexDedicationFeasible(machine, job)) { Console.WriteLine("FAILED DEDICATION"); }


            if (IsGearFeasible(machine, job) && IsUnloadingFeasible(machine, job) && IsBargeFeasible(machine, job) && IsOGVFeasible(machine, job))
            {
                switch (dedicationType)
                {
                    case Settings.DedicationType.Flexible:
                        if (IsFlexDedicationFeasible(machine, job))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Settings.DedicationType.Strict:
                        if (IsStrictDedicationFeasible(machine, job))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                }

            }
            else
            {
                return false;
            }

            return false;
        }

        public bool IsGearFeasible(Machine machine, Job job)
        {
            if (job.IsGeared)
            {
                if (machine.IsGearAccepting)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public bool IsFlexDedicationFeasible(Machine machine, Job job)
        {
            if (job.IsDedicated)
            {
                if (!machine.IsDedicated)
                {
                    return false;
                }
                else if (machine.DedicatedCustomer != job.Shipper)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }

        public bool IsStrictDedicationFeasible(Machine machine, Job job)
        {
            if (job.IsDedicated)
            {
                if (machine.DedicatedCustomer == job.Shipper)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (machine.IsDedicated)
            {
                if (machine.DedicatedCustomer == job.Shipper)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsUnloadingFeasible(Machine machine, Job job)
        {
            if (job.IsUnloading)
            {
                if (machine.Index == job.MachineIdUnload)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public bool IsBargeFeasible(Machine machine, Job job)
        {
            if (job.IsBarge)
            {
                if (machine.Index == job.MachineIdBarge)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public Tuple<double, double> GetJobStartCompleteTime (Machine machine, Job job)
        {
            double startTime;
            double procTime;
            double completeTime;

            startTime = Math.Max(machine.LatestReadyTime, job.ReadyTime);

            // Calculate time needed to process Job (quantity * time needed per unit quanitit)
            procTime = job.Quantity * machine.ProcRate;

            completeTime = startTime + procTime;

            // Downtimes consideration
            foreach (Stoppage stoppage in machine.Stoppages)
            {

                if (startTime <= stoppage.StartTime)
                {
                    if (completeTime >= stoppage.StartTime)
                    {
                        procTime = procTime + stoppage.EndTime - stoppage.StartTime;
                    }
                }
                else if (startTime >= stoppage.StartTime && startTime <= stoppage.EndTime)
                {
                    startTime = stoppage.EndTime;
                }
            }

            completeTime = startTime + procTime;

            return Tuple.Create(startTime, completeTime);
        }

        public bool IsOGVFeasible(Machine machine, Job job)
        {

            if (job.Ogv.AssignedMachine.Index == -1 || job.Ogv.AssignedMachine.Index == machine.Index)
            {
                return true;
            }
            else
            {
                Job prevJob = new Job();
                Job nextJob = new Job();
                double jobStartTime;
                double jobCompleteTime;

                if (job.AssignedMachine == null)
                {
                    jobStartTime = GetJobStartCompleteTime(machine, job).Item1;
                    jobCompleteTime = GetJobStartCompleteTime(machine, job).Item2;
                }
                else
                {
                    jobStartTime = job.StartTime;
                    jobCompleteTime = job.CompleteTime;
                }

                prevJob = Jobs.Where(x => x.OgvId == job.OgvId).Where(y => y.CompleteTime < jobStartTime && y.CompleteTime > 0).ToList().Count != 0 ? Jobs.Where(x => x.OgvId == job.OgvId).Where(y => y.CompleteTime < jobStartTime && y.CompleteTime > 0).Last() : null;
                nextJob = Jobs.Where(x => x.OgvId == job.OgvId).Where(y => y.StartTime    > jobCompleteTime).ToList().Count != 0 ? Jobs.Where(x => x.OgvId == job.OgvId).Where(y => y.StartTime > jobCompleteTime).First() : null;

                if (prevJob != null)
                {
                    //Console.WriteLine("machine.Index = {0}, prevJob.Index = {1} job.Index = {2}", machine.Index, prevJob.Index, job.Index);
                    //Console.WriteLine("prevJob.CompleteTime = {0}, job.StartTime = {1}", prevJob.CompleteTime, jobStartTime);

                    if (Math.Abs(jobStartTime - prevJob.CompleteTime) > Settings.InterrupedSetUpTime)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (nextJob != null)
                {
                    //Console.WriteLine("machine.Index = {0}, nextJob.Index = {1} job.Index = {2}", machine.Index, nextJob.Index, job.Index);
                    //Console.WriteLine("job.CompleteTime = {0}, nextJob.StartTime = {1}", jobCompleteTime, nextJob.StartTime);

                    if (Math.Abs(nextJob.StartTime - jobCompleteTime) > Settings.InterrupedSetUpTime)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
        }

    }
}
