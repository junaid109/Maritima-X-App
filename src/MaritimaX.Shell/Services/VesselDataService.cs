using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MaritimaX.Core.Models;

namespace MaritimaX.Shell.Services
{
    public class VesselDataService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5200";

        public VesselDataService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<ShipTelemetry>> GetFleetTelemetryAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ShipTelemetry>>("/telemetry") ?? new List<ShipTelemetry>();
            }
            catch (Exception ex)
            {
                // In production, log this.
                System.Diagnostics.Debug.WriteLine($"Error fetching telemetry: {ex.Message}");
                return new List<ShipTelemetry>();
            }
        }
    }
}
