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

        public (List<string[]> samples, List<string> rejects) ReadSamples(int maxRows = 100)
        {
            var results = new List<string[]>();
            var rejects = new List<string>();

            int rowCount = 0;
            string line;

            while((line = textReader.ReadLine()) != null)
            {
                string[] columns = line.Split(',');

                if(columns.Length != 6)
                {
                    rejects.Add(line);
                    continue;
                }

                if (!double.TryParse(columns[1], NumberStyles.Float, CultureInfo.InvariantCulture, out _) ||
                    !double.TryParse(columns[2], NumberStyles.Float, CultureInfo.InvariantCulture, out _) ||
                    !double.TryParse(columns[3], NumberStyles.Float, CultureInfo.InvariantCulture, out _) ||
                    !double.TryParse(columns[5], NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                {
                    rejects.Add(line);
                    continue;
                }

                if(rowCount < maxRows)
                {
                    results.Add(columns);
                    rowCount++;
                } else
                {
                    rejects.Add(line);
                }
            }
            return (results, rejects);
        }

    }
}
