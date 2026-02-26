using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using YooAsset;
using static UnityEngine.Rendering.ReloadAttribute;

public partial class UIManager
{
    public static string UI_PATH = "Assets/GameRes/UI/{0}";
    
    Dictionary<string, int> packageCount = new Dictionary<string, int>();
    Dictionary<string, List<AssetHandle>> packHandles = new Dictionary<string, List<AssetHandle>>();
    public UITextureManager textureManager = new UITextureManager();

    // 加载包
    public void AddPackage(string packName)
    {
        if (packageCount.ContainsKey(packName))
        {
            packageCount[packName]++;
        }
        else
        {
            //int index = packName.IndexOf('/');
            //string packageName = packName.Substring(index + 1);
            //string packetType = packName.Substring(0, index);
            //string realPath = LocalizationPath.GetUIResPath(packetType, packageName);

            //if (UIPackage.GetByName(packageName) == null)
            //{
            //    // 描述
            //    var desAb = ResourceLoader.Instance.GetDirectlyBundle(realPath);
            //    // 图集
            //    var resAb = ResourceLoader.Instance.GetDirectlyBundle(string.Format("{0}atlas", realPath));

            //    if (desAb != null && resAb != null)
            //    {
            //        UIPackage.AddPackage(desAb, resAb);
            //    }
            //    else
            //    {
            //        UIPackage.AddPackage(realPath);
            //    }
            //}

            if (UIPackage.GetByName(packName) == null)
                UIPackage.AddPackage(packName, YooAssetsLoadResource);

            packageCount[packName] = 1;
        }

    }

    object YooAssetsLoadResource(string name, string extension, System.Type type, out DestroyMethod destroyMethod)
    {
        var index = name.LastIndexOf('_');

        destroyMethod = DestroyMethod.None; //注意：这里一定要设置为None
        string location = string.Format(UI_PATH, $"{name}{extension}");
        if (!YooAssets.CheckLocationValid(location)) return null;

        var handle = YooAssets.LoadAssetSync(location, type);

        var packName = name.Substring(0, index);
        if (packHandles.ContainsKey(packName))
        {
            packHandles[packName].Add(handle);
        }
        else
        {
            packHandles.Add(packName, new List<AssetHandle>() { handle });
        }
        return handle.AssetObject;
    }

    public void RemovePackage(string packName)
    {
        if (packageCount.ContainsKey(packName))
        {
            packageCount[packName]--;
            if (packageCount[packName] == 0)
            {
                UIPackage.RemovePackage(packName);
                packageCount.Remove(packName);
                foreach (var handle in packHandles[packName])
                {
                    handle.Release();
                }
                packHandles.Remove(packName);
            }
        }
    }

    public void RemoveUnusedPackage()
    {
        var list = new List<string>();
        var enumer = packageCount.GetEnumerator();
        while (enumer.MoveNext())
        {
            if (enumer.Current.Value <= 0)
            {
                string packageName = enumer.Current.Key;
                UIPackage.RemovePackage(packageName);
                list.Add(enumer.Current.Key);
                foreach (var handle in packHandles[packageName])
                {
                    handle.Release();
                }
                packHandles.Remove(packageName);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            packageCount.Remove(list[i]);
        }

    }

    public void DebugPackageCount()
    {
        var enumer = packageCount.GetEnumerator();
        while (enumer.MoveNext())
        {
            var debugWarning = string.Format("UIPackage:{0}占有数{1}", enumer.Current.Key, enumer.Current.Value);
            if (enumer.Current.Value > 1)
                Debug.LogError(debugWarning);
            else
                Debug.Log(debugWarning);
        }
    }

    void UnloadAllTexture()
    {
        textureManager.UnloadAll();
    }
    public bool IsExitPackage(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            int count = 0;
            if (packageCount.TryGetValue(name, out count))
            {
                return count > 0 ? true : false;
            }
        }
        return false;
    }
}
