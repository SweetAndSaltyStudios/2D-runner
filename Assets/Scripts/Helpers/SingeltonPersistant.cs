using UnityEngine;

public abstract class SingeltonPersistant<T> : MonoBehaviour where T : Component
{
    #region VARIABLES

    private static T instance;

    #endregion VARIABLES

    #region PROPERTIES

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    var gameObject = new GameObject
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    instance = gameObject.AddComponent<T>();

                }
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    #endregion PROPERTIES

    #region UNITY_FUNCTIONS

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion UNITY_FUNCTIONS
}
