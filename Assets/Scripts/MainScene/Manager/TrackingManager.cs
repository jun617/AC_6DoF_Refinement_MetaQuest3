using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// Manages object placement and pose refinement based on server-side tracking results.
/// 
/// This class handles:
/// 1) initial AC model placement in the scene,
/// 2) starting image capture for server-based refinement,
/// 3) updating the AC model pose using refined tracking results,
/// 4) transitioning to airflow visualization after tracking is stabilized.
/// </summary>

public class TrackingManager : Singleton<TrackingManager>
{
    [Header("ACSetup1")]
    [SerializeField] public UnityEngine.UI.Button renderButton;
    [SerializeField] public UnityEngine.UI.Button deleteButton;
    [SerializeField] public UnityEngine.UI.Button next1Button;

    [Header("ACSetup2")]
    [SerializeField] public UnityEngine.UI.Button next2Button;
    [SerializeField] private GameObject worldACModel;


    private bool isFirstReceived = false;
    private int receiveCnt = 0;

    private Matrix4x4 mc2wMat;
    private Matrix4x4 o2mcMat;
    private Matrix4x4 o2wMat;

    private Vector3 spawn_pos;
    private Quaternion spawn_rot;
    private Vector3 spawn_rot_euler;

    private Vector3 relative_pos;
    private Quaternion relative_rot;

