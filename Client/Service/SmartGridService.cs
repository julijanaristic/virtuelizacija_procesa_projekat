using Common;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SmartGridService : ISmartGridService
    {
        //delegati
        public delegate void TransferStartedHandler();
        public delegate void SampleReceivedHandler(SmartGridSample sample);
        public delegate void TransferCompletedHandler();
        public delegate void WarningRaisedHandler(string message, SmartGridSample sample);
        
        //dogadjaji
        public event TransferStartedHandler OnTransferStarted;
        public event SampleReceivedHandler OnSampleReceived;
        public event TransferCompletedHandler OnTransferCompleted;
        public event WarningRaisedHandler OnWarningRaised;

        private double V_threshold;
        private double I_threshold;
        private double CurrentDeviationPercentage;

        private List<double> currentSample = new List<double>();
        private double previousVoltage = 0;

        private bool sessionActive = false;
        private CsvWriter fileWriter;
        public string StartSession(SmartGridSample meta)
        {
            if(meta == null)
            {
                throw new FaultException<DataFormatFault>(new DataFormatFault("Meta sample is null"));
            }

            V_threshold = double.Parse(ConfigurationManager.AppSettings["V_threshold"]);
            I_threshold = double.Parse(ConfigurationManager.AppSettings["I_threshold"]);
            CurrentDeviationPercentage = double.Parse(ConfigurationManager.AppSettings["CurrentDeviationPercentage"]);

            ValidateSample(meta);

            //NAPOMENA: promeniti ime foldera gde se snimaju podaci
            Console.WriteLine($"Trying to create session folder: {Path.GetFullPath("session1")}");
            fileWriter = new CsvWriter("session1");

            sessionActive = true;

            if (OnTransferCompleted != null)
                OnTransferCompleted();

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

            Console.WriteLine("[INFO] transmission in progress");

            string line = $"{sample.Timestamp},{sample.Voltage},{sample.Current},{sample.PowerUsage}, {sample.FaultIndicator},{sample.Frequency}";
            bool valid = TryValidateSample(sample, out string error);

            if(valid)
            {
                fileWriter.WriteMeasurement(line);

                if (OnSampleReceived != null)
                    OnSampleReceived(sample);

                if(Math.Abs(sample.Voltage - previousVoltage) > V_threshold)
                {
                    if (OnWarningRaised != null)
                        OnWarningRaised("Voltage spike detected", sample);
                }

                currentSample.Add(sample.Current);
                double Imean = currentSample.Average();
                double lowerLimit = Imean * (1 - CurrentDeviationPercentage / 100.0);
                double upperLimit = Imean * (1 + CurrentDeviationPercentage / 100.0);

                if(sample.Current < lowerLimit || sample.Current > upperLimit)
                {
                    if (OnWarningRaised != null)
                        OnWarningRaised("Current out of band", sample);
                }

                previousVoltage = sample.Voltage;
                return "ACK: Sample received";
            }
            else
            {
                fileWriter.WriteRejects(line);
                if (OnWarningRaised != null)
                    OnWarningRaised($"Rejected sample {error}", sample);
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

            if(OnTransferCompleted != null) 
                OnTransferCompleted();

            Console.WriteLine("[INFO] Session ended");
            return "ACK: Session completed";
        }

        public string PushReject(string rawLine)
        {
            if (!sessionActive || fileWriter == null)
                return "NACK: No active session!";

            Console.WriteLine($"[INFO] Reject received from client: {rawLine}");
            fileWriter.WriteRejects(rawLine);
            return "ACK: Reject stored";
        }

        private void ValidateSample(SmartGridSample sample)
        {
            if (sample.Timestamp.GetDateTimeFormats().Equals("yyyy-MM-dd") )
                throw new FaultException<ValidationFault>(new ValidationFault("Timestamp is required"), new FaultReason("Validation error"));

            if (sample.Frequency < 0)
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
