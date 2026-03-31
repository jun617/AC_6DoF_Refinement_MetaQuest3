using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages network setup and data transmission for the application.
/// This class handles:
/// 1) UI-based server connection setup
/// 2) Packaging captured image frames with camera/object pose metadata
///    for server-side processing
/// </summary>
public class NetworkManager : Singleton<NetworkManager>
{
    [Header("Network UI")]

    /// <summary>
    /// Dropdown used to select the target server IP address.
    /// </summary>
    [SerializeField] public TMP_Dropdown ip_address_;

    /// <summary>
    /// Dropdown used to select the target server port number.
    /// </summary>
    [SerializeField] public TMP_Dropdown port_number_;

    /// <summary>
    /// Button that attempts to connect to the selected server.
    /// </summary>
    [SerializeField] public UnityEngine.UI.Button connectButton;

    /// <summary>
    /// UI text used to display the current network connection status.
    /// </summary>
    [SerializeField] private TMP_Text statusText;

    /// <summary>
    /// Button used to proceed to the next application state
    /// after a successful network connection.
    /// </summary>
    [SerializeField] public UnityEngine.UI.Button nextButton;

    [Header("Scene References")]
    /// <summary>
    /// Reference world-space air-conditioner model used to compute
    /// the relative pose between the object and the locatable camera.
    /// </summary>
    [SerializeField] private GameObject worldACModel;

    /// <summary>
    /// Registers UI button callbacks when the scene starts.
    /// </summary>
    void Start()
    {
        if (connectButton != null)
        {
            connectButton.onClick.AddListener(() => OnConnectButtonClicked());
        }
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() => OnNextButtonClicked());
        }

    }

    /// <summary>
    /// Attempts to connect to the server using the selected IP address and port.
    /// This method is triggered by a UI button click event.
    /// </summary>
    private async void OnConnectButtonClicked()
    {
        if (ip_address_ == null || port_number_ == null)
        {
            Debug.LogWarning("[NetworkManager] IP address dropdown or port dropdown is not assigned.");
            return;
        }

        string ip_address = ip_address_.options[ip_address_.value].text;
        int port = int.Parse(port_number_.options[port_number_.value].text);

        Client.Instance.SetIP(ip_address);
        Client.Instance.SetPort(port);

        DebugLogger.Instance.WriteLog($"[NetworkManager] Server IP address: {ip_address}");
        DebugLogger.Instance.WriteLog($"[NetworkManager] Server Port: {port}");

        try
        {
            if (await Client.Instance.ConnectToServer())
            {
                Client.Instance.SetIsConnected(true);

                if (statusText != null)
                {
                    statusText.text = "Network Connected";
                }

                if (nextButton != null)
                {
                    nextButton.gameObject.SetActive(true);
                }
            }
            else
            {
                DebugLogger.Instance.WriteLog("[NetworkManager] ConnectToServer failed.");
            }
        }
        catch (Exception ex)
        {
            Client.Instance.SetIsConnected(false);
            DebugLogger.Instance.WriteLog($"[NetworkManager] ConnectToServer failed: {ex}");
        }
    }

    /// <summary>
    /// Moves the application flow to the AC setup state.
    /// This is enabled after the network connection succeeds.
    /// </summary>
    private void OnNextButtonClicked()
    {
        AppFlowManager.Instance.ChangeState(AppFlowManager.AppState.ACSetup);
    }

    private int frameCnt;
    private int imageWidth;
    private int imageHeight;

    private float fx, fy, cx, cy;

    /// <summary>
    /// Transformation matrix from locatable camera space to world space.
    /// Position of the locatable camera in world space.
    /// Rotation of the locatable camera in world space.
    /// </summary>
    private Matrix4x4 lc2wMat;
    private Vector3 lc2w_pos;
    private Quaternion lc2w_rot;

    /// <summary>
    /// Transformation matrix from the AC object to world space.
    /// </summary>
    private Matrix4x4 lco2wMat;

    /// <summary>
    /// Transformation matrix from the AC object to locatable camera space.
    /// Relative position of the AC model with respect to the locatable camera.
    /// Relative rotation of the AC model with respect to the locatable camera.
    /// </summary>
    private Matrix4x4 lco2lcMat;
    private Vector3 lco2lc_pos;
    private Quaternion lco2lc_rot;

    /// <summary>
    /// Called when a camera texture is captured.
    /// This method:
    /// 1) extracts image data and camera intrinsics
    /// 2) computes world and relative object pose information
    /// 3) packages the data for transmission to the server
    /// </summary>
    /// <param name="tex">Captured image frame.</param>
    /// <param name="cameraToWorld">Camera-to-world transformation matrix.</param>
    /// <param name="intrinsic">Camera intrinsic matrix.</param>
    public void OnTextureCaptured(Texture2D tex, Matrix4x4 cameraToWorld, Matrix4x4 intrinsic)
    {
        if (tex == null || worldACModel == null)
        {
            DebugLogger.Instance.WriteLog($"[NetworkManager] Texture or world AC model is missing.");
            return;
        }

        /// Save image dimensions
        imageWidth = tex.width;
        imageHeight = tex.height;

        /// Extract intrinsic camera parameters
        fx = intrinsic[0, 0];
        fy = intrinsic[1, 1];
        cx = intrinsic[0, 2];
        cy = intrinsic[1, 2];

        /// Encode the captured texture as a JPG byte array
        byte[] imageBytes = tex.EncodeToJPG();

        /// Compute locatable camera pose in world space
        lc2w_pos = cameraToWorld.MultiplyPoint(Vector3.zero);
        lc2w_rot = Quaternion.LookRotation(cameraToWorld.GetColumn(2), cameraToWorld.GetColumn(1));
        lc2wMat = Matrix4x4.TRS(lc2w_pos, lc2w_rot, Vector3.one);
        //WriteLog($"[Initialization] Left Camera pos: {lc2w_pos}, rot: {lc2w_rot}");

        /// Compute object pose relative to the locatable camera
        lco2wMat = Matrix4x4.TRS(worldACModel.transform.position, worldACModel.transform.rotation, Vector3.one);
        lco2lcMat = lc2wMat.inverse * lco2wMat; // Relative pose Matrix
        lco2lc_pos = lco2lcMat.GetColumn(3);
        lco2lc_rot = Quaternion.LookRotation(lco2lcMat.GetColumn(2), lco2lcMat.GetColumn(1));
        //WriteLog($"[Initialization] Object to Left Camera rel_pos: {lco2lc_pos}, rel_rot: {lco2lc_rot}");

        /// Get current frame count from the passthrough camera manager
        frameCnt = PassthroughManager.Instance.GetFrameCnt();

        if (Client.Instance.IsConnected())
        {
            CustomizedPacket packet = new CustomizedPacket();
            packet.InitializePacket(
                System.DateTime.Now,
                imageBytes,
                tex.width,
                tex.height,
                0, /// label
                frameCnt, /// frameCounter
                lc2w_pos, /// Locatable Camera to World pos
                lc2w_rot, /// Locatable Camera to World rot
                lco2lc_pos, /// LC_ACModel to World pos
                lco2lc_rot, /// LC_ACModel to World rot
                fx,
                fy,
                cx,
                cy
            );
            byte[] sendData = packet.GetPacket();
            Client.Instance.SendPacket(sendData);
        }
    }
}
