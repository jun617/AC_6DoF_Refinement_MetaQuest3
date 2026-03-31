using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a simple airflow visualization using Unity particle systems.
/// 
/// Note:
/// The original project used a physics-based fluid simulation system.
/// In this public version, it is replaced with a simplified particle system.
/// </summary>
public class AirflowController : Singleton<AirflowController>
{
    [SerializeField] private GameObject ParticleSystemPrefab;
    [HideInInspector] public GameObject ParticleSystemInstant = null;

    private ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystemInstant = Instantiate(ParticleSystemPrefab, Vector3.zero, Quaternion.identity);
        ParticleSystemInstant.gameObject.SetActive(false);

        ps = ParticleSystemInstant.GetComponent<ParticleSystem>();
    }
    public void OnRedButtonClick()
    {
        SetColor(new Color(1f, 0f, 0f, 0.8f));
    }
    public void OnWhiteButtonClick()
    {
        SetColor(new Color(1f, 1f, 1f, 1.0f));
    }
    public void OnBlueButtonClick()
    {
        SetColor(new Color(0f, 0f, 1f, 0.8f));
    }

    public void OnLowButtonClick()
    {
        SetSpeed(1f);
    }

    public void OnMidButtonClick()
    {
        SetSpeed(3f);
    }

    public void OnHighButtonClick()
    {
        SetSpeed(6f);
    }

    private void SetColor(Color color)
    {
        if (ps == null) return;

        var main = ps.main;
        main.startColor = color;

        ps.Stop();
        ps.Play();
    }

    private void SetSpeed(float speed)
    {
        if (ps == null) return;

        var main = ps.main;
        main.startSpeed = speed;

        ps.Stop();
        ps.Play();
    }
}
