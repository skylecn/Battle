using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGameStep
{
    public virtual void Init(object args)
    {
        RegisterEvent();
    }

    public virtual void FixedUpdate()
    {
        
    }
    
    public virtual void Update()
    {
        
    }

    public virtual void LateUpdate()
    {
        
    }
    
    public virtual void Release()
    {
        UnRegisterEvent();
    }

    public virtual void RegisterEvent()
    {
        
    }

    public virtual void UnRegisterEvent()
    {
        
    }
}
