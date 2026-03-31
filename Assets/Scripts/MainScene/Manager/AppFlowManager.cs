using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Manages the overall application flow by controlling transitions
/// between high-level app states such as login, setup, testing,
/// and user study.
/// </summary>
public class AppFlowManager : Singleton<AppFlowManager>
{
    /// <summary>
    /// Defines the major states of the application flow.
    /// </summary>
    public enum AppState
    {
        Login,
        SceneLoad, /// Reserved for future implementation
        TrackingCheck, /// Reserved for future implementation
        NetworkSetup,
        ACSetup,
        Test,
        UserStudy, /// Reserved for future implementation
        End
    }

    /// <summary>
    /// Stores the current state of the application.
    /// The app starts from the Login state.
    /// </summary>
    private AppState currentState = AppState.Login;

    /// <summary>
    /// Initializes the application flow when the scene starts.
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        ChangeState(AppState.Login);
    }

    /// <summary>
    /// Changes the current application state.
    /// This method first executes the exit logic of the current state,
    /// then updates the state value, and finally executes the enter logic
    /// of the new state.
    /// </summary>
    /// <param name="newState">The next state to enter.</param>
    public void ChangeState(AppState newState)
    {
        /// Exit logic for the current state
        switch (currentState)
        {
            case AppState.Login:
                ExitLoginState();
                break;
            case AppState.NetworkSetup:
                ExitNetworkSetupState();
                break;
            case AppState.ACSetup:
                ExitACSetupState();
                break;
            case AppState.Test:
                ExitTestState();
                break;
            case AppState.UserStudy:
                ExitUserStudyState();
                break;
        }

        /// Update the current state
        currentState = newState;

        /// Enter logic for the new state
        switch (newState)
        {
            case AppState.Login:
                EnterLoginState();
                break;
            case AppState.NetworkSetup:
                EnterNetworkSetupState();
                break;
            case AppState.ACSetup:
                EnterACSetupState();
                break;
            case AppState.Test:
                EnterTestState();
                break;
            case AppState.UserStudy:
                EnterUserStudyState();
                break;
            case AppState.End:
                EnterEndState();
                break;
        }
    }

    /// <summary>
    /// Enters the login state and displays the login UI.
    /// </summary>
    private void EnterLoginState()
    {
        UIManager.Instance.HideAllCanvas();
        UIManager.Instance.ShowCanvas("LoginCanvas");
    }

    /// <summary>
    /// Exits the login state and hides the login UI.
    /// </summary>
    private void ExitLoginState()
    {
        UIManager.Instance.HideCanvas("LoginCanvas");

    }

    /// <summary>
    /// Enters the network setup state and displays the network setup UI.
    /// </summary>
    private void EnterNetworkSetupState()
    {
        UIManager.Instance.ShowCanvas("NetworkCanvas");
    }

    /// <summary>
    /// Exits the network setup state and hides the network setup UI.
    /// </summary>
    private void ExitNetworkSetupState()
    {
        UIManager.Instance.HideCanvas("NetworkCanvas");

    }

    /// <summary>
    /// Enters the air-conditioner setup state and displays the first setup UI.
    /// </summary>
    private void EnterACSetupState()
    {
        UIManager.Instance.ShowCanvas("ACSetup1Canvas");
    }

    /// <summary>
    /// Exits the air-conditioner setup state and hides the related setup UI.
    /// </summary>
    private void ExitACSetupState()
    {
        UIManager.Instance.HideCanvas("ACSetup2Canvas");
    }

    /// <summary>
    /// Enters the user study state.
    /// UI behavior is currently not implemented.
    /// </summary>
    private void EnterUserStudyState()
    {
    }

    /// <summary>
    /// Exits the user study state.
    /// UI behavior is currently not implemented.
    /// </summary>
    private void ExitUserStudyState()
    {
    }

    /// <summary>
    /// Enters the test state and displays the test UI.
    /// </summary>
    private void EnterTestState()
    {
        UIManager.Instance.ShowCanvas("TestCanvas");
    }

    /// <summary>
    /// Exits the test state and hides the test UI.
    /// </summary>
    private void ExitTestState()
    {
        UIManager.Instance.HideCanvas("TestCanvas");
    }

    /// <summary>
    /// Enters the end state and terminates the application.
    /// </summary>
    public void EnterEndState()
    {
        Application.Quit();
    }
}
