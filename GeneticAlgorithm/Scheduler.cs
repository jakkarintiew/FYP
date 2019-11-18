using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Scheduler
    {
        public List<int> assignment { get;  set; }
        public List<Machine> machines { get;  set; }
        public List<Job> jobs { get;  set; }
        public double cost { get; set; }
        public double makespan { get;  set; }
        public bool isFeasible { get;  set; }
        public bool gearFeasible { get; set; }
        public  bool dedicationFeasible { get; set; }


        // Construtor
        public Scheduler()
        {
            machines = Data.InitMachines();
            jobs = Data.InitJobs();
        }

        public void Assign(Machine machine, Job job)
        {
            job.assignedMachine = machine;
            machine.assignedJobs.Add(job);
        }

        public void AssignSchedule(Machine machine, Job job)
        {

            job.startTime = Math.Max(machine.latestReadyTime, job.readyTime);

            // Calculate time needed to process Job (quantity * time needed per unit quanitit)
            double processingTime = job.quantity * machine.procRate;

            job.completeTime = job.startTime + processingTime;

            // Downtimes consideration
            for (int i = 0; i < machine.downTimes.GetLength(0); i++)
            {
                if (job.startTime <= machine.downTimes[i, 0])
                {
                    if (job.completeTime >= machine.downTimes[i, 0])
                    {
                        processingTime = processingTime + machine.downTimes[i, 1] - machine.downTimes[i, 0];
                    }
                }
                else if (job.startTime >= machine.downTimes[i, 0] && job.startTime <= machine.downTimes[i, 1])
                {
                    job.startTime = machine.downTimes[i, 1];
                }
            }

            job.completeTime = job.startTime + processingTime;

            // Update new readyTime for machine
            machine.latestReadyTime = job.completeTime;
        }

        public void GetSchedule(List<int> assignment)
        {
            this.assignment = assignment;

            for (int j = 0; j < jobs.Count; j++)
            {
                Assign(machines[assignment[j]], jobs[j]);
                AssignSchedule(machines[assignment[j]], jobs[j]);
                cost += Data.cost_mat[assignment[j], j];
            }

            SwapByPriority();
            GetEventSchedule();
            makespan = jobs.Select(x => x.completeTime).Max();
        }

        public bool IsFeasible(Machine machine, Job job)
        {
            Data.objetiveFunction objetiveFunction = (Data.objetiveFunction)Data.objectiveCase;
            Data.dedicationType dedicationType = (Data.dedicationType)Data.dedicationCase;

            if (IsGearFeasible(machine, job))
            {
                switch (dedicationType)
                {
                    case Data.dedicationType.Felxible:
                        if (IsFlexDedicationFeasible(machine, job))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Data.dedicationType.Strict:
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
            } else

            return true;
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
            }

            return true;
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

        public void SwapByPriority()
        {
            for (int i = 0; i < machines.Count; i++)
            {
                for (int j = 0; j < machines[i].assignedJobs.Count; j++)
                {
                    if (j > 0 && Math.Abs(machines[i].assignedJobs[j].readyTime - machines[i].assignedJobs[j-1].readyTime) < 10800)
                    {
                        if (machines[i].assignedJobs[j].priority > machines[i].assignedJobs[j-1].priority)
                        {
                            //Console.WriteLine("SWAP");
                            Job tmp = machines[i].assignedJobs[j];
                            machines[i].assignedJobs[j] = machines[i].assignedJobs[j-1];
                            machines[i].assignedJobs[j-1] = tmp;
                        }
                    }
                }
            }

            for (int i = 0; i < machines.Count; i++)
            {
                machines[i].latestReadyTime = machines[i].readyTime;
                for (int j = 0; j < machines[i].assignedJobs.Count; j++)
                {
                    AssignSchedule(machines[i], machines[i].assignedJobs[j]);
                }
            }
        }
    }
}
