using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using System;
using DG.Tweening;
using LitJson;

/// <summary>
/// UI窗口和资源的管理
/// </summary>
public partial class UIManager
{
    public static int MAX_PAGE_POOL = 3; // 最大窗口数目
    public static int MAX_ENFORCE_POOL = 3; // 最大弹窗池数目


    //EnforceWindow 1000 弹窗
    //WarningWindow 3000 确认弹窗
    //PopWarningWindow 3100 提示窗口
    //TurningWaitWindow 3200 菊花

    #region Singleton

    private class SingletonNested
    {
        static SingletonNested()
        {
        }

        internal static readonly UIManager instance = new UIManager();
    }

    public static UIManager instance
    {
        get { return SingletonNested.instance; }
    }

    #endregion

    //pageWindow工厂函数
    delegate PageWindow PageFactoryNew();

    Dictionary<WindowPage, PageFactoryNew> pageWindowFactory = new Dictionary<WindowPage, PageFactoryNew>();

    // pageWindow回收池
    LinkedList<PageWindow> recyclePageList = new LinkedList<PageWindow>();

    // 后退窗口列表
    LinkedList<PageWindow> backPageList = new LinkedList<PageWindow>();

    //enforceWindow工厂函数
    delegate EnforceWindow EnforceFactoryNew();

    Dictionary<EnforceWindowPage, EnforceFactoryNew> enforceWindowFactory =
        new Dictionary<EnforceWindowPage, EnforceFactoryNew>();

    // enforceWindow回收池
    LinkedList<EnforceWindow> recycleEnforceList = new LinkedList<EnforceWindow>();

    // 可见的enforceWindow
    LinkedList<EnforceWindow> enforceWindowList = new LinkedList<EnforceWindow>();

    // 打开窗口的队列（只包括enforce)，为了优化全屏UI的OverDraw
    LinkedList<SmartResWindow> openedWindowQueue = new LinkedList<SmartResWindow>();

    //root component
    public GComponent cEntityUIRoot { get; protected set; }

    public static void InitConfig()
    {
        UIConfig.modalLayerColor = new Color(0, 0, 0, 0);
        UIConfig.bringWindowToFrontOnClick = false;
        GRoot.inst.SetContentScaleFactor(1080, 1920, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);

        var camera = StageCamera.main;
        camera.allowHDR = false;
        camera.allowDynamicResolution = false;
        camera.useOcclusionCulling = false;
        camera.allowMSAA = true;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        AddPackage("Common");

        RegisterPageWindow();
        RegisterEnforceWindow();

        RegisterComponent();

        CreateEntityUiParentRoot();
    }

    //create root component of entity ui
    protected void CreateEntityUiParentRoot()
    {
        if (cEntityUIRoot == null)
        {
            cEntityUIRoot = new GComponent();
            cEntityUIRoot.name = "entityUIRoot";
            GRoot.inst.AddChild(cEntityUIRoot);
        }
    }

    //delete root component of entity ui
    protected void DeleteEntityUIParentRoot()
    {
        if (cEntityUIRoot != null)
        {
            cEntityUIRoot.Dispose();
            cEntityUIRoot = null;
        }
    }

    /// <summary>
    /// 关闭UIManager
    /// </summary>
    public void Release()
    {
        if (Application.isEditor)
        {
            // 为了避免和FairyGUI的处理冲突
            packageCount.Clear();
        }

        //RemovePackage("UI/PublicUI");
        //RemovePackage("UI/CommonTools");

        ClearAllUI();
        DeleteEntityUIParentRoot();
    }

    /// <summary>
    /// 关闭所有UI并清空回收池
    /// </summary>
    public void ClearAllUI()
    {
        ClearBackPageList();
        ClearRecylePage();

        ClearAllEnforceWindow();

        ClearOpenedQueue();

        ClearRecyleEnforce();

        UnloadAllTexture();

        RemoveUnusedPackage();
    }

