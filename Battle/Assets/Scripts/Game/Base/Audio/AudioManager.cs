using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using YooAsset;
using Object = UnityEngine.Object;  


/// <summary>
/// 音频管理器
/// </summary>
public class AudioManager
{
    #region singleton
    public static AudioManager instance { get { return Singleton._instance; } }
    private static class Singleton
    {
        public static AudioManager _instance = new AudioManager();
        static Singleton()
        {
        }
    }

    private AudioManager() { }
    #endregion

    #region volume
    public float MusicVolume { get; private set; }
    public float SoundVolume { get; private set; }
    public bool SoundEnable { get; private set; }
    public bool MusicEnable { get; private set; }

    const string SOUND_VOLUME = "SoundVolume";
    const string MUSIC_VOLUME = "MusicVolume";
    const string SOUND_ENABLE = "SoundEnable";
    const string MUSIC_ENABLE = "MusicEnable";
    #endregion volume

    Transform parent;
    Transform audioPool;

    #region BGM
    string BGMName;
    public AudioSource BGMSource { get; private set; }
    AssetHandle BGMHandle;
    bool BGMIsLoop;

    bool isFadeIn;
    bool isFadeOut;
    Tween fadeOutTween;
    Tween fadeInTween;
    Tween stopMusicTween;
    float fadeInVolume = 1;
    float fadeOutVolume = 0;
    float fadeDuration = 1;
    #endregion

    #region sound
    HashSet<AudioRoutine> playingSoundSet = new HashSet<AudioRoutine>();
    Queue<AudioRoutine> cacheAudioSourceList = new Queue<AudioRoutine>(100);
    #endregion

    public void Init()
    {
        if (PlayerPrefs.HasKey(SOUND_ENABLE))
        {
            SoundEnable = PlayerPrefs.GetInt(SOUND_ENABLE) == 1;
        }

        if (PlayerPrefs.HasKey(MUSIC_ENABLE))
        {
            MusicEnable = PlayerPrefs.GetInt(MUSIC_ENABLE) == 1;
        }

        if (PlayerPrefs.HasKey(SOUND_VOLUME))
        {
            SoundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME);
        }

        if (PlayerPrefs.HasKey(MUSIC_VOLUME))
        {
            MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        }

        parent = new GameObject("AudioManager").transform;
        GameObject.DontDestroyOnLoad(parent.gameObject);

        audioPool = new GameObject("AudioPool").transform;
        GameObject.DontDestroyOnLoad(audioPool.gameObject);

