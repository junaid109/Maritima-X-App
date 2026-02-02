using System;

namespace MaritimaX.Core.Models
{
    public class ShipTelemetry
    {
        public string ShipId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heading { get; set; }
        public double SpeedKnots { get; set; }
        public double EngineRpm { get; set; }
        public double EngineTempC { get; set; }
        public double FuelLevelPercent { get; set; }
        public bool IsEmergency { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
