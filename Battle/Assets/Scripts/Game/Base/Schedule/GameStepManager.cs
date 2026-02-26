using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStepManager
{
    #region Singleton
    private class SingletonNested
    {
        static SingletonNested()
        {
        }

        internal static readonly GameStepManager instance = new GameStepManager();
    }

    public static GameStepManager instance { get { return SingletonNested.instance; } }
    #endregion

    public IGameStep curStep
    {
        get;
        private set;
    }

    public void Init()
    {
        curStep = new ResUpdateStep();
        curStep.Init(null);
    }

    public void GotoStep(StepDefine stepDefine, object args)
    {
        curStep.Release();
        switch (stepDefine)
        {
            case StepDefine.ResUpdate:
                curStep = new ResUpdateStep();
                break;
            case StepDefine.Login:
                curStep = new LoginStep();
                break;
            case StepDefine.Home:
                curStep = new HomeStep();
                break;
        }
        curStep.Init(args);
    }

    public void FixedUpdate()
    {
        curStep.FixedUpdate();
    }
    
    public void Update()
    {
        curStep.Update();
    }

    public void LateUpdate()
    {
        curStep.LateUpdate();
    }
}
