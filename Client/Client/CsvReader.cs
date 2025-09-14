using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

            textReader.ReadLine(); //preskoci header

            while ((line = textReader.ReadLine()) != null)
            {
                string[] columns = line.Split(',');

                if (columns.Length != 134)
                {
                    rejects.Add(line);
                    continue;
                }



                try
                {
                    DateTime timestamp;
                    bool timestampOk = DateTime.TryParseExact(
                        columns[0].Trim(),
                        "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out timestamp
                    );

                    if (!timestampOk)
                    {
                        rejects.Add(line);
                        continue;
                    }
                    //DateTime timestamp = DateTime.ParseExact(columns[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    double voltage = double.Parse(columns[1].Trim(), CultureInfo.InvariantCulture);
                    double current = double.Parse(columns[2].Trim(), CultureInfo.InvariantCulture);
                    double powerUsage = double.Parse(columns[3].Trim(), CultureInfo.InvariantCulture);
                    double frequency = double.Parse(columns[4].Trim(), CultureInfo.InvariantCulture);
                    int faultIndicator = int.Parse(columns[5].Trim(), CultureInfo.InvariantCulture); // int po tvom zahtevu


                    if (rowCount < maxRows)
                    {
                        //results.Add(new SmartGridSample(timestamp, voltage, current, powerUsage, faultIndicator, frequency));

                        results.Add(new SmartGridSample(
                            timestamp,
                            voltage,
                            current,
                            powerUsage,
                            faultIndicator,
                            frequency
                        ));
                        rowCount++;
                        Console.WriteLine(results[rowCount-1].Frequency + " " + results[rowCount-1].PowerUsage);
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
