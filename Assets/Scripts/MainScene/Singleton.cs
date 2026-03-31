using UnityEngine;

/// <summary>
/// Generic singleton base class for MonoBehaviour components.
/// 
/// This class ensures that only one instance of a given type exists
/// in the scene and provides a global access point via Instance.
/// 
/// If no instance is found in the scene, a new GameObject is created
/// and the component is automatically attached.
/// </summary>
/// <typeparam name="T">The MonoBehaviour type to be used as a singleton.</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// The single instance of the class.
    /// </summary>
    private static T instance;

    /// <summary>
    /// Global access point to the singleton instance.
    /// 
    /// If an instance does not exist in the scene, it will:
    /// 1) search for an existing instance,
    /// 2) create a new GameObject and attach the component if none is found.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Try to find an existing instance in the scene
                // Note: FindObjectOfType can be relatively expensive,
                // so Instance should not be accessed excessively during runtime.
                instance = FindObjectOfType<T>();

                // If no instance exists, create a new GameObject and attach the component
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Ensures that only one instance exists.
    /// 
    /// If another instance already exists, this object is destroyed.
    /// Otherwise, this instance is preserved across scene loads.
    /// </summary>
    protected virtual void Awake()
    {
        if (instance != null && instance != this as T)
        {
            // Destroy duplicate instance
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;

            // Prevent destruction when loading a new scene
            DontDestroyOnLoad(gameObject);
        }
    }
}
