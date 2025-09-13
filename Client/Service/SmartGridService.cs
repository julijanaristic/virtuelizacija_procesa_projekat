using Common;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SmartGridService : ISmartGridService
    {
        private bool sessionActive = false;
        private CsvWriter fileWriter;
        public string StartSession(SmartGridSample meta)
        {
            if(meta == null)
            {
                throw new FaultException<DataFormatFault>(new DataFormatFault("Meta sample is null"));
            }

            ValidateSample(meta);

            //NAPOMENA: promeniti ime foldera gde se snimaju podaci
            fileWriter = new CsvWriter("session1");

            sessionActive = true;
            Console.WriteLine($"[INFO] Session started at {meta.Timestamp}");
            return "ACK: Session started";
        }

        public string PushSample(SmartGridSample sample)
        {
            if (!sessionActive)
                return "NACK: No active session!";

            if(sample == null)
            {
                throw new FaultException<DataFormatFault>(new DataFormatFault("Sample is null"));
            }

            string line = $"{sample.Timestamp},{sample.Voltage},{sample.Current},{sample.PowerUsage}, {sample.FaultIndicator},{sample.Frequency}";
            bool valid = TryValidateSample(sample, out string error);

            if(valid)
            {
                fileWriter.WriteMeasurement(line);
                return "ACK: Sample received";
            }
            else
            {
                fileWriter.WriteRejects(line);
                Console.WriteLine($"[WARN] Rejected sample: {error}");
                return $"NACK: {error}";
            }

        }

        public string EndSession()
        {
            if (!sessionActive) 
                return "NACK: No active session!";

            sessionActive = false;

            fileWriter.Dispose();
            fileWriter = null;

            Console.WriteLine("[INFO] Session ended");
            return "ACK: Session completed";
        }

        private void ValidateSample(SmartGridSample sample)
        {
            if (sample.Timestamp == default)
                throw new FaultException<ValidationFault>(new ValidationFault("Timestamp is required"));

            if (sample.Frequency <= 0)
                throw new FaultException<ValidationFault>(new ValidationFault("Frequency must be greater than 0!"));

            if (sample.Voltage < 0 || sample.Voltage > 1000)
                throw new FaultException<ValidationFault>(new ValidationFault($"Voltage {sample.Voltage} out of range (0-1000)"));

            if (sample.Current < -1000 || sample.Current > 1000)
                throw new FaultException<ValidationFault>(new ValidationFault($"Current {sample.Current} out of range (-1000,1000)"));

            if (sample.PowerUsage < 0)
                throw new FaultException<ValidationFault>(new ValidationFault("Power usage must be >= 0"));
        }

        private bool TryValidateSample(SmartGridSample sample, out string error)
        {
            if(sample.Frequency <= 0)
            {
                error = "Frequency <= 0";
                return false;
            }

            if(sample.Voltage < 0 || sample.Voltage > 1000)
            {
                error = $"Voltage {sample.Voltage} out of range";
                return false;
            }

            if(sample.Current < -1000 || sample.Current > 1000)
            {
                error = $"Current {sample.Current} out of range";
                return false;
            }

            if(sample.PowerUsage < 0)
            {
                error = "PowerUsage < 0";
                return false;
            }

            error = null;
            return true;
        }
    }
}
