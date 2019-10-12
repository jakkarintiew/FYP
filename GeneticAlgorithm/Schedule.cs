using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Schedule
    {
        public List<int> assignment { get; private set; }
        public List<Machine> machines { get; private set; }
        public List<Job> jobs { get; private set; }
        public double cost { get; private set; }
        public double makespan { get; private set; }
        public bool isFeasible { get; private set; }

        // Construtor
        public Schedule(List<int> assignment)
        {
            // Initialize a new list of Machines
            machines = Data.InitMachines();
            // Initialize a new list of Jobs
            jobs = Data.InitJobs();

            this.assignment = assignment;

            isFeasible = false;


            for (int j = 0; j  < jobs.Count; j ++)
            {
                // Assgin a fresh Machine to the Job
                Assign(machines[assignment[j]], jobs[j]);
                cost += Data.costs[assignment[j], j];
                isFeasible = true;
            }

            makespan = jobs.Select(x => x.completeTime).Max();

        }

        private void Assign(Machine machine, Job job)
        {

            double processingTime;
            job.assignedMachine = machine;
            job.startTime = Math.Max(machine.readyTime, job.readyTime);

            // Calculate time needed to process Job (quantity * time needed per unit quanitit)
            processingTime = job.quantity * machine.procRate;
            job.completeTime = job.startTime + processingTime;

            // Update new readyTime for machine
            machine.readyTime = job.completeTime;

            // Push job to assignedJobs list
            machine.assignedJobs.Add(job);

        }
    }
}