    void FreeRecycle()
    {
        ClearRecylePage();
        ClearRecyleEnforce();
        //ClearRecycleLuaWindow();

        RemoveUnusedPackage();
    }

    public bool FreeMemory()
    {
        textureManager.FreeMemory();

        FreeRecycle();


        return true;
    }

    //////////////////////////////////////////////////////page/////////////////////////////////////////////

    /// <summary>
    /// 注册创建page工厂函数
    /// </summary>
    /// <param name="window"></param>
    /// <param name="factory"></param>
    void RegisterPageWindow(WindowPage window, PageFactoryNew factory)
    {
        pageWindowFactory[window] = factory;
    }

    void RegisterPageWindow()
    {
        RegisterPageWindow(WindowPage.MainWindow, () => new MainWindow());
    }

    /// <summary>
    /// 新建pageWindow
    /// </summary>
    /// <param name="windowPage"></param>
    /// <returns></returns>
    PageWindow NewPage(WindowPage windowPage)
    {
        if (pageWindowFactory.ContainsKey(windowPage))
        {
            return pageWindowFactory[windowPage]();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 创建pageWindow，优先使用回收池中的
    /// </summary>
    /// <param name="windowPage"></param>
    /// <returns></returns>
    PageWindow CreatePageWindow(WindowPage windowPage, object args = null)
    {
        var wp = PopRecylePage(windowPage);
        if (wp == null)
        {
            wp = NewPage(windowPage);
            wp.Show();
        }

        wp.InitData(args);
        return wp;
    }

    PageWindow curPageWindow
    {
        get
        {
            if (backPageList.Count > 0)
            {
                return backPageList.Last.Value;
            }

            return null;
        }
    }

    /// <summary>
    /// 跳转到pageWindow
    /// </summary>
    /// <param name="page">窗口类型</param>
    /// <param name="args">初始化参数</param>
    /// <param name="next">是否向前跳转</param>
    /// <param name="flip">是否有翻书动画</param>
    /// <returns></returns>
    public PageWindow OpenPageWindow(WindowPage page, object args = null)
    {
        var wp = CreatePageWindow(page, args);
        if (curPageWindow != null)
        {
            curPageWindow.HideImmediately();
        }

        PushBackPageList(wp);
        return wp;
    }

    /// <summary>
    /// 后退
    /// </summary>
    public bool BackPageWindow()
    {
        // 只有一个窗口时不能后退
        if (backPageList.Count > 1)
        {
            curPageWindow.HideImmediately();
            AddRecylePage(curPageWindow);
            PopBackPageList();
            curPageWindow.Show();
            curPageWindow.Refresh();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 压入后退列表
    /// </summary>
    /// <param name="page"></param>
    void PushBackPageList(PageWindow page)
    {
        backPageList.AddLast(page);
    }

    /// <summary>
    /// 弹出后退
    /// </summary>
    /// <returns></returns>
    public void PopBackPageList()
    {
        backPageList.RemoveLast();
    }

    /// <summary>
    /// 清空后退列表
    /// </summary>
    public void ClearBackPageList()
    {
        var enumer = backPageList.GetEnumerator();
        while (enumer.MoveNext())
        {
            AddRecylePage(enumer.Current);
        }

        backPageList.Clear();
    }

    /// <summary>
    /// 回收pageWindow
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public void AddRecylePage(PageWindow page)
    {
        recyclePageList.AddLast(page);
        if (recyclePageList.Count > MAX_PAGE_POOL)
        {
            //防止断线重连报错不能返回主城
            if (recyclePageList.First.Value != null)
            {
                recyclePageList.First.Value.Dispose();
            }

            recyclePageList.RemoveFirst();
        }
    }

    /// <summary>
    /// 推出pageWindow
    /// </summary>
    /// <param name="wp"></param>
    /// <returns></returns>
    public PageWindow PopRecylePage(WindowPage wp)
    {
        var enumer = recyclePageList.GetEnumerator();
        while (enumer.MoveNext())
        {
            if (enumer.Current != null && enumer.Current.selfPage == wp)
            {
                var page = enumer.Current;
                recyclePageList.Remove(page);
                return page;
            }
        }

        return null;
    }

    /// <summary>
    /// 清空回收池
    /// </summary>
    void ClearRecylePage()
    {
        var enumer = recyclePageList.GetEnumerator();
        while (enumer.MoveNext())
        {
            if (enumer.Current != null)
            {
                enumer.Current.Dispose();
            }
        }

        recyclePageList.Clear();
    }


    public Window GetWindowByName(string winName)
    {
        if (string.IsNullOrEmpty(winName))
        {
            Debug.LogError("GetWindowByName winName is null");
            return null;
        }

        if (Enum.IsDefined(typeof(WindowPage), winName) && curPageWindow.selfPage.ToString() == winName)
        {
            return curPageWindow;
        }

        if (Enum.IsDefined(typeof(EnforceWindowPage), winName))
        {
            EnforceWindowPage pageType = (EnforceWindowPage)Enum.Parse(typeof(EnforceWindowPage), winName);
            var iter = enforceWindowList.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.selfPage == pageType)
                {
                    return iter.Current;
                }
            }
        }

        return null;
    }

    public Vector2 GetChildPos(string winName, string child, bool isSoft = false)
    {
        Window win = GetWindowByName(winName);
        if (win != null)
        {
            var childList = child.Split('/');
            var cur = win.contentPane.GetChild(childList[0]);
            int i = 1;
            while (cur != null && i < childList.Length)
            {
                GList list = cur as GList;
                int listIndex;
                if (list != null && int.TryParse(childList[i], out listIndex))
                {
                    list.ScrollToView(listIndex);
                    if (list.isVirtual)
                    {
                        int childIndex = list.ItemIndexToChildIndex(listIndex);
                        if (childIndex >= 0 && childIndex < list.numChildren)
                        {
                            cur = list.GetChildAt(childIndex);
                        }
                    }
                    else
                    {
                        cur = list.GetChildAt(listIndex);
                    }
                }
                else
                {
                    cur = cur.asCom.GetChild(childList[i]);
                }

                i++;
            }

            if (i == childList.Length && cur != null)
            {
                GComponent com = cur.asCom;
                if (com != null)
                {
                    GObject guidePoint;
                    if (isSoft)
                    {
                        guidePoint = com.GetChild("softGuidePoint");
                    }
                    else
                    {
                        guidePoint = com.GetChild("guidePoint");
                    }

                    if (guidePoint != null)
                    {
                        return guidePoint.LocalToRoot(Vector2.zero, GRoot.inst);
                    }
                }

                return cur.LocalToRoot(Vector2.zero, GRoot.inst);
            }
        }

        return Vector2.zero;
    }

    public GObject GetChildGObj(string winName, string child)
    {
        GObject gObject = null;
        Window win = GetWindowByName(winName);
        if (win != null)
        {
            var childList = child.Split('/');
            var cur = win.contentPane.GetChild(childList[0]);
            int i = 1;
            while (cur != null && i < childList.Length)
            {
                GList list = cur as GList;
                int listIndex = 0;
                if (list != null && int.TryParse(childList[i], out listIndex))
                {
                    list.ScrollToView(listIndex);
                    if (list.isVirtual)
                    {
                        int childIndex = list.ItemIndexToChildIndex(listIndex);
                        if (childIndex >= 0 && childIndex < list.numChildren)
                        {
                            cur = list.GetChildAt(childIndex);
                        }
                    }
                    else
                    {
                        if (listIndex < list.numChildren)
                        {
                            cur = list.GetChildAt(listIndex);
                        }
                    }
                }
                else
                {
                    cur = cur.asCom.GetChild(childList[i]);
                }

                i++;
            }

            gObject = cur;
        }

        return gObject;
    }

    public bool ReconnectFinish()
    {
        if (curPageWindow != null)
            return curPageWindow.OnReconnectFinish();

        return false;
    }


    ///////////////////////////////////////////enforce/////////////////////////////////////////


    void RegisterEnforceWindow(EnforceWindowPage enforce, EnforceFactoryNew factory)
    {
        enforceWindowFactory[enforce] = factory;
    }

    void RegisterEnforceWindow()
    {
        RegisterEnforceWindow(EnforceWindowPage.LevelWindow, () => new LevelWindow());
    }


    public EnforceWindow NewEnforceWindow(EnforceWindowPage page)
    {
        if (enforceWindowFactory.ContainsKey(page))
        {
            return enforceWindowFactory[page]();
        }
        else
        {
            return null;
        }
    }

    public EnforceWindow OpenEnforceWindow(EnforceWindowPage page, object args = null)
    {
        var enforceWin = PopRecyleEnforce(page);
        if (enforceWin == null)
        {
            enforceWin = NewEnforceWindow(page);
        }

        enforceWindowList.AddLast(enforceWin);
        PushOpenedQueue(enforceWin);

        if (enforceWin == null)
        {
            return null;
        }

        enforceWin.Show();
        enforceWin.MakeFullScreen();

        enforceWin.SetData(args);

        return enforceWin;
    }

    /// <summary>
    /// 如果已经存在则不打开
    /// </summary>
    /// <param name="page"></param>
    /// <param name="isSingle"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public EnforceWindow OpenEnforceWindow(EnforceWindowPage page, bool isSingle, object args = null)
    {
        if (isSingle)
        {
            var iter = enforceWindowList.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.selfPage == page)
                {
                    iter.Current.SetData(args);
                    return iter.Current;
                }
            }

            return OpenEnforceWindow(page, args);
        }

        return OpenEnforceWindow(page, args);
    }

