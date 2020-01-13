using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class OGV: ICloneable
    {
        public int index { get; set; }
        public string shipper { get; set; }
        public Machine assignedMachine { get; set; }

        public OGV()
        {
            assignedMachine = new Machine();
        }

        public object Clone()
        {
            return new OGV
            {
                index = this.index,
                shipper = this.shipper,
                assignedMachine = this.assignedMachine
            };
        }
    }
}
