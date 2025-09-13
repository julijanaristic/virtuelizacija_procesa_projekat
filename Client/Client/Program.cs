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

            using(var reader = new CsvReader("smart_grid_dataset.csv"))
            {
                var (samples, rejects) = reader.ReadSamples();

                if(samples.Count > 0)
                {
                    try
                    {
                        proxy.StartSession(samples[0]);
                    } 
                    catch(FaultException<ValidationFault> validationFault)
                    {
                        Console.WriteLine($"Validation fault on session start: {validationFault.Detail.Message}");
                        return;
                    }
                    catch (FaultException<DataFormatFault> dataFormatFault)
                    {
                        Console.WriteLine($"Data format fault on session start: {dataFormatFault.Detail.Message}");
                        return;
                    }
                    catch (CommunicationException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    for (int i=0; i<samples.Count; i++)
                    {
                        try
                        {
                            var response = proxy.PushSample(samples[i]);
                            Console.WriteLine($"[{i}] Server response: {response}");
                        } 
                        catch (FaultException<ValidationFault> validationFault)
                        {
                            Console.WriteLine($"Validation fault on session start: {validationFault.Detail.Message}");
                        }
                        catch (FaultException<DataFormatFault> dataFormatFault)
                        {
                            Console.WriteLine($"Data format fault on session start: {dataFormatFault.Detail.Message}");
                        }
                        System.Threading.Thread.Sleep(500);
                    }
                    try
                    {
                        proxy.EndSession();
                    }
                    catch (FaultException<ValidationFault> validationFault)
                    {
                        Console.WriteLine($"Validation fault on session start: {validationFault.Detail.Message}");
                    }
                    catch (FaultException<DataFormatFault> dataFormatFault)
                    {
                        Console.WriteLine($"Data format fault on session start: {dataFormatFault.Detail.Message}");
                    }
                }

                if(rejects.Count > 0)
                {
                    Console.WriteLine("Rejected lines from CSV:");
                    foreach(var reject in rejects)
                    {
                        //Console.WriteLine(reject);
                    }
                }

            }

            Console.WriteLine("Client finished sending data.");
            Console.ReadKey();
        }
    }
}
