using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<ISmartGridService> factory = new ChannelFactory<ISmartGridService>("SmartGridServiceEndpoint");

            ISmartGridService proxy = factory.CreateChannel();

            using(var reader = new CsvReader("ulaz.csv"))
            {
                var (samples, rejects) = reader.ReadSamples();

                if(samples.Count > 0)
                {
                    proxy.StartSession(samples[0]);
                    for(int i=0; i<samples.Count; i++)
                    {
                        var response = proxy.PushSample(samples[i]);
                        Console.WriteLine($"[{i}] Server response: {response}");
                        System.Threading.Thread.Sleep(500);
                    }
                    proxy.EndSession();
                }

            }

            Console.WriteLine("Client finished sending data.");
            Console.ReadKey();
        }
    }
}
