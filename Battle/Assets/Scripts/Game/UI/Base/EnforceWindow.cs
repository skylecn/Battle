using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using DG.Tweening;

public class EnforceWindow : SmartResWindow
{

    public EnforceWindow()
    {
        this.sortingOrder = 1000;
    }

    public virtual EnforceWindowPage selfPage
    {
        get { return EnforceWindowPage.Null; }
    }

    public virtual void SetData(object args)
    {
        
    }

}
