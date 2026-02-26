using UnityEngine.SceneManagement;
using YooAsset;	
	
class AssetScene
{
	public SceneHandle handle { get; private set; }
	private System.Action<SceneHandle> finishCallback;
	private System.Action<int> progressCallback;
	private int lastProgressValue = 0;

	/// <summary>
	/// 场景地址
	/// </summary>
	public string Location { private set; get; }

	/// <summary>
	/// 场景加载进度（0-100）
	/// </summary>
	public int Progress
	{
		get
		{
			if (handle == null)
				return 0;
			return (int)(handle.Progress * 100f);
		}
	}

	/// <summary>
	/// 场景是否加载完毕
	/// </summary>
	public bool IsDone
	{
		get
		{
			if (handle == null)
				return false;
			return handle.IsDone;
		}
	}


	public AssetScene(string location)
	{
		Location = location;
	}
	public void Load(bool isAdditive, System.Action<SceneHandle> finishCallback, System.Action<int> progressCallbcak)
	{
		if (handle != null)
			return;

		var sceneMode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
		this.finishCallback = finishCallback;
		progressCallback = progressCallbcak;

		handle =  YooAssets.LoadSceneAsync(Location, sceneMode);
		handle.Completed += Handle_Completed;
	}
	public void UnLoad()
	{
		if (handle != null && handle.IsValid)
		{
			finishCallback = null;
			progressCallback = null;
			lastProgressValue = 0;

			// 异步卸载场景
			handle.UnloadAsync();
			handle = null;
		}
	}
	public void Update()
	{
		if (handle != null)
		{
			if (lastProgressValue != Progress)
			{
				lastProgressValue = Progress;
				progressCallback?.Invoke(lastProgressValue);
			}
		}
	}

	// 资源回调
	private void Handle_Completed(SceneHandle handle)
	{
        finishCallback?.Invoke(this.handle);
	}
}