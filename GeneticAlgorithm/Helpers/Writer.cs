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
        public Writer(string filePath, string header)
        {
            // Initialize csv writer
            this.filePath = filePath;
            File.Delete(filePath);
            csv = new StringBuilder();
            csv.AppendLine(header);
        }

        public void WriteLine(string newLine)
        {
            // Append to csv
            csv.AppendLine(newLine);
        }

        public void SaveFile()
        {
            // Save as csv
            File.AppendAllText(filePath, csv.ToString());
        }

    }
}
