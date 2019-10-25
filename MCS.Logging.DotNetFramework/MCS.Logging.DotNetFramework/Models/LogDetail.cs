using System;
using System.Collections.Generic;

namespace MCS.Logging.DotNetFramework.Models
{
    public class LogDetail
    {
        public LogDetail()
        {
            Timestamp = DateTime.Now;
        }
        public DateTime Timestamp { get; private set; }
        public string Message { get; set; }

        // WHERE
        public string Product { get; set; }
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }

        // WHO
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        // Misc.
        public long? ElapsedMilliseconds { get; set; }  // Perf Only
        public Exception Exception { get; set; } //Error Only
        public string CorrelationId { get; set; } // For grouping of logs bubbled up the exec chain
        public Dictionary<string, object> AdditionalInfo { get; set; }  // the kitchen sink
    }
}