        BGMSource = new GameObject("BGMSource", typeof(AudioSource)).GetComponent<AudioSource>();
        BGMSource.transform.SetParent(parent);
    }

    #region volume

    public void SetSoundVolume(float volume)
    {
        SoundVolume = volume;
        PlayerPrefs.SetFloat(SOUND_VOLUME, volume);

        foreach (var audioRoutine in playingSoundSet)
        {
            audioRoutine.audioSource.volume = SoundVolume;
        }
    }
    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME, volume);
        BGMSource.volume = MusicVolume;
    }

    public void SetSoundEnable(bool enable, bool save = true)
    {
        SoundEnable = enable;
        if (save)
        {
            PlayerPrefs.SetInt(SOUND_ENABLE, enable ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void SetMusicEnable(bool enable, bool save = true)
    {
        MusicEnable = enable;
        if (save)
        {
            PlayerPrefs.SetInt(MUSIC_ENABLE, enable ? 1 : 0);
            PlayerPrefs.Save();
        }
        if (enable)
        {
            if (BGMSource != null && BGMSource.clip != null)
            {
                BGMSource.Play();
            }
        }
        else
        {
            if (BGMSource != null && BGMSource.isPlaying)
            {
                BGMSource.Pause();
            }
        }
    }

    #endregion

    #region BGM

    public void PlayBGM(string audioName, bool isLoop, float volume = 1f, bool isFadeIn = true, bool isFadeOut = true)
    {
        if (!MusicEnable) return;

        if (BGMSource.isPlaying && BGMName == audioName)
        {
            return;
        }

        if (stopMusicTween != null)
        {
            stopMusicTween.Kill();
        }
        if (fadeOutTween != null)
        {
            fadeOutTween.Kill();
        }
        if (fadeInTween != null)
        {
            fadeInTween.Kill();
        }

        BGMName = audioName;
        BGMIsLoop = isLoop;
        fadeInVolume = MusicVolume * volume;

        this.isFadeIn = isFadeIn;
        this.isFadeOut = isFadeOut;

        PlayBGMInner();
    }

    void PlayBGMInner()
    {
        if (BGMSource.isPlaying && isFadeOut)
        {
            PlayFadeOut();
        }
        else
        {
            PlayFadeIn();
        }
    }

    void PlayFadeOut()
    {
        fadeOutTween = DOTween.To(() => BGMSource.volume, x => BGMSource.volume = x, fadeOutVolume, fadeDuration).OnComplete(() => PlayFadeIn());
    }

    void PlayFadeIn()
    {
        if (BGMHandle != null) BGMHandle.Release();

        BGMHandle = YooAssets.LoadAssetAsync<AudioClip>(BGMName);
        BGMHandle.Completed += LoadBgmCompleted;
    }

    void LoadBgmCompleted(AssetHandle request)
    {
        BGMSource.clip = request.AssetObject as AudioClip;
        BGMSource.loop = BGMIsLoop;
        BGMSource.Play();
        if (isFadeIn)
        {
            fadeInTween = DOTween.To(() => BGMSource.volume, x => BGMSource.volume = x, fadeInVolume, fadeDuration);
        }
        else
        {
            BGMSource.volume = fadeInVolume;
        }
    }

    public void StopBGM(bool isFadeOut)
    {
        if (isFadeOut)
            stopMusicTween = DOTween.To(() => BGMSource.volume, x => BGMSource.volume = x, 0, fadeDuration).OnComplete(() => BGMSource.Stop());
        else
            BGMSource.Stop();
    }

    #endregion

    #region 音效
    public AudioRoutine CreateOrRetrive(Transform parent)
    {
        AudioRoutine data = null;
        if (cacheAudioSourceList.Count > 0)
        {
            data = cacheAudioSourceList.Dequeue();
        }
        else
        {
            data = new AudioRoutine(parent);
        }
        return data;
    }

    public void Recyle(AudioRoutine data)
    {
        if (!cacheAudioSourceList.Contains(data))
        {
            cacheAudioSourceList.Enqueue(data);
        }
    }

    public void PlayAudio(string audioName, float volume = 1, bool loop = false)
    {
        var audioRoutine = CreateOrRetrive(parent);
        audioRoutine.Play(audioName, volume * SoundVolume, loop);

        playingSoundSet.Add(audioRoutine);
    }

    public void StopAudio()
    {
        foreach (var audioRoutine in playingSoundSet)
        {
            audioRoutine.Stop();
            audioRoutine.Recyle();
            Recyle(audioRoutine);
        }
        playingSoundSet.Clear();
    }

    public void SetRecyleTimer(AudioRoutine audioRoutine)
    {
        float time = audioRoutine.length;
        if (time == 0)
        {
            time = 2f;
        }
        vp_Timer.In(time, RecyleAudio, audioRoutine, audioRoutine);
    }

    void RecyleAudio(object param)
    {
        var audioRoutine = param as AudioRoutine;
        if (playingSoundSet.Contains(audioRoutine))
            playingSoundSet.Remove(audioRoutine);

        audioRoutine.Recyle();
        Recyle(audioRoutine);
    }


    #endregion

    public void ClearAll()
    {
        foreach (var audioRoutine in playingSoundSet)
        {
            audioRoutine.Stop();
            audioRoutine.Recyle();
            Recyle(audioRoutine);
        }
        playingSoundSet.Clear();

       
        BGMSource.Stop();
        BGMSource.clip = null;
        if (BGMHandle != null){
            BGMHandle.Release();
            BGMHandle = null;
        }
    }
}