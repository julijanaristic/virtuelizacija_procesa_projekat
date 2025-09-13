using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class CsvReader : IDisposable
    {
        private StreamReader textReader;
        private bool disposed = false;

        public CsvReader(string path)
        { 
            textReader = new StreamReader(path);
        }

        ~CsvReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if(textReader != null)
                {
                    textReader?.Dispose();
                }
                disposed = true;
            }
        }

        public (List<SmartGridSample> samples, List<string> rejects) ReadSamples(int maxRows = 100)
        {
            var results = new List<SmartGridSample>();
            var rejects = new List<string>();

            int rowCount = 0;
            string line;

            textReader.ReadLine();

            while ((line = textReader.ReadLine()) != null)
            {
                string[] columns = line.Split(',');

                if (columns.Length < 6)
                {
                    rejects.Add(line);
                    continue;
                }

                try
                {
                    DateTime timestamp = DateTime.Parse(columns[0], CultureInfo.InvariantCulture);
                    double voltage = double.Parse(columns[1], CultureInfo.InvariantCulture);
                    double current = double.Parse(columns[2], CultureInfo.InvariantCulture);
                    double powerUsage = double.Parse(columns[3], CultureInfo.InvariantCulture);
                    double frequency = double.Parse(columns[4], CultureInfo.InvariantCulture);
                    bool faultIndicator = int.Parse(columns[5], CultureInfo.InvariantCulture) != 0;

                    if(rowCount < maxRows)
                    {
                        results.Add(new SmartGridSample(timestamp, voltage, current, powerUsage, faultIndicator, frequency));
                        rowCount++;
                    }
                    else
                    {
                        rejects.Add(line);
                    }
                }
                catch
                {
                    rejects.Add(line);
                }

            }
            return (results, rejects);
        }

    }
}
