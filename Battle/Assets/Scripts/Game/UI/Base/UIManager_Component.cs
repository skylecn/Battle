using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public partial class UIManager
{
    // 注册自定义组件
    void RegisterComponent()
    {
/*
        UIObjectFactory.SetPackageItemExtension("ui://PA_Common/BtnHeroIcon", typeof(HeadComponent));
        UIObjectFactory.SetPackageItemExtension("ui://PA_Common/GroupZiyuan", typeof(ResourceComponent));
        UIObjectFactory.SetPackageItemExtension("ui://BuildingList/BuildingListGroupCell", typeof(BuildingListCell));
        UIObjectFactory.SetPackageItemExtension("ui://Explore/BtnBattleSite", typeof(ExplorationChapterUICell));
        UIObjectFactory.SetPackageItemExtension("ui://ExploreBattle/BtnEvent", typeof(ExplorationEventCell));
        UIObjectFactory.SetPackageItemExtension("ui://Learn/BtnLearn", typeof(KnowledgeNode));
        UIObjectFactory.SetPackageItemExtension("ui://PA_Common/ComCostCell", typeof(ResourceConsume));
        UIObjectFactory.SetPackageItemExtension("ui://MainPage/ResourceTips", typeof(InfoTips));
        UIObjectFactory.SetPackageItemExtension("ui://God/BtnGod", typeof(GodCard));
        UIObjectFactory.SetPackageItemExtension("ui://PA_Common/ComCostCellSmall", typeof(ResourceComponentSmall));
        UIObjectFactory.SetPackageItemExtension("ui://SceneInfo/CollectingNumTween", typeof(ResourceBubbleComponent));
*/

        ////混排icon注册方式
        //SetIconPackageItemExtension("ui://CommonTools/ItemIcon2", typeof(IconItem));
    }

    void SetIconPackageItemExtension(string resource,System.Type type)
    {
        //UIObjectFactory.SetPackageItemExtension(resource, type);
        //IconBase.SetResource(resource,type);
    }

}
