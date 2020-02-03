using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class OGV: ICloneable
    {
        public int Index { get; set; }
        public string Shipper { get; set; }
        public List<Job> Jobs { get; set; }
        public Machine AssignedMachine { get; set; }
        public OGV()
        {
            AssignedMachine = new Machine();
            Jobs = new List<Job>();
        }

        public object Clone()
        {
            return new OGV
            {
                Index = this.Index,
                Shipper = this.Shipper,
                Jobs = this.Jobs,
                AssignedMachine = this.AssignedMachine
            };
        }
    }
}
