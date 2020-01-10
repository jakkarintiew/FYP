using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Job: ICloneable
    {
        public int index { get; set; }
        public double position { get; set; }
        public double readyTime { get; set; }
        public double requestedProcRate { get; set; }
        public double requestedProcTime { get; set; }
        public double quantity { get; set; }
        public bool isGeared { get; set; }
        public bool isDedicated { get; set; }
        public bool isOutOfLaycan { get; set; }
        public bool isUnloading { get; set; }
        public int machineIdUnload { get; set; }
        public bool isBarge { get; set; }
        public int machineIdBarge { get; set; }
        public string shipper { get; set; }
        public double demurrage { get; set; }
        public double despatch { get; set; }
        public double travelingCost { get; set; }
        public double totalCost { get; set; }
        public Machine assignedMachine { get; set; }
        public double startTime { get; set; }
        public double lateStartTime { get; set; }
        public double completeTime { get; set; }
        public int priority { get; set; }
        public double dndTime { get; set; }


        // Construtor
        public Job()
        {
            this.index = -1;
            this.position = -1;
            this.readyTime = -1;
            this.requestedProcRate = -1;
            this.quantity = -1;
            this.isGeared = false;
            this.isDedicated = false;
            this.shipper = "-1";
            this.isOutOfLaycan = false;
            this.isUnloading = false;
            this.machineIdUnload = -1;
            this.isBarge = false;
            this.machineIdBarge = -1;
            this.demurrage = -1;
            this.despatch = -1;
            this.priority = -1;
        }

        public bool isLateComplete()
        {
           return dndTime > 0;
        }

        public object Clone()
        {          
            return new Job
            {
                index = this.index,
                position = this.position,
                readyTime = this.readyTime,
                requestedProcRate = this.requestedProcRate,
                requestedProcTime = this.requestedProcTime,
                quantity = this.quantity,
                isGeared = this.isGeared,
                isDedicated = this.isDedicated,
                isOutOfLaycan = this.isOutOfLaycan,
                isUnloading = this.isUnloading,
                machineIdUnload = this.machineIdUnload,
                isBarge = this.isBarge,
                machineIdBarge = this.machineIdBarge,
                shipper = this.shipper,
                demurrage = this.demurrage,
                despatch = this.despatch,
                travelingCost = this.travelingCost,
                totalCost = this.totalCost,
                assignedMachine = this.assignedMachine,
                startTime = this.startTime,
                lateStartTime = this.lateStartTime,
                completeTime = this.completeTime,
                priority = this.priority,
                dndTime = this.dndTime
            }; 
        }
    }
}
