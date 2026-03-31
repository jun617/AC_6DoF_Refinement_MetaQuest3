using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides a test interface that connects UI inputs to system controllers.
///
/// This class acts as a central hub for:
/// - Air-conditioner control (power, mode, temperature, wind strength)
/// - Airflow visualization control (speed and color)
///
/// It is primarily used for debugging, testing, and demonstration purposes.
/// </summary>
public class TestManager : Singleton<TestManager>
{
    [Header("Power")]
    [SerializeField] private Button powerButton;

    [Header("Wind Strength")]
    [SerializeField] private Button lowButton;
    [SerializeField] private Button midButton;
    [SerializeField] private Button highButton;

    [Header("Temperature")]
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;

    [Header("Mode")]
    [SerializeField] private Button coolButton;
    [SerializeField] private Button dryButton;

    [Header("Status")]
    [SerializeField] private Button statusButton;
    [SerializeField] public TMP_Text StatusText;

    [Header("Airflow Strength")]
    [SerializeField] private Button afLowButton;
    [SerializeField] private Button afMidButton;
    [SerializeField] private Button afHighButton;

    [Header("Airflow Color")]
    [SerializeField] private Button afRedButton;
    [SerializeField] private Button afWhiteButton;
    [SerializeField] private Button afBlueButton;

    /// <summary>
    /// Registers UI button callbacks at runtime.
    /// Each button triggers a corresponding function
    /// in ACController or AirflowController.
    /// </summary>
    private void Start()
    {
        // Power
        if (powerButton != null)
            powerButton.onClick.AddListener(AC_APIController.Instance.OnPowerButtonClick);

        // Wind strength
        if (lowButton != null)
            lowButton.onClick.AddListener(AC_APIController.Instance.OnLowButtonClick);

        if (midButton != null)
            midButton.onClick.AddListener(AC_APIController.Instance.OnMidButtonClick);

        if (highButton != null)
            highButton.onClick.AddListener(AC_APIController.Instance.OnHighButtonClick);

        // Temperature
        if (upButton != null)
            upButton.onClick.AddListener(AC_APIController.Instance.OnUpButtonClick);

        if (downButton != null)
            downButton.onClick.AddListener(AC_APIController.Instance.OnDownButtonClick);

        // Mode
        if (coolButton != null)
            coolButton.onClick.AddListener(AC_APIController.Instance.OnCoolButtonClick);

        if (dryButton != null)
            dryButton.onClick.AddListener(AC_APIController.Instance.OnDryButtonClick);

        // Status
        if (statusButton != null)
            statusButton.onClick.AddListener(AC_APIController.Instance.OnStatusButtonClick);

        // Airflow strength
        if (afLowButton != null)
            afLowButton.onClick.AddListener(AirflowController.Instance.OnLowButtonClick);

        if (afMidButton != null)
            afMidButton.onClick.AddListener(AirflowController.Instance.OnMidButtonClick);

        if (afHighButton != null)
            afHighButton.onClick.AddListener(AirflowController.Instance.OnHighButtonClick);

        // Airflow color
        if (afRedButton != null)
            afRedButton.onClick.AddListener(AirflowController.Instance.OnRedButtonClick);

        if (afWhiteButton != null)
            afWhiteButton.onClick.AddListener(AirflowController.Instance.OnWhiteButtonClick);

        if (afBlueButton != null)
            afBlueButton.onClick.AddListener(AirflowController.Instance.OnBlueButtonClick);
    }
}