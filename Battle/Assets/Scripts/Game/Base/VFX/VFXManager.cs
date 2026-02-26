using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Base.Utils;
using UnityEngine;

public class VFXManager
{
    #region Singleton

    private class SingletonNested
    {
        static SingletonNested()
        {
        }

        internal static readonly VFXManager instance = new VFXManager();
    }

    public static VFXManager instance
    {
        get { return SingletonNested.instance; }
    }

    private VFXManager()
    {
        _root = new GameObject("VFXManager");
        GameObject.DontDestroyOnLoad(_root);
    }

    #endregion

    GameObject _root;

    class LivingEffect
    {
        public VFXInstance instance;
        public float duration;
    }

    public enum EffectType
    {
        Bone = 1,
        Scene = 2,
    }

    HashSet<VFXInstance> livingEffects = new HashSet<VFXInstance>();

    private async UniTask<VFXInstance> CreateEffect(string name)
    {
        var go = await InstancePoolManager.Instance.Get(name);
        var inst = go.GetOrAddComponent<VFXInstance>();
        livingEffects.Add(inst);
        return inst;
    }

    public async UniTask<VFXInstance> CreateEffect(string name, Transform parent, Vector3 pos, float time)
    {
        var inst = await CreateEffect(name);
        inst.transform.SetParentAndResetTransform(parent ?? _root.transform);
        inst.transform.localPosition = pos;
        inst.Play(time);
        return inst;
    }

    public async UniTask<VFXInstance> CreateEffect(int id, Transform parent, Func<string, Transform> posfunc)
    {
        var effect = CfgData.GetEffect(id);
        if (effect == null)
        {
            return null;
        }

        float time = effect.Loop ? -1 : effect.Time / 1000f;
        var inst = await CreateEffect(effect.Res);
        switch ((EffectType)effect.EffectType)
        {
            case EffectType.Bone:
            {
                Transform bone = GetBone(effect.ParentNode, parent, posfunc);
                inst.transform.SetParentAndResetTransformRS(bone);
                inst.transform.localPosition = effect.PosOffset.ToVec3();
                break;
            }
            case EffectType.Scene:
            {
                Transform bone = GetBone(effect.ParentNode, parent, posfunc);
                inst.transform.SetParentAndResetTransformRS(_root.transform);
                if (!bone)
                {
                    inst.transform.position = effect.PosOffset.ToVec3();
                }
                else
                {
                    inst.transform.position = bone.position + effect.PosOffset.ToVec3();
                }

                break;
            }
        }

        inst.Play(time);
        return inst;
    }

    private Transform GetBone(string parentName, Transform parent, Func<string, Transform> posfunc)
    {
        return posfunc != null ? posfunc(parentName) :
            string.IsNullOrEmpty(parentName) ? parent : parent?.DeepFindChild(parentName);
    }

    public void RemoveEffect(VFXInstance instance)
    {
        instance.Stop();
        livingEffects.Remove(instance);
        InstancePoolManager.Instance.Recycle(instance.gameObject);
    }

    public void ClearEffects()
    {
        foreach (var le in livingEffects)
        {
            le.Stop();
            InstancePoolManager.Instance.Recycle(le.gameObject);
        }

        livingEffects.Clear();
    }
}