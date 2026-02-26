using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YooAsset.Editor
{
    [DisplayName("打包FairyGUI描述和资源文件")]
    public class PackFairyGUI : IPackRule
    {
        PackRuleResult IPackRule.GetPackRuleResult(PackRuleData data)
        {
            if (data.AssetPath.EndsWith(".bytes"))
            {
                string bundleName = PathUtility.RemoveExtension(data.AssetPath);
                PackRuleResult result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
                return result;
            }
            else
            {
                string bundleName = PathUtility.RemoveExtension(data.AssetPath);
                int index = bundleName.LastIndexOf('_');
                if (index != -1)
                {
                    bundleName = string.Format("{0}_atlas",  bundleName.Remove(index));
                }
                Debug.LogError("bundleName:" + bundleName);
                PackRuleResult result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
                return result;
            }

        }
    }
}
