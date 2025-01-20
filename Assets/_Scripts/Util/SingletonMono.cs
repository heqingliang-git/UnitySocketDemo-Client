using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    Debug.LogWarning($"{typeof(T).Name} 没有在场景中找到实例，已自动创建");
                    GameObject obj = new GameObject("Singleton");
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public SingletonMono()
    {
        if (instance != null)
        {
            Debug.LogWarning($"已经存在一个 {typeof(T).Name} 的实例，请勿重复创建");
        }
        else
        {
            instance = this as T;
        }
    }
}
