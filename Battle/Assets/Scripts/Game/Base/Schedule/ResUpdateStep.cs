using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using YooAsset;
using UnityEditor;

public class ResUpdateStep : IGameStep
{
   
    public override void Init(object args)
    {
        base.Init(args);
        
        CoroutineProvider.Instance.StartCoroutine(InitResPackage());
    }

    IEnumerator InitResPackage()
    {
        // ��ʼ����Դϵͳ
        YooAssets.Initialize();

        // ������Դ������
        var package = YooAssets.TryGetPackage("DefaultPackage");
        if (package == null)
            package = YooAssets.CreatePackage("DefaultPackage");

        InitializationOperation initializationOperation = null;
#if UNITY_EDITOR
        var simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), "DefaultPackage");
        var createParameters = new EditorSimulateModeParameters();
        createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateBuildResult);
        initializationOperation = package.InitializeAsync(createParameters);
        yield return initializationOperation;
#else


        var createParameters = new OfflinePlayModeParameters();
        createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        initializationOperation = package.InitializeAsync(createParameters);

        yield return initializationOperation;
#endif

        var operationVersion = package.RequestPackageVersionAsync();
        yield return operationVersion;

        var operationManifest = package.UpdatePackageManifestAsync(operationVersion.PackageVersion);
        yield return operationManifest;

        // ����Ĭ�ϵ���Դ��
        var gamePackage = YooAssets.GetPackage("DefaultPackage");
        YooAssets.SetDefaultPackage(gamePackage);

        StartLoadMeta();
    }
    
    void StartLoadMeta()
    {
        CfgData.Load();      
        OnLoadFinish();
    }

    void OnLoadFinish()
    {
        UIManager.instance.Init();

        GameStepManager.instance.GotoStep(StepDefine.Home, null);
    }
}
