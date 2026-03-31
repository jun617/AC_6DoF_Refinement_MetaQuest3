using System;
using UnityEngine;

/// <summary>
/// Represents a packet that bundles captured image data and
/// associated metadata for server transmission.
///
/// Note:
/// The original project used a protocol-specific binary layout.
/// The detailed serialization logic is omitted in this public version.
/// </summary>
public class CustomizedPacket
{
    private byte[] packet;
    private bool initialized = false;

    /// <summary>
    /// Initializes a packet using image data and pose-related metadata.
    /// </summary>
    /// <param name="time">Timestamp of the captured frame.</param>
    /// <param name="imageData">Encoded image bytes.</param>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="label">Optional label value for the frame.</param>
    /// <param name="frameID">Frame index.</param>
    /// <param name="hmdPos">HMD position in world space.</param>
    /// <param name="hmdRot">HMD rotation in world space.</param>
    /// <param name="objPos">Object position relative to the camera.</param>
    /// <param name="objRot">Object rotation relative to the camera.</param>
    /// <param name="fx">Camera intrinsic parameter fx.</param>
    /// <param name="fy">Camera intrinsic parameter fy.</param>
    /// <param name="cx">Camera intrinsic parameter cx.</param>
    /// <param name="cy">Camera intrinsic parameter cy.</param>
    public void InitializePacket(
        DateTime time,
        byte[] imageData,
        int width,
        int height,
        int label,
        int frameID,
        Vector3 hmdPos,
        Quaternion hmdRot,
        Vector3 objPos,
        Quaternion objRot,
        float fx,
        float fy,
        float cx,
        float cy)
    {
        // In the original implementation, all metadata fields were serialized
        // into a protocol-specific binary packet.
        // That detailed layout is omitted in the public version.

        // Placeholder behavior for the public version
        packet = imageData;
        initialized = true;
    }

    /// <summary>
    /// Returns the serialized packet data.
    /// </summary>
    public byte[] GetPacket()
    {
        if (!initialized)
        {
            Debug.LogWarning("[CustomizedPacket] Packet has not been initialized.");
        }

        return packet;
    }
}