    public void CloseEnforceWindow()
    {
        if (enforceWindowList.Count > 0)
        {
            var lastEnforce = enforceWindowList.Last.Value;
            lastEnforce.Hide();
            AddRecyleEnforce(lastEnforce);
            enforceWindowList.RemoveLast();
            RemoveOpenedQueue(lastEnforce);
        }
    }


    public void CloseEnforceWindow(EnforceWindow ew)
    {
        var enumer = enforceWindowList.GetEnumerator();
        while (enumer.MoveNext())
        {
            if (enumer.Current == ew)
            {
                ew.Hide();
                enforceWindowList.Remove(ew);
                RemoveOpenedQueue(ew);
                AddRecyleEnforce(ew);
                break;
            }
        }
    }

    public void CloseEnforceWindow(EnforceWindowPage page)
    {
        if (enforceWindowList.Count <= 0) return;

        var cur = enforceWindowList.First;
        do
        {
            var next = cur.Next;
            if (cur.Value.selfPage == page)
            {
                cur.Value.Hide();
                AddRecyleEnforce(cur.Value);
                RemoveOpenedQueue(cur.Value);
                enforceWindowList.Remove(cur);
            }

            cur = next;
        } while (cur != null);
    }

    public void CloseAllEnforceWindow()
    {
        var enumer = enforceWindowList.GetEnumerator();
        while (enumer.MoveNext())
        {
            enumer.Current.Hide();
            AddRecyleEnforce(enumer.Current);
            RemoveOpenedQueue(enumer.Current);
        }

        enforceWindowList.Clear();
    }

