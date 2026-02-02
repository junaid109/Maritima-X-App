using System;

namespace MaritimaX.Core.Models
{
    /// <summary>
    /// Represents a command sent from the WPF Shell to the Unity Instance.
    /// </summary>
    public class UnityCommand
    {
        /// <summary>
        /// The type of operation (e.g., "TELEMETRY", "CAMERA_FOCUS", "RESET").
        /// </summary>
        public string CommandType { get; set; } = string.Empty;

        /// <summary>
        /// JSON payload or raw parameter string.
        /// </summary>
        public string Payload { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
