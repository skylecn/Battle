using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

/// <summary>
/// 页窗口
/// </summary>
public class PageWindow : SmartResWindow{


    public virtual WindowPage selfPage
    {
        get { return WindowPage.Null; }
    }
    

    /// <summary>
    /// 默认初始化
    ///     因为窗口会存在缓存池中，务必在这里重置成员变量
    /// </summary>
    public virtual void InitData(object args)
    {

    }

    /// <summary>
    /// 刷新
    ///     返回此窗口时触发刷新
    /// </summary>
    public virtual void Refresh()
    {

    }

    /// <summary>
    /// 屏幕位置是否在组件范围内
    /// </summary>
    /// <param name="pointerPos"></param>
    /// <param name="component"></param>
    /// <returns></returns>
    protected bool HitTest(Vector2 pointerPos, GComponent component)
    {
        pointerPos.y = Stage.inst.stageHeight - pointerPos.y;
        return component.rootContainer.HitTest(pointerPos, true) != null;
    }
    

    /// <summary>
    /// 重连成功时处理
    /// </summary>
    /// <returns>false，不处理</returns>
    public virtual bool OnReconnectFinish()
    {
        return false;
    }
    /// <summary>
    /// Page窗口参数传递
    /// </summary>
    /// <param name="args"></param>
    public virtual void SetData(object args)
    {

    }

    public override bool IsOpaqueFullScreen()
    {
        return true;
    }

    public override void ShowAgain()
    {
        base.ShowAgain();
        Refresh();
    }
}
