using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles TCP-based communication between the Unity client
/// and the external processing server.
/// 
/// This class is responsible for:
/// 1) establishing a server connection,
/// 2) sending binary packet data,
/// 3) receiving server responses asynchronously.
///
/// Note:
/// The original project used a protocol-specific binary payload format.
/// In this public version, the detailed parsing logic is intentionally omitted.
/// </summary>
public class Client : Singleton<Client>
{
    /// <summary>
    /// TCP client used to connect to the remote server.
    /// </summary>
    private TcpClient tcp_client_;

    /// <summary>
    /// Network stream used for reading and writing binary data.
    /// </summary>
    private NetworkStream network_stream_;

    /// <summary>
    /// Optional text writer bound to the current network stream.
    /// </summary>
    private StreamWriter writer;

    /// <summary>
    /// Optional text reader bound to the current network stream.
    /// </summary>
    private StreamReader reader;

    /// <summary>
    /// Indicates whether the client is currently connected to the server.
    /// </summary>
    private bool is_connected_ = false;

    /// <summary>
    /// Target server IP address.
    /// </summary>
    private string ip_;

    /// <summary>
    /// Target server port number.
    /// </summary>
    private int port_;

    /// <summary>
    /// Controls whether the asynchronous receive loop should continue running.
    /// </summary>
    private bool recvLoopActive = false;

    /// <summary>
    /// Sets the target server IP address.
    /// </summary>
    /// <param name="ip">Server IP address.</param>
    public void SetIP(string ip)
    {
        ip_ = ip;
    }

    /// <summary>
    /// Sets the target server port number.
    /// </summary>
    /// <param name="portNumber">Server port number.</param>
    public void SetPort(int port)
    {
        port_ = port;
    }

    /// <summary>
    /// Updates the current connection state.
    /// </summary>
    /// <param name="isConnected">True if connected; otherwise false.</param>
    public void SetIsConnected(bool isConnected)
    {
        is_connected_ = isConnected;
    }

    /// <summary>
    /// Returns whether the client is currently connected to the server.
    /// </summary>
    public bool IsConnected()
    {
        if (is_connected_) return true;
        else return false;
    }

    /// <summary>
    /// Attempts to establish a TCP connection to the configured server.
    /// </summary>
    /// <returns>
    /// True if the connection succeeds; otherwise false.
    /// </returns>
    public async Task<bool> ConnectToServer()
    {
        try
        {
            tcp_client_ = new TcpClient();

            var result = tcp_client_.BeginConnect(ip_, port_, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            if (!success)
            {
                throw new Exception("Connection TimeOut");
            }

            /// EndConnect must be called after BeginConnect.
            tcp_client_.EndConnect(result);

            network_stream_ = tcp_client_.GetStream();
            writer = new StreamWriter(network_stream_) { AutoFlush = true };
            reader = new StreamReader(network_stream_);

            is_connected_ = true;

            StartReceiveLoop();

            return true;
        }
        catch (Exception ex)
        {
            DebugLogger.Instance.WriteLog($"[Client] Exception: {ex}");
            return false;
        }
    }

    /// <summary>
    /// Sends a binary packet to the server.
    /// </summary>
    /// <param name="data">Serialized binary data to send.</param>
    /// <returns>
    /// True if the packet is sent successfully; otherwise false.
    /// </returns>
    public bool SendPacket(byte[] data)
    {
        if (!is_connected_)
        {
            DebugLogger.Instance.WriteLog($"[Client] SendPacket failed. Not Connected to Server.");
            return false;
        }

        try
        {
            network_stream_.Write(data, 0, data.Length);
            network_stream_.Flush();
            //DebugLogger.Instance.WriteLog("[Client] Data Send Success.");
            return true;
        }
        catch (Exception ex)
        {
            DebugLogger.Instance.WriteLog($"[Client] Data Send Fail: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Closes the current socket connection and releases related resources.
    /// </summary>
    public void CloseSocket()
    {
        is_connected_ = false;

        if (writer != null)
        {
            writer.Close();
            writer = null;
        }
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
        if (network_stream_ != null)
        {
            network_stream_.Close();
            network_stream_ = null;
        }
        if (tcp_client_ != null)
        {
            tcp_client_.Close();
            tcp_client_ = null;
        }
    }

    /// <summary>
    /// Starts an asynchronous receive loop for incoming server messages.
    /// 
    /// In the original project, this loop parsed a protocol-specific
    /// binary payload and forwarded pose-related data to the tracking system.
    /// In this public version, the detailed packet parsing is intentionally omitted.
    /// </summary>
    private async void StartReceiveLoop()
    {
        // DebugLogger.Instance.WriteLog($"[Client] Receive Loop Started.");
        recvLoopActive = true;

        try
        {
            while (recvLoopActive && is_connected_)
            {
            byte[] message = await ReceiveServerMessageAsync();

            if (message == null || message.Length == 0)
            {
                throw new Exception("Connection closed by server.");
            }

            ServerTrackingResult result = ParseTrackingResult(message);

            TrackingManager.Instance.OnReceivedServerData(
                result.refinedPos,
                result.refinedRot,
                result.hmdPos,
                result.hmdRot,
                result.lost
            );

            }
        }
        catch (Exception ex)
        {
            DebugLogger.Instance.WriteLog($"[Client] Receive Loop Exception: {ex}");
            CloseSocket();
        }
    }

    /// <summary>
    /// Receives a message from the network stream.
    /// 
    /// This is a simplified placeholder for the original
    /// protocol-specific receive logic.
    /// </summary>
    /// <returns>
    /// A byte array containing the received message,
    /// or null if the connection has been closed.
    /// </returns>
    private async Task<byte[]> ReceiveServerMessageAsync()
    {
        if (network_stream_ == null)
        {
            return null;
        }

        byte[] buffer = new byte[256];
        int bytesRead = await network_stream_.ReadAsync(buffer, 0, buffer.Length);

        if (bytesRead == 0)
        {
            return null;
        }

        byte[] result = new byte[bytesRead];
        Array.Copy(buffer, result, bytesRead);
        return result;
    }


    /// <summary>
    /// Parses tracking-related response data from the server.
    /// The detailed protocol-specific decoding logic is omitted
    /// in the public version.
    /// </summary>
    private ServerTrackingResult ParseTrackingResult(byte[] message)
    {
        /// Placeholder example.
        /// In the original project, this method decoded a protocol-specific
        /// binary payload into pose and tracking state values.

        return new ServerTrackingResult
        {
            refinedPos = Vector3.zero,
            refinedRot = Quaternion.identity,
            hmdPos = Vector3.zero,
            hmdRot = Quaternion.identity,
            lost = 0f
        };
    }

    private struct ServerTrackingResult
    {
        public Vector3 refinedPos;
        public Quaternion refinedRot;
        public Vector3 hmdPos;
        public Quaternion hmdRot;
        public float lost;
    }
}

