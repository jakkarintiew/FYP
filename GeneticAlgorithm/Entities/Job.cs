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
        public int Index { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double ReadyTime { get; set; }
        public double RequestedProcRate { get; set; }
        public double RequestedProcTime { get; set; }
        public double Quantity { get; set; }
        public bool IsGeared { get; set; }
        public bool IsDedicated { get; set; }
        public bool IsOutOfLaycan { get; set; }
        public bool IsUnloading { get; set; }
        public int MachineIdUnload { get; set; }
        public bool IsBarge { get; set; }
        public int MachineIdBarge { get; set; }
        public string Shipper { get; set; }
        public int OgvId { get; set; }
        public OGV Ogv { get; set; }
        public double Demurrage { get; set; }
        public double Despatch { get; set; }
        public double TotalCost { get; set; }
        public Machine AssignedMachine { get; set; }
        public double StartTime { get; set; }
        public double LateStartTime { get; set; }
        public double CompleteTime { get; set; }
        public int Priority { get; set; }
        public double DndTime { get; set; }
        public double ProcTime { get; set; }
        public double TravelCost { get; set; }
        public double HandlingCost { get; set; }
        public double DndCost { get; set; }
        public double RentalCost { get; set; }


        // Construtor
        public Job()
        {
            Index = -1;
            AssignedMachine = null;
        }
        public void Init(List<OGV> ogvs)
        {
            RequestedProcTime = RequestedProcRate * Quantity;
            foreach (OGV _ogv in ogvs)
            {
                if (_ogv.Index == OgvId)
                {
                    Ogv = _ogv; 
                }
            }
        }
        public bool IsLateComplete()
        {
           return DndTime > 0;
        }
        public object Clone()
        {          
            return new Job
            {
                Index = this.Index,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                ReadyTime = this.ReadyTime,
                RequestedProcRate = this.RequestedProcRate,
                RequestedProcTime = this.RequestedProcTime,
                Quantity = this.Quantity,
                IsGeared = this.IsGeared,
                IsDedicated = this.IsDedicated,
                IsOutOfLaycan = this.IsOutOfLaycan,
                IsUnloading = this.IsUnloading,
                MachineIdUnload = this.MachineIdUnload,
                IsBarge = this.IsBarge,
                MachineIdBarge = this.MachineIdBarge,
                Shipper = this.Shipper,
                OgvId = this.OgvId,
                Ogv = this.Ogv,
                TravelCost = this.TravelCost,
                HandlingCost = this.HandlingCost,
                Demurrage = this.Demurrage,
                Despatch = this.Despatch,
                RentalCost = this.RentalCost,
                TotalCost = this.TotalCost,
                AssignedMachine = this.AssignedMachine,
                StartTime = this.StartTime,
                LateStartTime = this.LateStartTime,
                CompleteTime = this.CompleteTime,
                Priority = this.Priority,
                DndTime = this.DndTime
            }; 
        }
    }
}
