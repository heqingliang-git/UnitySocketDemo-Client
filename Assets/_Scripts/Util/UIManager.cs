using System.Collections.Generic;
using SocketProtocol;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMono<UIManager>
{
    private readonly Dictionary<string, BasePanel> panelDic = new();
    private Transform uiRoot;

    private void Start()
    {
        uiRoot = GameObject.Find(SCENENODENAME.UIRoot).transform;
        foreach (Transform child in uiRoot)
        {
            panelDic.Add(child.name, child.GetComponent<BasePanel>());
            child.gameObject.SetActive(false);
        }
        OpenPanel(UINAME.UserPanel);
    }

    public BasePanel OpenPanel(string panelName)
    {
        // 判断是否实例化，没有则实例化
        if (panelDic.TryGetValue(panelName, out BasePanel panel))
        {
            // 没有显示才打开
            if (!panel.gameObject.activeInHierarchy)
            {
                panel.OpenPanel();
            }
            return panel;
        }
        else
        {
            // 加载资源并实例化
            GameObject prefab = Resources.Load(panelName) as GameObject;
            GameObject go = Instantiate(prefab, uiRoot);
            go.name = panelName;
            BasePanel newPanel = go.GetComponent<BasePanel>();
            panelDic.Add(panelName, newPanel);
            return newPanel;
        }
    }

    public bool ClosePanel(string panelName)
    {
        // 判断是否实例化，没有则返回false
        if (panelDic.TryGetValue(panelName, out BasePanel panel))
        {
            // 显示才关闭
            if (panel.gameObject.activeInHierarchy)
            {
                panel.ClosePanel();
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}

