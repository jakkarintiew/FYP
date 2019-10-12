using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Writer
    {
        private string filePath { get;  set; }
        private StringBuilder csv { get; set; }
        public Writer(string filePath)
        {
            // Initialize csv writer
            this.filePath = filePath;
            File.Delete(filePath);
            csv = new StringBuilder();
            var newLine = string.Format("Generation,Fitness");
            csv.AppendLine(newLine);
        }

        public void WriteLine(GA ga)
        {
            // Append to csv
            string newLine = string.Format("{0}, {1}", ga.Generation, ga.BestFitness);
            csv.AppendLine(newLine);
        }

        public void SaveFile()
        {
            // Save as csv
            File.AppendAllText(filePath, csv.ToString());
        }

    }
}
