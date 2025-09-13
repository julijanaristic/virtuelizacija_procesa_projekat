using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SmartGridService smartGridService = new SmartGridService();

            //pretplata za dogadjaje
            smartGridService.OnTransferStarted += () => Console.WriteLine("Session started.");
            smartGridService.OnSampleReceived += (sample) => Console.WriteLine($"Sample primljen: {sample.Timestamp}");
            smartGridService.OnWarningRaised += (message, sample) => Console.WriteLine($"WARNING: {message}, Current={sample.Current}, Voltage={sample.Voltage}");
            smartGridService.OnTransferCompleted += () => Console.WriteLine("Session completed.");

            ServiceHost host = new ServiceHost(typeof(SmartGridService));
            host.Open();

            Console.WriteLine("Service is open, press any key to close it.");
            Console.ReadKey();

            host.Close();
            Console.WriteLine("Service is closed");
        }
    }
}
