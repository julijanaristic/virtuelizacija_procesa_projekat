using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface ISmartGridService
    {
        [OperationContract]
        [FaultContract(typeof(CustomException))]
        bool StartSession(SmartGridSample meta);

        [OperationContract]
        [FaultContract(typeof(CustomException))]
        bool PushSample(SmartGridSample sample);

        [OperationContract]
        [FaultContract(typeof(CustomException))]
        bool EndSession();
    }
}
