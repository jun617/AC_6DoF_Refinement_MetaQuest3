using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages multiple UI canvases and controls their visibility.
/// Only one canvas is typically active at a time based on the app state.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// List of all canvas GameObjects managed by this UI manager.
    /// These should be assigned via the Unity Inspector.
    /// </summary>
    [SerializeField] private List<GameObject> canvasObjects;

    /// <summary>
    /// Activates the specified canvas and deactivates all others.
    /// </summary>
    /// <param name="canvasName">The name of the canvas to display.</param>
    public void ShowCanvas(string canvasName)
    {
        foreach (var canvas in canvasObjects)
        {
            canvas.SetActive(canvas.name.Equals(canvasName));
        }
    }

    /// <summary>
    /// Deactivates the specified canvas.
    /// </summary>
    /// <param name="canvasName">The name of the canvas to hide.</param>
    public void HideCanvas(string canvasName)
    {
        foreach (var canvas in canvasObjects)
        {
            if (canvas.name.Equals(canvasName))
                canvas.SetActive(false);
        }
    }

    /// <summary>
    /// Deactivates all registered canvases.
    /// </summary>
    public void HideAllCanvas()
    {
        foreach (var canvas in canvasObjects)
        {
            canvas.SetActive(false);
        }
    }
}