    private void Start()
    {
        // ACSetup1
        if (renderButton != null)
        {
            renderButton.onClick.AddListener(() => OnRenderButtonClicked());
        }
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(() => OnDeleteButtonClicked());
        }
        if (next1Button != null)
        {
            next1Button.onClick.AddListener(() => OnNext1ButtonClicked());
        }
        // ACSetup1
        if (next2Button != null)
        {
            next2Button.onClick.AddListener(() => OnNext2ButtonClicked());
        }

    }

    /// <summary>
    /// Places the AC model in front of the main camera using a simple initial pose.
    /// This pose does not need to be accurate and is later refined by the tracking server.
    /// </summary>
    private void OnRenderButtonClicked()
    {
        // Get MainCamera to World pose (Doesn't have to be accurate)
        mc2wMat = Matrix4x4.TRS(Camera.main.transform.position, Camera.main.transform.rotation, Vector3.one);

        // Set relative pose LC_ACModel to MainCamera
        relative_pos = Vector3.forward * 2.5f;
        relative_rot = Quaternion.identity;
        o2mcMat = Matrix4x4.TRS(relative_pos, relative_rot, Vector3.one);

        // Calculate LC_ACModel to World pose
        o2wMat = mc2wMat * o2mcMat;

        // Get position and rotation of LC_ACModel to World
        spawn_pos = o2wMat.GetColumn(3);
        spawn_rot = Quaternion.LookRotation(o2wMat.GetColumn(2), o2wMat.GetColumn(1));

        // Simple manual adjustment for a convenient initial placement.
        spawn_pos.y = 0.9f;

        spawn_rot_euler = spawn_rot.eulerAngles; // Quaternion to Euler
        spawn_rot_euler.x = 0f;
        spawn_rot.y = 0f;
        spawn_rot = Quaternion.Euler(spawn_rot_euler); // Euler to Quaternion

        // Spawn LC_ACModel
        worldACModel.SetActive(true);
        worldACModel.transform.position = spawn_pos;
        worldACModel.transform.rotation = spawn_rot;
        return;
    }

    /// <summary>
    /// Hides the AC model from the scene.
    /// </summary>
    private void OnDeleteButtonClicked()
    {
        worldACModel.SetActive(false);
        return;
    }

    /// <summary>
    /// Moves to the second setup step and starts image capture for server-based refinement.
    /// </summary>
    private void OnNext1ButtonClicked()
    {
        UIManager.Instance.HideCanvas("ACSetupCanvas");
        UIManager.Instance.ShowCanvas("ACSetup2Canvas");
        CaptureStart();
    }

    /// <summary>
    /// Enables passthrough capture and transmission to the server.
    /// </summary>
    private void CaptureStart()
    {
        PassthroughManager.Instance.SetCaptureEnabled(true);

        //yield return new WaitForSeconds(1f);
        PassthroughManager.Instance.SetSendCaptureEnabled(true);
        DebugLogger.Instance.WriteLog("[TrackingManager] SendCapture Started.");
    }

    // Tracking: Data Received from Server
    private Matrix4x4 rf_lco2lcMat; // Refined LC_ACModel to Locatable Camera Matrix
    private Matrix4x4 rf_lc2wMat; // Previous Locatable Camera to World pose

    private Matrix4x4 rf_lco2wMat; // Refined LC_ACModel to World Matrix
    private Vector3 rf_lco2w_pos; // Refined LC_ACModel to World position
    private Quaternion rf_lco2w_rot; // Refined LC_ACModel to World position

    private Vector3 rf_lco2lc_pos; // Refined LC_ACModel to Locatable Camera position
    private Quaternion rf_lco2lc_rot; // Refined LC_ACModel to Locatable Camera rotation
    private Matrix4x4 rf_lco2lc_posMat; // Refined LC_ACModel to Locatable Camera position Matrix
    private Matrix4x4 rf_lco2lc_rotMat; // Refined LC_ACModel to Locatable Camera rotation Matrix

    /// <summary>
    /// Called when refined tracking results are received from the server.
    /// Converts the refined object pose from camera space into world space
    /// and updates the AC model accordingly.
    /// </summary>
    public void OnReceivedServerData(Vector3 refinedPos, Quaternion refinedRot, Vector3 hmdPos, Quaternion hmdRot, float lost)
    {
        // Return when no refined pose
        if (lost == 1.0f)
        {
            return;
        }

        // Refined LC_ACModel to Locatable Camera pose
        rf_lco2lcMat = Matrix4x4.TRS(refinedPos, refinedRot, Vector3.one);

        // Locatable Camera to World pose (Which was sent with frame previously)
        rf_lc2wMat = Matrix4x4.TRS(hmdPos, hmdRot, Vector3.one);

        // Calculate Refined LC_ACModel to World pose
        rf_lco2wMat = rf_lc2wMat * rf_lco2lcMat;

        // Get LC_ACModel to World position and rotation
        rf_lco2w_pos = rf_lco2wMat.GetColumn(3);
        rf_lco2w_rot = Quaternion.LookRotation(rf_lco2wMat.GetColumn(2), rf_lco2wMat.GetColumn(1));

        if (!isFirstReceived)
        {
            isFirstReceived = true;
        }

        if (receiveCnt < 10)
        {
            UpdateAC_Pose();
            receiveCnt = receiveCnt + 1;
        }
        else if (receiveCnt == 10)
        {
            PassthroughManager.Instance.SetCaptureEnabled(false);
            PassthroughManager.Instance.SetSendCaptureEnabled(false);
            AirflowController.Instance.ParticleSystemInstant.gameObject.SetActive(true);
            worldACModel.gameObject.SetActive(false);
            UpdatePS_Pose();
            DebugLogger.Instance.WriteLog($"[TrackingManager] Tracking done.");
        }
    }

    /// <summary>
    /// Updates the AC model pose in world space.
    /// </summary>
    private void UpdateAC_Pose()
    {
        worldACModel.transform.position = rf_lco2w_pos;
        worldACModel.transform.rotation = rf_lco2w_rot;
        return;
    }

    /// <summary>
    /// Updates the airflow particle system pose in world space.
    /// </summary>
    private void UpdatePS_Pose()
    {
        AirflowController.Instance.ParticleSystemInstant.transform.position = rf_lco2w_pos;
        AirflowController.Instance.ParticleSystemInstant.transform.rotation = rf_lco2w_rot;
        return;
    }

    /// <summary>
    /// Moves the application flow to the test state.
    /// </summary>
    private void OnNext2ButtonClicked()
    {
        AppFlowManager.Instance.ChangeState(AppFlowManager.AppState.Test);
    }
}
