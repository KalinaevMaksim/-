using System;
using System.Collections.Generic;

namespace SMO_AZS.Utils
{
    public class QueueListComparer : IEqualityComparer<KeyValuePair<string, double>>
    {
        public static QueueListComparer queueListComparer { get; set; } = new QueueListComparer();

        public bool Equals(KeyValuePair<string, double> x, KeyValuePair<string, double> y)
        {
            return string.Compare(x.Value.ToString(), y.Value.ToString(), StringComparison.CurrentCulture) == 0;
        }

        public int GetHashCode(KeyValuePair<string, double> obj)
        {
            return obj.GetHashCode();
        }
    }
}