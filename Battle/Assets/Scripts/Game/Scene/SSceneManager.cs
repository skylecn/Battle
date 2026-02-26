using System;
using System.Collections.Generic;
using YooAsset;

/// <summary>
/// 场景管理器
/// </summary>
public sealed class SSceneManager
{
    #region singleton
    public static SSceneManager instance { get { return Singleton._instance; } }
    private static class Singleton
    {
        public static SSceneManager _instance = new SSceneManager();
        static Singleton()
        {
        }
    }

    private SSceneManager() { }
    #endregion

    private readonly List<AssetScene> additionScenes = new List<AssetScene>();
	private AssetScene mainScene;


	void Update()
	{
		if (mainScene != null)
			mainScene.Update();

		foreach (var addtionScene in additionScenes)
		{
			if (addtionScene != null)
				addtionScene.Update();
		}
	}

	/// <summary>
	/// 切换主场景，之前的主场景以及附加场景将会被卸载
	/// </summary>
	/// <param name="location">场景资源地址</param>
	/// <param name="callback">场景加载完毕的回调</param>
	public void ChangeMainScene(string location, System.Action<SceneHandle> finishCallback = null,
		System.Action<int> progressCallback = null)
	{
		if (mainScene != null && mainScene.IsDone == false)
			DebugLogger.Log($"The current main scene {mainScene.Location} is not loading done.");

		mainScene = new AssetScene(location);
		mainScene.Load(false, finishCallback, progressCallback);
	}

	/// <summary>
	/// 在当前主场景上加载附加场景
	/// </summary>
	/// <param name="location">场景资源地址</param>
	/// <param name="activeOnLoad">加载完成时是否激活附加场景</param>
	/// <param name="callback">场景加载完毕的回调</param>
	public void LoadAdditionScene(string location, System.Action<SceneHandle> finishCallback = null,
		System.Action<int> progressCallback = null)
	{
		AssetScene scene = TryGetAdditionScene(location);
		if (scene != null)
		{
			return;
		}

		AssetScene newScene = new AssetScene(location);
		additionScenes.Add(newScene);
		newScene.Load(true, finishCallback, progressCallback);
	}

	/// <summary>
	/// 卸载当前主场景的附加场景
	/// </summary>
	public void UnLoadAdditionScene(string location)
	{
		for (int i = additionScenes.Count - 1; i >= 0; i--)
		{
			if (additionScenes[i].Location == location)
			{
				additionScenes[i].UnLoad();
				additionScenes.RemoveAt(i);
			}
		}
	}

	/// <summary>
	/// 获取场景当前的加载进度，如果场景不存在返回0
	/// </summary>
	public int GetSceneLoadProgress(string location)
	{
		if (mainScene != null)
		{
			if (mainScene.Location == location)
				return mainScene.Progress;
		}

		AssetScene scene = TryGetAdditionScene(location);
		if (scene != null)
			return scene.Progress;

		return 0;
	}

	/// <summary>
	/// 检测场景是否加载完毕，如果场景不存在返回false
	/// </summary>
	public bool CheckSceneIsDone(string location)
	{
		if (mainScene != null)
		{
			if (mainScene.Location == location)
				return mainScene.IsDone;
		}

		AssetScene scene = TryGetAdditionScene(location);
		if (scene != null)
			return scene.IsDone;

		return false;
	}


	/// <summary>
	/// 尝试获取一个附加场景，如果不存在返回NULL
	/// </summary>
	private AssetScene TryGetAdditionScene(string location)
	{
		for (int i = 0; i < additionScenes.Count; i++)
		{
			if (additionScenes[i].Location == location)
				return additionScenes[i];
		}
		return null;
	}
}