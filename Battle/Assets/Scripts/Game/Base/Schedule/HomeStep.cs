using System.Collections;
using System.Collections.Generic;
using Game.CityBattle;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class HomeStep : IGameStep
{
    bool initFinished = false;

    public override void Init(object args)
    {
        base.Init(args);

        SSceneManager.instance.ChangeMainScene("CityBattle", FinishLoadScene);
    }

    void FinishLoadScene(SceneHandle sceneHandle)
    {
        CityBattleManager.Instance.Init();
        CityBattleManager.Instance.InitMapObj(1);
        //UIManager.instance.OpenEnforceWindow(EnforceWindowPage.LevelWindow);
        initFinished = true;
    }

    public override void Update()
    {
        if (!initFinished)
            return;
        //HomeSceneProcessor.instance.Update();
        //SceneEffectManager.instance.Update(Time.deltaTime);

        CityBattleManager.Instance.Update(Time.deltaTime);
    }

    public override void Release()
    {
        //HomeSceneProcessor.instance.Release();
        CityBattleManager.Instance.Release();
    }
}