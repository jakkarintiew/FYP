using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Schedule
    {
        public List<int> assignment { get; set; }
        public List<Machine> machines { get; set; }
        public List<Job> jobs { get; set; }
        public bool isFeasible { get; private set; }

        // Construtor
        public Schedule(List<int> assignment)
        {
            // Initialize a new list of Machines
            machines = InitMachines();
            // Initialize a new list of Jobs
            jobs = InitJobs();

            this.assignment = assignment;

            isFeasible = false;

            for (int j = 0; j  < jobs.Count; j ++)
            {
                if (machines[assignment[j]].readyTime <= jobs[j].readyTime)
                {
                    // Assgin a fresh Machine to the Job
                    Assign(machines[assignment[j]], jobs[j]);
                    isFeasible = true;
                }
                else
                {
                    isFeasible = false;
                    break; 
                }
            }
        }

        private List<Machine> InitMachines()
        {
            List<Machine> newMachines = new List<Machine>();

            newMachines.Add(new Machine(index: 0, readyTime: 0.0, procRate: 1.0));
            newMachines.Add(new Machine(index: 1, readyTime: 0.0, procRate: 0.8));
            newMachines.Add(new Machine(index: 2, readyTime: 0.0, procRate: 1.2));
            newMachines.Add(new Machine(index: 3, readyTime: 0.0, procRate: 0.6));
            newMachines.Add(new Machine(index: 4, readyTime: 0.0, procRate: 1.5));

            return newMachines;
        }

        private List<Job> InitJobs()
        {
            List<Job> newJobs = new List<Job>();

            newJobs.Add(new Job(index: 0, readyTime: 0.00, quantity: 90.00));
            newJobs.Add(new Job(index: 1, readyTime: 30.00, quantity: 150.00));
            newJobs.Add(new Job(index: 2, readyTime: 140.00, quantity: 80.00));
            newJobs.Add(new Job(index: 3, readyTime: 180.00, quantity: 40.00));
            newJobs.Add(new Job(index: 4, readyTime: 260.00, quantity: 30.00));
            newJobs.Add(new Job(index: 5, readyTime: 300.00, quantity: 80.00));
            newJobs.Add(new Job(index: 6, readyTime: 440.00, quantity: 120.00));
            newJobs.Add(new Job(index: 7, readyTime: 450.00, quantity: 60.00));
            newJobs.Add(new Job(index: 8, readyTime: 460.00, quantity: 50.00));
            newJobs.Add(new Job(index: 9, readyTime: 560.00, quantity: 50.00));
            newJobs.Add(new Job(index: 10, readyTime: 570.00, quantity: 40.00));
            newJobs.Add(new Job(index: 11, readyTime: 720.00, quantity: 60.00));
            newJobs.Add(new Job(index: 12, readyTime: 840.00, quantity: 30.00));
            newJobs.Add(new Job(index: 13, readyTime: 880.00, quantity: 120.00));
            newJobs.Add(new Job(index: 14, readyTime: 940.00, quantity: 30.00));


            return newJobs;
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

        public void PrintSchedule()
        {
            Console.WriteLine("------- Schedule -------");
            for (int i = 0; i < machines.Count; i++)
            {
                Console.WriteLine("\nMachine {0} Job list: [ {1} ]", machines[i].index, string.Join(",", machines[i].assignedJobs.Select(x => x.index)));
                for (int j = 0; j < machines[i].assignedJobs.Count; j ++)
                {
                    Console.WriteLine("Job {0}: Start Time = {1}, Complete Time = {2} ", 
                        machines[i].assignedJobs[j].index,
                        machines[i].assignedJobs[j].startTime,
                        machines[i].assignedJobs[j].completeTime);
                }
            }
            Console.WriteLine("------------------------");
        }
    }
}
