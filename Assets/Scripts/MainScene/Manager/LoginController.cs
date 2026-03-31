using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the login UI interactions.
/// Handles user input from the login screen and triggers
/// transitions to the next application state.
/// </summary>
public class LoginController : Singleton<LoginController>
{
    /// <summary>
    /// Button to proceed to the next step in the application flow.
    /// </summary>
    public UnityEngine.UI.Button nextButton;

    /// <summary>
    /// Toggle to bypass the normal setup flow and directly enter test mode.
    /// </summary>
    public UnityEngine.UI.Toggle Bypass2TestToggle;

    /// <summary>
    /// Button to open additional settings (currently not implemented).
    /// </summary>
    public UnityEngine.UI.Button settingButton;

    /// <summary>
    /// Initializes UI event listeners when the scene starts.
    /// </summary>
    private void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(()=> OnNextButtonClicked());
        }
        if (settingButton != null)
        {
            settingButton.onClick.AddListener(() => OnSettingButtonClicked());
        }
    }

    /// <summary>
    /// Handles the logic when the "Next" button is clicked.
    /// Depending on the toggle state, it either:
    /// - Skips directly to the test state
    /// - Proceeds to the network setup state
    /// </summary>
    private void OnNextButtonClicked()
    {
        if (Bypass2TestToggle != null && Bypass2TestToggle.isOn == true)
        {
            AppFlowManager.Instance.ChangeState(AppFlowManager.AppState.Test);
        }
        else
        {
            AppFlowManager.Instance.ChangeState(AppFlowManager.AppState.NetworkSetup);
        }
    }

    /// <summary>
    /// Handles the logic when the "Settings" button is clicked.
    /// Currently not implemented.
    /// </summary>
    private void OnSettingButtonClicked()
    {
        Debug.Log("Settings button clicked (not implemented).");
    }
}
