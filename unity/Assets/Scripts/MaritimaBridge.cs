using UnityEngine;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

public class MaritimaBridge : MonoBehaviour
{
    [Header("Configuration")]
    public string pipeName = "MaritimaX_Pipe";
    public bool isEmbedded = true;

    private NamedPipeClientStream _client;
    private Thread _readThread;
    private ConcurrentQueue<string> _commandQueue = new ConcurrentQueue<string>();
    private bool _isRunning = false;

    [Header("Debug")]
    public string lastMessage = "Waiting for Shell...";

    void Start()
    {
        if (isEmbedded)
        {
            // Connect to the WPF Shell
            _isRunning = true;
            _readThread = new Thread(ConnectAndRead);
            _readThread.IsBackground = true;
            _readThread.Start();
        }
    }

    private void ConnectAndRead()
    {
        while (_isRunning)
        {
            try
            {
                if (_client == null || !_client.IsConnected)
                {
                    _client = new NamedPipeClientStream(".", pipeName, PipeDirection.In);
                    _client.Connect(1000); // Try connect for 1 sec
                    // If we get here, we connected!
                }

                using (var reader = new StreamReader(_client))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null && _isRunning)
                    {
                        _commandQueue.Enqueue(line);
                    }
                }
            }
            catch (Exception)
            {
                // Connection lost or failed, retry in 1s
                Thread.Sleep(1000);
            }
        }
    }

    void Update()
    {
        // Process Main Thread actions from the Queue
        while (_commandQueue.TryDequeue(out string command))
        {
            ProcessCommand(command);
        }
    }

    private void ProcessCommand(string command)
    {
        lastMessage = command;
        Debug.Log($"[Shell CMD]: {command}");

        // BASIC TEST IMPLEMENTATION
        if (command == "RESET_SIMULATION")
        {
            transform.rotation = Quaternion.identity;
        }
        else if (command == "EMERGENCY_STOP")
        {
            // Spin wildly or change color to Red
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void OnApplicationQuit()
    {
        _isRunning = false;
        if (_client != null)
        {
            _client.Dispose();
        }
    }
}
