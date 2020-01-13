using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public class Scheduler
    {
        public List<int> genes { get; set; }
        public List<Machine> machines { get; set; }
        public List<Job> jobs { get; set; }
        public List<OGV> ogvs { get; set; }

        public double totalCost { get; set; }
        public double travelCost { get; set; }
        public double handlingCost { get; set; }
        public double dndCost { get; set; }
        public double sumLateStart { get; set; }
        public double makespan { get; set; }
        public bool isFeasible { get; set; }
        public bool gearFeasible { get; set; }
        public bool dedicationFeasible { get; set; }


        // Construtor
        public Scheduler()
        {
            machines = new List<Machine>();
            jobs = new List<Job>();
            machines = Data.Machines.Select(x => (Machine)x.Clone()).ToList();
            foreach (Machine machine in machines) { machine.Init(); }
            jobs = Data.Jobs.Select(x => (Job)x.Clone()).ToList();
            ogvs = Data.OGVs.Select(x => (OGV)x.Clone()).ToList();
            foreach (Job job in jobs) { job.Init(ogvs); }

            genes = new List<int>(new int[machines.Count * jobs.Count]);
            genes = genes.Select(x => -1).ToList();
        }

        public void Assign(Machine machine, Job job)
        {
            job.assignedMachine = machine;
            job.ogv.assignedMachine = machine;
            machine.assignedJobs.Add(job);
            
            AssignSchedule(machine, job);
        }

        public void AssignSchedule(Machine machine, Job job)
        {

            job.startTime = Math.Max(machine.latestReadyTime, job.readyTime);

            // Calculate time needed to process Job (quantity * time needed per unit quanitit)
            job.procTime = job.quantity * machine.procRate;

            job.completeTime = job.startTime + job.procTime;

            // Downtimes consideration
            foreach (Stoppage stoppage in machine.stoppages)
            {

                if (job.startTime <= stoppage.start)
                {
                    if (job.completeTime >= stoppage.start)
                    {
                        job.procTime = job.procTime + stoppage.end - stoppage.start;
                    }
                }
                else if (job.startTime >= stoppage.start && job.startTime <= stoppage.end)
                {
                    job.startTime = stoppage.end;
                }
            }

            job.completeTime = job.startTime + job.procTime;


            if (job.demurrage > 0 && job.isOutOfLaycan == false)
            {
                job.dndTime = job.completeTime - (job.readyTime + job.requestedProcRate * job.quantity);
            }
            else
            {
                job.dndTime = job.procTime - (job.requestedProcRate * job.quantity);
            }

            getCost(machine, job);
            //machine.accumDistance += Math.Abs(machine.latestPosition - job.position);
            // Update machine
            machine.latestReadyTime = job.completeTime;
            machine.latestPosition = job.position;
        }

        public double getCost(Machine machine, Job job)
        {
            job.travelCost = Math.Abs(machine.latestPosition - job.position);
            job.handlingCost = machine.loadingUnitCost * job.quantity;
            job.dndCost = 0.00;

            if (job.isLateComplete())
            {
                job.dndCost = job.demurrage * job.dndTime; 
            }
            else
            {
                job.dndCost = job.despatch * job.dndTime;
            }

            job.totalCost = job.travelCost + job.handlingCost + job.dndCost;
            return job.totalCost;
        }

        public void genesToSchedule()
        {
            //Console.WriteLine("genes:  [ {0} ]", string.Join(",  ", genes));

            machines = new List<Machine>();
            jobs = new List<Job>();
            machines = Data.Machines.Select(x => (Machine)x.Clone()).ToList();
            jobs = Data.Jobs.Select(x => (Job)x.Clone()).ToList();
            ogvs = Data.OGVs.Select(x => (OGV)x.Clone()).ToList();
            foreach (Machine machine in machines) { machine.Init(); }
            foreach (Job job in jobs) { job.Init(ogvs); }


            for (int i = 0; i < genes.Count; i++)
            {
                if (genes[i] <= 100)
                {
                    int machineId = i / jobs.Count;
                    Assign(machines[machineId], jobs.Find(x => x.index == genes[i]));
                    //Console.WriteLine("genes:  [ {0} ]", string.Join(",  ", genes));
                    //Console.WriteLine("machineId: " + machineId);
                    //Console.WriteLine("jobId: " + genes[i]);
                }
            }

        }

        public void scheduleToGenes()
        {
            int nullCounter = 100;

            for (int i = 0; i < machines.Count; i++)
            {
                for (int j = 0; j < jobs.Count; j++)
                {
                    int slotPosition = jobs.Count * i + j;

                    if (j < machines[i].assignedJobs.Count)
                    {
                        genes[slotPosition] = machines[i].assignedJobs[j].index;
                    }
                    else
                    {
                        nullCounter += 1;
                        genes[slotPosition] = nullCounter;
                    }
                }
            }

        }

        public void SortByPriority()
        {
            for (int i = 0; i < machines.Count; i++)
            {
                for (int j = 0; j < machines[i].assignedJobs.Count; j++)
                {
                    if (j > 0 && Math.Abs(machines[i].assignedJobs[j].readyTime - machines[i].assignedJobs[j - 1].readyTime) < Data.PriorityGapTime)
                    {
                        if (machines[i].assignedJobs[j].priority > machines[i].assignedJobs[j - 1].priority)
                        {
                            if (!machines[i].assignedJobs[j].isOutOfLaycan && machines[i].assignedJobs[j].demurrage != 0)
                            {
                                //Console.WriteLine("SWAP");
                                Job tmp = machines[i].assignedJobs[j];
                                machines[i].assignedJobs[j] = machines[i].assignedJobs[j - 1];
                                machines[i].assignedJobs[j - 1] = tmp;
                                j = 0;
                            }
                        }
                    }
                }
            }

            scheduleToGenes();
            genesToSchedule();
        }

        public void SortByReadyTime()
        {
            for (int i = 0; i < machines.Count; i++)
            {
                machines[i].assignedJobs = machines[i].assignedJobs.OrderBy(o => o.readyTime).ToList();
            }

            scheduleToGenes();
            genesToSchedule();
        }

        public void GetEventSchedule()
        {
            for (int i = 0; i < machines.Count; i++)
            {
                for (int j = 0; j < machines[i].assignedJobs.Count; j++)
                {
                    machines[i].scheduledEvents.Add(new Event(
                           type: "Job",
                           startTime: machines[i].assignedJobs[j].startTime,
                           endTime: machines[i].assignedJobs[j].completeTime,
                           job: machines[i].assignedJobs[j]
                       ));
                }
                machines[i].scheduledEvents = machines[i].scheduledEvents.OrderBy(o => o.startTime).ToList();
                for (int k = 0; k < machines[i].scheduledEvents.Count; k++)
                {
                    machines[i].scheduledEvents[k].index = k;
                }
            }

        }

        public void GetSchedule()
        {
            if (Data.objectiveCase == 1)
            {
                SortByPriority();
            }
            else
            {
                SortByReadyTime();
            }

            totalCost = 0;
            travelCost = 0;
            handlingCost = 0;
            dndCost = 0;
            sumLateStart = 0;

            for (int i = 0; i < machines.Count; i++)
            {
                for (int j = 0; j < machines[i].assignedJobs.Count; j++)
                {
                    machines[i].assignedJobs[j].lateStartTime = machines[i].assignedJobs[j].startTime - machines[i].assignedJobs[j].readyTime;
                    sumLateStart += machines[i].assignedJobs[j].lateStartTime;

                    //Cost calculation

                    travelCost += machines[i].assignedJobs[j].travelCost;
                    handlingCost += machines[i].loadingUnitCost * machines[i].assignedJobs[j].quantity;

                    if (machines[i].assignedJobs[j].isLateComplete())
                    {
                        dndCost += machines[i].assignedJobs[j].demurrage * machines[i].assignedJobs[j].dndTime;
                    }
                    else
                    {
                        dndCost += machines[i].assignedJobs[j].despatch * machines[i].assignedJobs[j].dndTime;
                    }
                }
            }

            GetEventSchedule();

            totalCost = travelCost + handlingCost + dndCost;
            makespan = jobs.Select(x => x.completeTime).Max();
        }

        public bool IsOverallFeasible()
        {
            bool isFeasible = false;

            for (int i = 0; i < machines.Count; i++)
            {
                if (Data.IsAllMachinesUtilized && machines.Count(mc => !mc.isThirdParty) <= jobs.Count && !machines[i].isThirdParty && machines[i].assignedJobs.Count == 0)
                {
                    return false;
                }

                if (machines.Count >= jobs.Count && machines[i].isCompulsary && machines[i].assignedJobs.Count == 0)
                {
                    return false;
                }

                for (int j = 0; j < machines[i].assignedJobs.Count; j++)
                {
                    if (IsFeasible(machines[i], machines[i].assignedJobs[j]))
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
            Data.DedicationType dedicationType = (Data.DedicationType)Data.DedicationCase;

            if (IsGearFeasible(machine, job) && IsUnloadingFeasible(machine, job) && IsBargeFeasible(machine, job) && IsOGVFeasible(machine, job))
            {
                switch (dedicationType)
                {
                    case Data.DedicationType.Flexible:
                        if (IsFlexDedicationFeasible(machine, job))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Data.DedicationType.Strict:
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
            if (job.isGeared)
            {
                if (machine.isGearAccepting)
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
            if (job.isDedicated)
            {
                if (!machine.isDedicated)
                {
                    return false;
                }
                else if (machine.dedicatedCustomer != job.shipper)
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
            if (job.isDedicated)
            {
                if (machine.dedicatedCustomer == job.shipper)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (machine.isDedicated)
            {
                if (machine.dedicatedCustomer == job.shipper)
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
            if (job.isUnloading)
            {
                if (machine.index == job.machineIdUnload)
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
            if (job.isBarge)
            {
                if (machine.index == job.machineIdBarge)
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

        public double getEarliersJobStartTime (Machine machine, Job job)
        {
            double startTime;
            double procTime;
            double completeTime;

            startTime = Math.Max(machine.latestReadyTime, job.readyTime);

            // Calculate time needed to process Job (quantity * time needed per unit quanitit)
            procTime = job.quantity * machine.procRate;

            completeTime = startTime + procTime;

            // Downtimes consideration
            foreach (Stoppage stoppage in machine.stoppages)
            {

                if (startTime <= stoppage.start)
                {
                    if (completeTime >= stoppage.start)
                    {
                        procTime = procTime + stoppage.end - stoppage.start;
                    }
                }
                else if (startTime >= stoppage.start && startTime <= stoppage.end)
                {
                    startTime = stoppage.end;
                }
            }

            completeTime = startTime + procTime;

            return startTime;
        }

        public bool IsOGVFeasible(Machine machine, Job job)
        {
            Machine targetMachine = new Machine();
            Job lastJob = new Job();



            if (job.ogv.assignedMachine.index == -1)
            {
                return true;
            }

            foreach (Machine _machine in machines)
            {
                foreach (Job _job in machine.assignedJobs)
                {
                    if (job.ogv.index == _job.ogv.index)
                    {
                        targetMachine = _machine;
                        lastJob = _job;
                    }
                }
            }


            if (job.ogv.assignedMachine.index == machine.index)
            {
                return true;
            }
            else
            {
                if (getEarliersJobStartTime(machine, job) - lastJob.completeTime > Data.InterrupedSetUpTime)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

    }
}
