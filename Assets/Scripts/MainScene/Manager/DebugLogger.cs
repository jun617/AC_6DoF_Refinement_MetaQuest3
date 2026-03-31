using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Provides a simple in-app debugging and logging system.
/// 
/// This class allows:
/// - conditional debug logging (enabled/disabled),
/// - displaying logs on a UI text element,
/// - resetting logs,
/// - handling application exit with proper cleanup.
/// </summary>
public class DebugLogger : Singleton<DebugLogger>
{
    /// <summary>
    /// Enables or disables debug logging output.
    /// When disabled, all logging operations are ignored.
    /// </summary>
    [SerializeField] public bool debugEnabled = false;

    /// <summary>
    /// UI text element used to display debug messages.
    /// </summary>
    [SerializeField] private TMP_Text logText;

    /// <summary>
    /// Button used to exit the application.
    /// </summary>
    [SerializeField] private Button exitButton;

    /// <summary>
    /// Registers the exit button callback when the scene starts.
    /// </summary>
    private void Start()
    {
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
    }

    /// <summary>
    /// Enables or disables debug mode.
    /// </summary>
    /// <param name="active">True to enable logging, false to disable.</param>
    public void SetDebugMode(bool active)
    {
        debugEnabled = active;
    }

    /// <summary>
    /// Writes a log message to the UI if debug mode is enabled.
    /// </summary>
    /// <param name="message">The message to append to the log.</param>
    public void WriteLog(string message)
    {
        if (!debugEnabled) return;

        if (logText != null)
        {
            logText.text += "\n" + message;
        }
    }

    /// <summary>
    /// Resets the log display to its initial state.
    /// </summary>
    public void ResetLog()
    {
        if (!debugEnabled) return;

        if (logText != null)
        {
            logText.text = "[Log]";
        }
    }

    /// <summary>
    /// Handles application exit.
    /// 
    /// This method:
    /// 1) closes the network connection,
    /// 2) transitions the application to the End state.
    /// </summary>
    private void OnExitButtonClicked()
    {
        Client.Instance.CloseSocket();
        AppFlowManager.Instance.ChangeState(AppFlowManager.AppState.End);
    }
}