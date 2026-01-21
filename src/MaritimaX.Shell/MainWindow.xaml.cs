using System.Windows;
using MaritimaX.Shell.Controls;

namespace MaritimaX.Shell
{
    public partial class MainWindow : Window
    {
        private UnityHwndHost _unityHost;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the Bridge
            // In a real scenario, we pass the build path.
            _unityHost = new UnityHwndHost(@"C:\Your\Unity\Build\Path");
            UnityContainer.Child = _unityHost;
            
            // Start the IPC Server
            _unityHost.InitializePipeServer();
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