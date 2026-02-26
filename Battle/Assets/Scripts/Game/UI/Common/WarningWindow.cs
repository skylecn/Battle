using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public class WarningWindowArgs : EventArgs
{
    public string tips;

    public WarningWindowArgs(string tips)
    {
        this.tips = tips;
    }
}

public class WarningWindow : EnforceWindow
{
    public override EnforceWindowPage selfPage => EnforceWindowPage.WarningWindow;

    Transition hintTwenn;
    GTextField hintLabel;

    protected override void OnInit()
    {
        AddPackage("FGUI/CommonHint");

        GObject obj = UIPackage.CreateObject("CommonHint", "ComHint");
        contentPane = obj.asCom;

        hintTwenn = contentPane.GetTransition("HintTwenn");
        var com = contentPane.GetChild("GroupTween").asCom;
        hintLabel = com.GetChild("HintLabel").asTextField;
    }

    public override void SetData(object args)
    {
        WarningWindowArgs arg = args as WarningWindowArgs;
        hintLabel.text = arg.tips;

        hintTwenn.Play(OnPlayCallback);
    }

    void OnPlayCallback()
    {
        UIManager.instance.CloseEnforceWindow(this);
    }


}
