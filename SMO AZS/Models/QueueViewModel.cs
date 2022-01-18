using System.Collections.Generic;

namespace SMO_AZS.Models
{
    public class QueueViewModel
    {
        public QueueViewModel()
        {
            PropertiesValue = new Dictionary<string, double>();
        }

        public Исходные_данные Исходные_Данные { get; set; }
        public Dictionary<string, double> PropertiesValue { get; set; }
    }
}