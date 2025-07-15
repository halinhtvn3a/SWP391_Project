using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;

namespace BuildingBlocks.Logging.Enrichers
{
    internal class BaggageEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (Activity.Current == null)
                return;

            foreach (var (key, value) in Activity.Current.Baggage)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(key, value));
            }
        }
    }
}
