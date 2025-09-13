using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CsvWriter : IDisposable
    {
        private StreamWriter measurementWriter;
        private StreamWriter rejectsWriter;
        private bool disposed = false;

        public CsvWriter(string path)
        {
            measurementWriter = new StreamWriter(Path.Combine(path, "measurements_session.csv"), append: true);
            rejectsWriter = new StreamWriter(Path.Combine(path, "rejects.csv"), append: true);
        }

        ~CsvWriter()
        {
            Dispose(false);
        }

        public void WriteMeasurement(string line)
        {
            measurementWriter.WriteLine(line);
            measurementWriter.Flush();
        }

        public void WriteRejects(string line)
        {
            rejectsWriter.WriteLine(line);
            rejectsWriter.Flush();
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
                if (disposing)
                {
                    if(measurementWriter != null)
                    {
                        measurementWriter.Flush();
                        measurementWriter?.Dispose();
                    }
                    if(rejectsWriter != null)
                    {
                        rejectsWriter.Flush();
                        rejectsWriter?.Dispose();
                    }
                }
                disposed = true;
            }
        }
    }
}
