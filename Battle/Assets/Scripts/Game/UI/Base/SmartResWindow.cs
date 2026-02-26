using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using DG.Tweening;


public class SmartResWindow : Window{
    List<string> packList = new List<string>();

    /// <summary>
    /// 是否不透明全屏显示
    /// </summary>
    /// <returns></returns>
    public virtual bool IsOpaqueFullScreen()
    {
        return false;
    }

    /// <summary>
    /// 再次显示，如果需要刷新，需继承被刷新
    /// </summary>
    public virtual void ShowAgain()
    {
        Show();
    }

    protected void AddPackage(string str)
    {
        UIManager.instance.AddPackage(str);
        packList.Add(str);
    }

    protected void RemovePackage(string str)
    {
        UIManager.instance.RemovePackage(str);
        packList.Remove(str);
    }

    public override void Dispose()
    {
        if (this != null && Application.isPlaying)
        {
            base.Dispose();
        }
        var enumer = packList.GetEnumerator();
        while (enumer.MoveNext())
        {
            UIManager.instance.RemovePackage(enumer.Current);
        }
        packList.Clear();
    }


    public static void SendNotifycation(object n, object note = null)
    {
        NotificationCenter.Instance.DispatchEvent(n, note);
    }

    public static void RegisterNotifycation(object n, OnNotificationDelegate de)
    {
        NotificationCenter.Instance.AddEventListener(n, de);
    }

    public static void UnRegisterNotifycation(object n, OnNotificationDelegate de)
    {
        NotificationCenter.Instance.RemoveEventListener(n, de);
    }
}
