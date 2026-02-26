using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using static Unity.VisualScripting.Member;

public class AudioRoutine : vp_Timer.Handle
{
    public static int index;

    public AudioSource audioSource;
    public GameObject sourceObject
    {
        get;
        private set;
    }

    AssetHandle assetHandle;

    public float length
    {
        get
        {
            return audioSource.clip != null ? audioSource.clip.length : 0f;
        }
    }

    public AudioRoutine(Transform parent)
    {
        sourceObject = new GameObject("AudioSource" + index++);
        sourceObject.transform.parent = parent;
        sourceObject.transform.localPosition = Vector3.zero;
        audioSource = sourceObject.AddComponent<AudioSource>();
        audioSource.loop = false;
    }

    public void SetParent(Transform parent)
    {
        sourceObject.transform.parent = parent;
    }

    public void Play(string audioPath, float volume = 1f, bool loop = false)
    {
        audioSource.volume = volume;
        audioSource.loop = loop;
        assetHandle = YooAssets.LoadAssetAsync<AudioClip>(audioPath);
        assetHandle.Completed += OnLoadClipCompleted;
    }

    private void OnLoadClipCompleted(AssetHandle request)
    {
        AudioClip clip = request.AssetObject as AudioClip;
        audioSource.clip = clip;
        audioSource.Play();
        if (!audioSource.loop)
        {
            AudioManager.instance.SetRecyleTimer(this);
        }
    }

    public void Stop()
    {
        this.Paused = true;
        audioSource.Stop();
    }

    public void Recyle()
    {
        audioSource.clip = null;
        if(assetHandle!=null)
            assetHandle.Release();

        assetHandle = null;
    }
}
