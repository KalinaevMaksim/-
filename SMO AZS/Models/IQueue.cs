using System.Collections.Generic;

namespace SMO_AZS.Models
{
    public interface IQueue
    {
        void CalcProperties();
        QueueViewModel GetQueueViewModel();
    }
}