    public void ClearAllEnforceWindow()
    {
        var enumer = enforceWindowList.GetEnumerator();
        while (enumer.MoveNext())
        {
            RemoveOpenedQueue(enumer.Current);
            enumer.Current.Hide();
        }

        enforceWindowList.Clear();
    }

    public void AddRecyleEnforce(EnforceWindow enforce)
    {
        recycleEnforceList.AddLast(enforce);
        if (recycleEnforceList.Count > MAX_ENFORCE_POOL)
        {
            recycleEnforceList.First.Value.Dispose();
            recycleEnforceList.RemoveFirst();
        }
    }

    public EnforceWindow PopRecyleEnforce(EnforceWindowPage ewp)
    {
        var enumer = recycleEnforceList.GetEnumerator();
        while (enumer.MoveNext())
        {
            if (enumer.Current.selfPage == ewp)
            {
                var enforce = enumer.Current;
                recycleEnforceList.Remove(enforce);
                return enforce;
            }
        }

        return null;
    }

    public void ClearRecyleEnforce()
    {
        var enumer = recycleEnforceList.GetEnumerator();
        while (enumer.MoveNext())
        {
            enumer.Current.Dispose();
        }

        recycleEnforceList.Clear();
    }


    /////////////////////////////////////已打开窗口管理//////////////////////////////////////////////////
    /// <summary>
    /// 加入打开窗口队列
    /// </summary>
    /// <param name="window"></param>
    void PushOpenedQueue(SmartResWindow window)
    {
        // 新窗口是全屏，把前面的都隐藏
        if (window.IsOpaqueFullScreen())
        {
            var last = openedWindowQueue.Last;
            while (last != null)
            {
                if (last.Value.isShowing)
                {
                    last.Value.HideImmediately();
                    last = last.Previous;
                }
                else
                    break;
            }

            // 最前的窗口是pageWindow
            if (last == null && curPageWindow != null) curPageWindow.HideImmediately();
        }

        openedWindowQueue.AddLast(window);
    }

