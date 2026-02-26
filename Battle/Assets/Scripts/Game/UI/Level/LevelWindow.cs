using FairyGUI;
using UnityEngine;

public class LevelWindow : EnforceWindow
{
    public override EnforceWindowPage selfPage => EnforceWindowPage.LevelWindow;

    GList levelList;

    protected override void OnInit()
    {
        AddPackage("LevelUI");

        GObject obj = UIPackage.CreateObject("LevelUI", "LevelWindow");
        contentPane = obj.asCom;

/*        levelList = contentPane.GetChild("levelList").asList;
        levelList.SetVirtual();
        levelList.onClickItem.Add(OnLevelListItemClick);
        levelList.itemRenderer = OnRenderItem;
        levelList.numItems = DataManager.levelDataList.dataList.Count;*/

        for (int i = 1; i <= 11; i++)
        {
            var com = contentPane.GetChild(string.Format("item{0}", i)).asCom;
            com.onClick.Add(OnLevelListItemClick);
            com.data = i;
            com.GetChild("title").asTextField.text = i.ToString();
        }
    }

    override public void SetData(object args)
    {
    }

    /*void OnRenderItem(int index, GObject obj)
    {
        obj.data = index;
        obj.asCom.GetChild("title").asTextField.text = CfgData.levelDataList.dataList[index].Name;
    }*/

    void OnLevelListItemClick(EventContext context)
    {
        /*int index = (int)((GComponent)context.sender).data;
        if (index > CfgData.levelDataList.dataList.Count)
            return;
        var levelData = CfgData.levelDataList.dataList[index-1];
        if (levelData == null)
            return;*/

        //HomeSceneProcessor.instance.InitMapObj(levelData);
        //UIManager.instance.CloseEnforceWindow(this);
    }
}