using PassthroughCameraSamples;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles passthrough camera capture, intrinsics extraction,
/// and frame transmission for server-side processing.
/// 
/// This class is responsible for:
/// 1) capturing frames from the device camera,
/// 2) computing camera intrinsics and pose,
/// 3) optionally sending captured data to the server,
/// 4) displaying the camera feed in the UI.
/// </summary>
public class PassthroughManager : Singleton<PassthroughManager>
{
    /// <summary>
    /// Manages access to the device camera texture.
    /// </summary>
    [SerializeField] private WebCamTextureManager webCamTextureManager;

    /// <summary>
    /// Displays camera permission status in the UI.
    /// </summary>
    [SerializeField] private TMP_Text permissionCheck;

    /// <summary>
    /// UI element used to display the passthrough camera image.
    /// </summary>
    [Tooltip("Displays the passthrough camera feed")]
    [SerializeField] private RawImage cameraRawImage;

    private bool hasInitialized = false;

    /// <summary>
    /// Reusable texture to avoid allocating a new Texture2D every frame.
    /// </summary>
    private Texture2D reusableTexture = null;

    /// <summary>
    /// Enables or disables frame capture.
    /// </summary>
    private bool captureEnabled_ = true;

    /// <summary>
    /// Enables or disables sending captured frames to the server.
    /// </summary>
    private bool sendPacketEnabled_ = false;

    /// <summary>
    /// Frame counter used for sampling and synchronization.
    /// </summary>
    private int frameCnt = 0;

    /// <summary>
    /// Waits until the WebCamTexture becomes available.
    /// </summary>
    private IEnumerator Start()
    {
        while (webCamTextureManager.WebCamTexture == null)
        {
            yield return null;
        }

        hasInitialized = true;
        DebugLogger.Instance.WriteLog("[PassthroughManager] WebCamTexture is ready and playing.");
    }

    /// <summary>
    /// Updates permission status and performs periodic frame capture.
    /// </summary>
    private void Update()
    {
        // Display camera permission status
        permissionCheck.text = PassthroughCameraPermissions.HasCameraPermission == true ? "Permission granted." : "No permission granted.";

        if (!hasInitialized || webCamTextureManager.WebCamTexture == null)
        {
            return;
        }

        if (captureEnabled_)
        {
            // Capture every N frames to reduce load
            if (frameCnt % 100 == 0)
            {
                CaptureFrame();
            }

            frameCnt++;
        }
    }

    /// <summary>
    /// Captures a frame from the camera and prepares data for processing.
    /// This includes:
    /// - image extraction
    /// - camera intrinsics computation
    /// - camera pose estimation
    /// </summary>
    public void CaptureFrame()
    {
        Matrix4x4 cameraToWorld = Matrix4x4.identity;
        Matrix4x4 intrinsics = Matrix4x4.identity;

        WebCamTexture camTex = webCamTextureManager.WebCamTexture;
        if (camTex == null) return;

        // (1) Copy camera image into reusable Texture2D
        if (reusableTexture == null)
        {
            reusableTexture = new Texture2D(camTex.width, camTex.height, TextureFormat.RGBA32, false);
        }

        reusableTexture.SetPixels32(camTex.GetPixels32());
        reusableTexture.Apply(false);

        // (2) Compute camera intrinsic matrix (K)
        var cameraEye = webCamTextureManager.Eye;
        var cameraDetails = PassthroughCameraUtils.GetCameraIntrinsics(cameraEye);
        intrinsics = BuildIntrinsicMatrix4x4(cameraDetails);

        // (3) Compute camera-to-world transformation
        var cameraPose = PassthroughCameraUtils.GetCameraPoseInWorld(cameraEye);
        cameraToWorld = Matrix4x4.TRS(cameraPose.position, cameraPose.rotation, Vector3.one);

        // (4) Display camera image in UI
        if (cameraRawImage != null)
        {
            cameraRawImage.texture = reusableTexture;
        }

        // (5) Send captured data to server if enabled
        if (sendPacketEnabled_)
        {
            NetworkManager.Instance.OnTextureCaptured(reusableTexture, cameraToWorld, intrinsics);
        }
    }

    /// <summary>
    /// Builds a 4x4 camera intrinsic matrix from provided parameters.
    /// 
    /// K =
    /// [ fx  skew  cx ]
    /// [ 0    fy   cy ]
    /// [ 0     0    1 ]
    /// </summary>
    private Matrix4x4 BuildIntrinsicMatrix4x4(PassthroughCameraIntrinsics intrinsics)
    {
        float fx = intrinsics.FocalLength.x;
        float fy = intrinsics.FocalLength.y;
        float cx = intrinsics.PrincipalPoint.x;
        float cy = intrinsics.PrincipalPoint.y;
        float skew = intrinsics.Skew;

        Matrix4x4 K = Matrix4x4.identity;

        K[0, 0] = fx;
        K[0, 1] = skew;
        K[0, 2] = cx;

        K[1, 1] = fy;
        K[1, 2] = cy;

        K[2, 2] = 1f;

        return K;
    }

    /// <summary>
    /// Enables or disables frame capture.
    /// </summary>
    public void SetCaptureEnabled(bool captureEnabled)
    {
        captureEnabled_ = captureEnabled;
        DebugLogger.Instance.WriteLog("[PassthroughManager] Capture enabled state changed.");
    }

    /// <summary>
    /// Enables or disables sending captured frames to the server.
    /// </summary>
    public void SetSendCaptureEnabled(bool sendPacketEnabled)
    {
        sendPacketEnabled_ = sendPacketEnabled;
        DebugLogger.Instance.WriteLog("[PassthroughManager] Packet sending state changed.");
    }

    /// <summary>
    /// Returns the current frame count.
    /// </summary>
    public int GetFrameCnt()
    {
        return frameCnt;
    }
}