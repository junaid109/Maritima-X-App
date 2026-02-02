using System.Windows;
using System.Windows.Threading;
using MaritimaX.Shell.Controls;
using MaritimaX.Shell.Services;

namespace MaritimaX.Shell
{
    public partial class MainWindow : Window
    {
        private UnityHwndHost _unityHost;
        private readonly VesselDataService _dataService;
        private readonly DispatcherTimer _telemetryTimer;

        public MainWindow()
        {
            InitializeComponent();
            _dataService = new VesselDataService();
            
            // Poll every 1s
            _telemetryTimer = new DispatcherTimer();
            _telemetryTimer.Interval = System.TimeSpan.FromSeconds(1);
            _telemetryTimer.Tick += TelemetryTimer_Tick;
            
            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        private async void TelemetryTimer_Tick(object? sender, System.EventArgs e)
        {
            var data = await _dataService.GetFleetTelemetryAsync();
            TelemetryGrid.ItemsSource = data;
            
            // "Elite" Feature: If any ship is in Emergency, trigger Unity
            foreach(var ship in data)
            {
                if (ship.IsEmergency)
                {
                    _unityHost.SendCommand($"ALERT_SHIP:{ship.ShipId}");
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the Bridge
            // In a real scenario, we pass the build path.
            _unityHost = new UnityHwndHost(@"C:\Your\Unity\Build\Path");
            UnityContainer.Child = _unityHost;
            
            // Start the IPC Server
            _unityHost.InitializePipeServer();
            
            // Start Polling
            _telemetryTimer.Start();
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _telemetryTimer.Stop();
        }

        private void Emergency_Click(object sender, RoutedEventArgs e)
        {
            _unityHost.SendCommand("EMERGENCY_STOP");
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _unityHost.SendCommand("RESET_SIMULATION");
        }
    }
}