using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class MainWindow : PageWindow
{
    public override WindowPage selfPage
    {
        get
        {
            return WindowPage.MainWindow;
            
        }
    }

    public override void InitData(object args)
    {
        base.InitData(args);
        
        AddPackage("MainWindow");
        
        GObject obj = UIPackage.CreateObject("MainWindow", "MainWindow");
        contentPane = obj.asCom;
    }
}
