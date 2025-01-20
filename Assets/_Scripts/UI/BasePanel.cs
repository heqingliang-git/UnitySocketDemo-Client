using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
    public virtual void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    public virtual void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

