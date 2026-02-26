using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

/// <summary>
/// 打字机效果对象
/// </summary>
public class TypeWriterEffect
{
    private static float DefaultCharsPerSecond = 0.2f;
    
    private float mCharsPerSecond = DefaultCharsPerSecond;//打字时间间隔
    private string mWords;//保存需要显示的文字

    private bool mIsActive = false;
    private float mTimer;//计时器
    private GTextField mPlayText;
    private int mCurrentPos = 0;//当前打字位置
    
    private void Reset () {
        mTimer = 0;
        mIsActive = false;
        mCurrentPos = 0;
        mCharsPerSecond = DefaultCharsPerSecond;
        mPlayText = null;
        mWords = "";
    }

    /// <summary>
    /// 执行打字任务
    /// </summary>
    private void OnStartWriter(){
        if(mIsActive){
            mTimer += Time.deltaTime;
            if(mTimer>=mCharsPerSecond){//判断计时器时间是否到达
                mTimer = 0;
                mCurrentPos++;
                mPlayText.text = mWords.Substring (0,mCurrentPos);//刷新文本显示内容
                if (mCurrentPos >= mWords.Length)
                {
                   Reset();
                }
            }
        }
    }

    public bool IsActive()
    {
        return mIsActive;
    }
    
    // Update is called once per frame
    public void Update () {
        OnStartWriter ();
    }

    /// <summary>
    /// 开始打字效果
    /// </summary>
    /// <param name="textField">打字效果展示的文本框</param>
    /// <param name="charsPerMilliSecond">每个字体的间隔时间(毫秒)</param>
    public bool StartEffect(GTextField textField, int charsPerMilliSecond = 0){
        Reset();
        
        if(textField == null || string.IsNullOrEmpty(textField.text))
            return false;
        
        mIsActive = true;
        
        if (charsPerMilliSecond != 0)
        {
            mCharsPerSecond = charsPerMilliSecond / 1000f;
        }

        mPlayText = textField;
        mWords = mPlayText.text;
        mPlayText.text = "";
        return true;
    }

    /// <summary>
    /// 强制打字效果
    /// </summary>
    public void ForceEndEffect()
    { 
        mPlayText.text = mWords;//刷新文本显示内容
        Reset();
    }
    
}

/// <summary>
/// 打字机效果管理器
/// </summary>
public class TypeWriterEffectManager
{
    private static TypeWriterEffectManager instance;
    private static int InitPoolSize = 2;
    
    private bool bIsInit = false;
    private List<TypeWriterEffect> mExecuteList;
    private Queue<TypeWriterEffect> mObjectPool;
    
    public static TypeWriterEffectManager getInstance()
    {

        if (null == TypeWriterEffectManager.instance)
        {
            instance = new TypeWriterEffectManager();
        }

        return instance;
    }

    public TypeWriterEffectManager()
    {
        mExecuteList = new List<TypeWriterEffect>();
        mObjectPool = new Queue<TypeWriterEffect>();
    }

    public void Start()
    {
        if (bIsInit)
        {
            return;
        }

        for (int i = 0; i < InitPoolSize; i++)
        {
            mObjectPool.Enqueue(new TypeWriterEffect());
        }
        
        bIsInit = true;
    }

    private List<int> ChooseIndex = new List<int>();
    public void Update()
    {
        if (!bIsInit)
        {
            return;
        }

        if (mExecuteList.Count > 0)
        {
            ChooseIndex.Clear();
            for (int i = 0; i < mExecuteList.Count; i++)
            {
                if (!mExecuteList[i].IsActive())
                {
                    ChooseIndex.Add(i);
                    continue;
                }
                mExecuteList[i].Update();
            }
        
            foreach (var index in ChooseIndex)
            {
                mObjectPool.Enqueue(mExecuteList[index]);
                mExecuteList.RemoveAt(index);
            }
        }

    }

    private TypeWriterEffect GetTypeWriterEffect()
    {
        if (!bIsInit)
        {
            return null;
        }
        
        if (mObjectPool.Count > 0)
        {
            return mObjectPool.Dequeue();
        }

        return new TypeWriterEffect();
    }
    
    /// <summary>
    /// 强制结束
    /// </summary>
    /// <param name="twEffect"></param>
    /// <param name="recycle"></param>
    public void HandleTypeWriterForceEffectEnd(TypeWriterEffect twEffect)
    {
        if (!bIsInit)
        {
            return;
        }
        
        if(twEffect == null || !twEffect.IsActive())
            return;
        
        twEffect.ForceEndEffect();
        mObjectPool.Enqueue(twEffect);
        mExecuteList.Remove(twEffect);
    }
    
    /// <summary>
    /// 开启打字效果
    /// </summary>
    /// <param name="textField"></param>
    /// <param name="charsPerMilliSecond"></param>
    /// <returns></returns>
    public TypeWriterEffect HandleTypeWriterEffectBegin(GTextField textField, int charsPerMilliSecond = 0)
    {
        TypeWriterEffect twEffect = null;
        if (!bIsInit)
        {
            return twEffect;
        }

        twEffect = GetTypeWriterEffect();
        
        if(twEffect == null)
            return null;
        
        bool bRes = twEffect.StartEffect(textField, charsPerMilliSecond);

        if (!bRes)
        {
            return null;
        }
        mExecuteList.Add(twEffect);
        
        return twEffect;
    }
    
}