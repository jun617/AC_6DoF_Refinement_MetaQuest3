using UnityEngine;

/// <summary>
/// Handles user interactions for air-conditioner-related controls in the demo.
/// 
/// In the original project, this controller was integrated with an external
/// device API for real air-conditioner monitoring and control.
/// In this public version, the external API integration is removed and
/// replaced with local state updates and airflow visualization control.
/// </summary>
public class AC_APIController : Singleton<AC_APIController>
{
    private bool isPowerOn = false;
    private int targetTemperature = 24;
    private string currentMode = "COOL";
    private string currentWindStrength = "MID";

    private void Start()
    {
        UpdateStatusText();
    }

    /// <summary>
    /// Toggles the power state.
    /// </summary>
    public void OnPowerButtonClick()
    {
        isPowerOn = !isPowerOn;
        DebugLogger.Instance.WriteLog($"[AC_APIController] Power toggled: {isPowerOn}");
        UpdateStatusText();
    }

    /// <summary>
    /// Sets wind strength to low and updates the airflow visualization.
    /// </summary>
    public void OnLowButtonClick()
    {
        currentWindStrength = "LOW";
        DebugLogger.Instance.WriteLog("[AC_APIController] Wind strength set to LOW");
        AirflowController.Instance.OnLowButtonClick();
        UpdateStatusText();
    }

    /// <summary>
    /// Sets wind strength to medium and updates the airflow visualization.
    /// </summary>
    public void OnMidButtonClick()
    {
        currentWindStrength = "MID";
        DebugLogger.Instance.WriteLog("[AC_APIController] Wind strength set to MID");
        AirflowController.Instance.OnMidButtonClick();
        UpdateStatusText();
    }

    /// <summary>
    /// Sets wind strength to high and updates the airflow visualization.
    /// </summary>
    public void OnHighButtonClick()
    {
        currentWindStrength = "HIGH";
        DebugLogger.Instance.WriteLog("[AC_APIController] Wind strength set to HIGH");
        AirflowController.Instance.OnHighButtonClick();
        UpdateStatusText();
    }

    /// <summary>
    /// Increases the target temperature.
    /// </summary>
    public void OnUpButtonClick()
    {
        targetTemperature += 1;
        DebugLogger.Instance.WriteLog($"[AC_APIController] Target temperature increased to {targetTemperature}");
        UpdateStatusText();
    }

    /// <summary>
    /// Decreases the target temperature.
    /// </summary>
    public void OnDownButtonClick()
    {
        targetTemperature -= 1;
        DebugLogger.Instance.WriteLog($"[AC_APIController] Target temperature decreased to {targetTemperature}");
        UpdateStatusText();
    }

    /// <summary>
    /// Sets the operation mode to COOL.
    /// </summary>
    public void OnCoolButtonClick()
    {
        currentMode = "COOL";
        DebugLogger.Instance.WriteLog("[AC_APIController] Mode set to COOL");
        UpdateStatusText();
    }

    /// <summary>
    /// Sets the operation mode to DRY.
    /// </summary>
    public void OnDryButtonClick()
    {
        currentMode = "DRY";
        DebugLogger.Instance.WriteLog("[AC_APIController] Mode set to DRY");
        UpdateStatusText();
    }

    /// <summary>
    /// Displays the current local demo status.
    /// </summary>
    public void OnStatusButtonClick()
    {
        DebugLogger.Instance.WriteLog("[AC_APIController] Status requested");
        UpdateStatusText();
    }

    /// <summary>
    /// Resets the status UI text.
    /// </summary>
    private void ResetStatusText()
    {
        if (TestManager.Instance.StatusText != null)
        {
            TestManager.Instance.StatusText.text = "[Status]\n";
        }
    }

    /// <summary>
    /// Writes the current demo state to the status UI text.
    /// </summary>
    private void UpdateStatusText()
    {
        ResetStatusText();
        WriteStatusText($"Power: {(isPowerOn ? "ON" : "OFF")}");
        WriteStatusText($"Mode: {currentMode}");
        WriteStatusText($"Wind Strength: {currentWindStrength}");
        WriteStatusText($"Target Temperature: {targetTemperature}");
    }

    /// <summary>
    /// Appends a line of text to the status UI.
    /// </summary>
    private void WriteStatusText(string message)
    {
        if (TestManager.Instance.StatusText != null)
        {
            TestManager.Instance.StatusText.text += message + "\n";
        }
    }
}