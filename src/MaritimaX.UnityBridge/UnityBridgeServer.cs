using System;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MaritimaX.Core.Models;

namespace MaritimaX.UnityBridge
{
    public class UnityBridgeServer : IDisposable
    {
        private const string DefaultPipeName = "MaritimaX_Pipe";
        private NamedPipeServerStream? _pipeServer;
        private StreamWriter? _writer;
        private CancellationTokenSource? _cts;
        private Task? _serverTask;

        public bool IsConnected => _pipeServer != null && _pipeServer.IsConnected;

        public event EventHandler<string>? OnLog;

        public void Start(string pipeName = DefaultPipeName)
        {
            if (_serverTask != null) return;

            _cts = new CancellationTokenSource();
            _serverTask = Task.Run(() => ServerLoop(pipeName, _cts.Token));
            Log("Bridge Server Started.");
        }

        public async Task SendCommandAsync(UnityCommand command)
        {
            if (_writer == null || !_pipeServer!.IsConnected)
            {
                Log("Cannot send: Peer not connected.");
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(command);
                await _writer.WriteLineAsync(json);
                // No need to FlushAsync explicitly if AutoFlush is true, but good practice if needed
            }
            catch (Exception ex)
            {
                Log($"Error sending command: {ex.Message}");
            }
        }

        private async Task ServerLoop(string pipeName, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Log("Waiting for Unity connection...");
                    using (_pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                    {
                        await _pipeServer.WaitForConnectionAsync(token);
                        Log("Unity Connected!");

                        using (_writer = new StreamWriter(_pipeServer))
                        {
                            _writer.AutoFlush = true;

                            // Keep the connection alive until broken or cancelled
                            while (_pipeServer.IsConnected && !token.IsCancellationRequested)
                            {
                                await Task.Delay(500, token);
                            }
                        }
                    }
                    Log("Unity Disconnected.");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log($"Bridge Error: {ex.Message}");
                    await Task.Delay(1000, token); // Retry backoff
                }
            }
        }

        private void Log(string message)
        {
            OnLog?.Invoke(this, $"[Bridge] {message}");
            System.Diagnostics.Debug.WriteLine($"[Bridge] {message}");
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _pipeServer?.Dispose(); // This might throw if accessed concurrently, but wrapper handles it
        }
    }
}
