using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MaritimaX.UnityBridge;
using MaritimaX.Core.Models;

namespace MaritimaX.Shell.Controls
{
    public class UnityHwndHost : HwndHost
    {
        private Process? _unityProcess;
        private IntPtr _unityWindowHandle = IntPtr.Zero;
        private readonly string _unityProjectPath;

        // The Bridge Service
        public UnityBridgeServer Bridge { get; private set; }

        // P/Invoke constants
        internal const int GWL_STYLE = -16;
        internal const int WS_VISIBLE = 0x10000000;
        internal const int WS_CHILD = 0x40000000;
        internal const int WM_ACTIVATE = 0x0006;
        internal const int WA_ACTIVE = 1;
        internal const int WA_INACTIVE = 0;

        public UnityHwndHost(string unityProjectPath)
        {
            _unityProjectPath = unityProjectPath;
            Bridge = new UnityBridgeServer();
            Bridge.Start(); // Start listening immediately
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // 1. Launch Unity Instance
            // We pass -parentHWND <Handle> to tell Unity to dock itself here.
            
            StartUnity(hwndParent.Handle);

            return new HandleRef(this, _unityWindowHandle);
        }

        private void StartUnity(IntPtr parentHandle)
        {
             // For this step, to prevent crashing without an EXE, we will create a blank dummy window.
             // But I will include the REAL code commented out for the user.
             
             /*
             var startInfo = new ProcessStartInfo
             {
                 FileName = "Path/To/UnityBuild.exe",
                 Arguments = $"-parentHWND {parentHandle.ToInt64()} -no-window-activation-logic",
                 UseShellExecute = true,
                 CreateNoWindow = true
             };
             _unityProcess = Process.Start(startInfo);
             _unityProcess.WaitForInputIdle();
             */
             
             // Create a dummy window for now to satisfy WPF
             _unityWindowHandle = CreateWindowEx(
                0, "static", "Unity Placeholder",
                WS_CHILD | WS_VISIBLE, 
                0, 0, 800, 600,
                parentHandle,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (_unityProcess != null && !_unityProcess.HasExited)
            {
                _unityProcess.Kill();
                _unityProcess.Dispose();
            }
            Bridge.Dispose();
            DestroyWindow(hwnd.Handle);
        }
        
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
             base.OnRenderSizeChanged(sizeInfo);
             if (_unityWindowHandle != IntPtr.Zero)
             {
                 // Resize Unity window to fit WPF container
                 MoveWindow(_unityWindowHandle, 0, 0, (int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height, true);
             }
        }

        public async void SendTestCommand(string msg)
        {
            var cmd = new UnityCommand 
            { 
                CommandType = "TEST_MSG", 
                Payload = msg 
            };
            await Bridge.SendCommandAsync(cmd);
        }

        // P/Invoke
        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
        private static extern IntPtr CreateWindowEx(
            int exStyle, string className, string windowName, int style,
            int x, int y, int width, int height,
            IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    }
}