    /// <summary>
    /// 从打开窗口队列中移除
    /// </summary>
    /// <param name="window"></param>
    void RemoveOpenedQueue(SmartResWindow window)
    {
        var cur = openedWindowQueue.FindLast(window);
        if (cur != null)
        {
            if (cur.Value.IsOpaqueFullScreen())
            {
                var pre = cur.Previous;
                while (pre != null)
                {
                    if (pre == null || pre.Value == null)
                    {
                        continue;
                    }

                    if (pre.Value.displayObject.gameObject != null)
                    {
                        pre.Value.ShowAgain();
                    }

                    if (pre.Value.IsOpaqueFullScreen())
                    {
                        break;
                    }

                    pre = pre.Previous;
                }

                if (pre == null && curPageWindow != null)
                {
                    // 最前的全屏窗口是pageWindow
                    curPageWindow.ShowAgain();
                }
            }

            openedWindowQueue.Remove(cur);
        }
    }

    void ClearOpenedQueue()
    {
#if UNITY_EDITOR
        if (openedWindowQueue.Count != 0)
        {
            var enumer = openedWindowQueue.GetEnumerator();
            while (enumer.MoveNext())
            {
                Debug.LogErrorFormat("[隐患]未通过正常流程关闭", enumer.Current.GetType());
            }
        }
#endif
        openedWindowQueue.Clear();
    }

    ////////////////////////////////////////////////////////////////////////////////////////

/*    public void ShowPopWarningByIndex(string key)
    {
        ShowPopWarning(Language.GetContent(key));
    }

    public void ShowPopWarning(string warning)
    {
        OpenEnforceWindow(EnforceWindowPage.WarningWindow, new WarningWindowArgs(warning));
    }*/


    public bool InScreen(Vector2 pos)
    {
        return pos.x >= 0 && pos.x <= GRoot.inst.width && pos.y >= 0 && pos.y < GRoot.inst.height;
    }

    public bool IsEnforceWindowOpen(EnforceWindowPage window)
    {
        var iter = enforceWindowList.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.selfPage == window)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 粒子特效缩放
    /// </summary>
    static float _particleScale;

    public static float GetParticleScale()
    {
        if (_particleScale != 0) return _particleScale;

        float ratio = GRoot.inst.width / GRoot.inst.height;
        if (ratio < 16 / 9f)
            _particleScale = 720 / GRoot.inst.height;
        else
            _particleScale = 1;
        return _particleScale;
    }

    public bool enableInput
    {
        get { return GRoot.inst.touchable; }
        set { GRoot.inst.touchable = value; }
    }
}