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
        public bool faultIndicator;
        public double frequency;

        public SmartGridSample() : this(new DateTime(), 0, 0, 0, false, 0) { }

        public SmartGridSample(DateTime timestamp, double voltage, double current, double powerUsage, bool faultIndicator, double frequency)
        {
            this.timestamp = timestamp;
            this.voltage = voltage;
            this.current = current;
            this.powerUsage = powerUsage;
            this.faultIndicator = faultIndicator;
            this.frequency = frequency;
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
        public bool FaultIndicator { get; set; }
        [DataMember]
        public double Frequency { get; set; }

    }
}
