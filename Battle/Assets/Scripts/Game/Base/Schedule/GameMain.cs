using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    
    protected static GameMain m_instance;
    public static GameMain Instance
    {
        get
        {
            return m_instance;
        }
    }
    
    private void Awake()
    {
        m_instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        OutputLogger.Log("Game Start", "GameMain", "Start");
        OutputLogger.Level = OutputLogger.LogLevel.Debug;

        Application.logMessageReceivedThreaded += OnLogMessageReceived;

        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;

        UIManager.InitConfig();

        // 最后走这个
        GameStepManager.instance.Init();
    }

    private void FixedUpdate()
    {
        GameStepManager.instance.FixedUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFacade.instance.Update();
        GameStepManager.instance.Update();
    }

    private void LateUpdate()
    {
        GameStepManager.instance.LateUpdate();
    }

    private void OnDestroy()
    {
        
    }

    private void OnApplicationQuit()
    {
        OutputLogger.CloseAll();
    }

    private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception)
            return;

        OutputLogger.Error("-------------- Unity " + type + " -------------- ");
        OutputLogger.Error("Condition : " + condition);
        OutputLogger.Error("StackTrace : " + stackTrace);
        OutputLogger.Error("------------------------------------------------ ");
    }

    private void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
    {
        OutputLogger.Error("-------------- Exception --------------");
        OutputLogger.Error("Sender : " + sender);
        OutputLogger.Error("Exception Object : " + e.ExceptionObject);
        OutputLogger.Error("Message : " + e.ToString());
        OutputLogger.Error("---------------------------------------");
    }
}
