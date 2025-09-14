using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class SmartGridSample
    {
        public DateTime timestamp;
        public double voltage;
        public double current;
        public double powerUsage;
        public int faultIndicator;
        public double frequency;

        public SmartGridSample() : this(new DateTime(), 0, 0, 0, 0, 0) { }

        public SmartGridSample(DateTime timestamp, double voltage, double current, double powerUsage, int faultIndicator, double frequency)
        {
            this.Timestamp = timestamp;
            this.Voltage = voltage;
            this.Current = current;
            this.PowerUsage = powerUsage;
            this.FaultIndicator = faultIndicator;
            this.Frequency = frequency;
        }

        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public double Voltage { get; set; }
        [DataMember]
        public double Current { get; set; }
        [DataMember]
        public double PowerUsage { get; set; }
        [DataMember]
        public int FaultIndicator { get; set; }
        [DataMember]
        public double Frequency { get; set; }

    }
}
