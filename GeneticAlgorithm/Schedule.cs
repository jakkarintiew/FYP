using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Schedule
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
        public Schedule()
        {
            machines = Data.InitMachines();
            jobs = Data.InitJobs();
        }

        public void Assign(Machine machine, Job job)
        {

            double processingTime;
            job.assignedMachine = machine;
            job.startTime = Math.Max(machine.readyTime, job.readyTime);

            // Calculate time needed to process Job (quantity * time needed per unit quanitit)
            processingTime = job.quantity * machine.procRate;
            job.completeTime = GetJobCompleteTime(machine, job, processingTime);

            // Update new readyTime for machine
            machine.readyTime = job.completeTime;

            // Push job to assignedJobs list
            machine.assignedJobs.Add(job);

        }

        public double GetJobCompleteTime(Machine machine, Job job, double processingTime)
        {
            job.completeTime = job.startTime + processingTime;

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

            return job.startTime + processingTime;
        }

        public void GetSchedule(List<int> assignment)
        {
            this.assignment = assignment;
            gearFeasible = true;
            dedicationFeasible = true;

            for (int j = 0; j < jobs.Count; j++)
            {
                Assign(machines[assignment[j]], jobs[j]);
                cost += Data.cost_mat[assignment[j], j];
                if (!GearedContraintCheck(jobs[j].assignedMachine, jobs[j]))
                {
                    gearFeasible = false;

                }

                if (!FlexDedicationContraintCheck(jobs[j].assignedMachine, jobs[j]))
                {
                    dedicationFeasible = false;
                }
            }

            if (gearFeasible && dedicationFeasible)
            {
                isFeasible = true;
            }
            else
            {
                isFeasible = false;
            }

            SwapByPriority();

            makespan = jobs.Select(x => x.completeTime).Max();
        }

        public bool GearedContraintCheck(Machine machine, Job job)
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

        public bool FlexDedicationContraintCheck(Machine machine, Job job)
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

        public bool StrictDedicationContraintCheck(Machine machine, Job job)
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
        }
    }
}
