using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T obj = Component.FindObjectOfType(typeof(T)) as T;
                if (obj)
                {
                    instance = obj;
                }
                else
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    obj = go.AddComponent<T>() as T;
                    instance = obj;
                }
            }
            return instance;
        }
    }

    //Awake時に呼び出す.
    protected void Init(bool dontDestroyOnLoad = true)
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
    }
}
