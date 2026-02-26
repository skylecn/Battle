using UnityEngine;

public class VFXInstance : MonoBehaviour
{
    ParticleSystem[] psList;
    TrailRenderer[] trails;

    bool isPlaying;
    float elapsed;
    float maxLifeTime = -1; // -1 means infinite

    void Awake()
    {
        psList = GetComponentsInChildren<ParticleSystem>(true);
        trails = GetComponentsInChildren<TrailRenderer>(true);
    }

    private void ResetState()
    {
        foreach (var ps in psList)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Clear(true);
        }

        foreach (var t in trails)
            t.Clear();
    }

    /// <summary>
    /// Play the VFX
    /// </summary>
    /// <param name="lifeTime">-1 means infinite</param>
    public void Play(float lifeTime)
    {
        ResetState();

        gameObject.SetActive(true);

        foreach (var ps in psList)
            ps.Play(true);

        elapsed = 0;
        maxLifeTime = lifeTime;
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying) return;

        if(maxLifeTime == -1) return;

        elapsed += Time.deltaTime;

        if (elapsed >= maxLifeTime)
        {
            Stop();
            // recycle
            VFXManager.instance.RemoveEffect(this);
        }
    }

    public void Stop()
    {
        isPlaying = false;
        gameObject.SetActive(false);
    }
}
