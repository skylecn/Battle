using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class UITexRes
{
    public NTexture nt;
    public Texture2D tex;

    public UITexRes(NTexture nt, Texture2D tex)
    {
        this.nt = nt;
        this.tex = tex;
    }
}

/// <summary>
/// 图片资源管理
/// </summary>
public class UITextureManager {


    Dictionary<string, UITexRes> dictTexture = new Dictionary<string, UITexRes>();

    private UITexRes lastTex;       // 最近加载的TeX

    //////////////////////////////////////////////////////

    /// <summary>
    /// 加载图片
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    NTexture GetTexture(string path)
    {
        UITexRes texRes;
        if(dictTexture.TryGetValue(path, out texRes))
        {
            lastTex = texRes;
            return texRes.nt;
        }

        return null;

        //var tex = ResourceLoader.GetLargeImage(path);
        //if (tex == null) return null;

        //var nt = new NTexture(tex);
        //lastTex = new UITexRes(nt, tex);
        //dictTexture.Add(path, lastTex);
        //return nt;
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public NTexture GetTexture(string path, string name)
    {
        string fileName = string.Format("{0}{1}", path, name);
        return GetTexture(fileName);
    }

    /// <summary>
    /// 释放某路径的图片
    /// </summary>
    /// <param name="path"></param>
    public void UnloadPath(string path)
    {
        var list = new List<string>();
        var enumer = dictTexture.GetEnumerator();
        while (enumer.MoveNext())
        {
            if (enumer.Current.Key.StartsWith(path))
            {
                list.Add(enumer.Current.Key);
                var cur = enumer.Current.Value;
                if (cur == lastTex) lastTex = null;
                cur.nt.Dispose();
                Resources.UnloadAsset(cur.tex);
            }
        }
        var listEnumer = list.GetEnumerator();
        while (listEnumer.MoveNext())
        {
            dictTexture.Remove(listEnumer.Current);
        }
    }

    /// <summary>
    /// 释放所有图片
    /// </summary>
    public void UnloadAll()
    {
        lastTex = null;
        var enumer = dictTexture.GetEnumerator();
        while (enumer.MoveNext())
        {
            var cur = enumer.Current.Value;
            cur.nt.Dispose();
            Resources.UnloadAsset(cur.tex);
        }
        dictTexture.Clear();
    }

    public bool FreeMemory()
    {
        var list = new List<string>();
        var enumer = dictTexture.GetEnumerator();
        while (enumer.MoveNext())
        {
            var cur = enumer.Current.Value;
            if (cur!=lastTex)
            {
                list.Add(enumer.Current.Key);
                cur.nt.Dispose();
                Resources.UnloadAsset(cur.tex);
            }
        }
        var listEnumer = list.GetEnumerator();
        while (listEnumer.MoveNext())
        {
            dictTexture.Remove(listEnumer.Current);
        }

        return list.Count > 0;
    }
}
