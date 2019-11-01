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

        // Construtor
        public Schedule()
        {
            // Initialize a new list of Machines
            machines = Data.InitMachines();
            // Initialize a new list of Jobs
            jobs = Data.InitJobs();
            isFeasible = false;
        }

        public void Assign(Machine machine, Job job)
        {

            double processingTime;
            job.assignedMachine = machine;
            job.startTime = Math.Max(machine.readyTime, job.readyTime);

            // Calculate time needed to process Job (quantity * time needed per unit quanitit)
            processingTime = job.quantity * machine.procRate;
            GetJobCompleteTime(machine, job, processingTime);

            // Update new readyTime for machine
            machine.readyTime = job.completeTime;

            // Push job to assignedJobs list
            machine.assignedJobs.Add(job);

        }

        public void GetJobCompleteTime(Machine machine, Job job, double processingTime)
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

            job.completeTime = job.startTime + processingTime;

            return;
        }

        public void GetSchedule(List<int> assignment)
        {
            this.assignment = assignment;

            isFeasible = false;

            for (int j = 0; j < jobs.Count; j++)
            {
                // Assgin a fresh Machine to the Job
                Assign(machines[assignment[j]], jobs[j]);
                cost += Data.cost_mat[assignment[j], j];
                isFeasible = true;
            }

            makespan = jobs.Select(x => x.completeTime).Max();
        }
    }
}
