using UnityEngine;
using System;

public class SingleTon<T> : PGMonoBehaviour where T : SingleTon<T>
{
    private static T s_instance = null;

    private static bool isQuit = false;

    public static T Instance
    {
        get
        {
            if (ReferenceEquals(s_instance, null))
            {
                if (!Application.isPlaying || isQuit)
                {
                    //CGLog.LogError(typeof(T).ToString() + " is can't Access because Game dosen't play.");
                    return null;
                }

                UpdateInstance();
            }
            return s_instance;
        }
    }

    private static void UpdateInstance()
    {
        Type type = typeof(T);
        s_instance = FindObjectOfType(type) as T;
        if (s_instance == null)
        {
            GameObject go = new GameObject(type.Name);
            s_instance = go.AddComponent<T>();
        }
    }

    /// <summary>
    /// ΩÃ±€≈Ê ª˝º∫Ω√ »£√‚
    /// </summary>
    protected virtual void OnAwakeSingleton() { }

    /// <summary>
    /// ΩÃ±€≈Ê ¡¶∞≈Ω√ »£√‚
    /// </summary>
    protected virtual void OnDestroySingleton() { }
    

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this as T;
        }
        else if (s_instance != this)
        {
            Destroy(this);
            return;
        }

        OnAwakeSingleton();
    }

    private void OnDestroy()
    {
        if (s_instance == this)
        {
            s_instance = null;
            OnDestroySingleton();
        }
    }

    private void OnApplicationQuit()
    {
        isQuit = true;
    }
}
