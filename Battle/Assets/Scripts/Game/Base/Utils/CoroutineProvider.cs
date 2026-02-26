using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CoroutineProvider : MonoBehaviour
{
    private static CoroutineProvider instance = null;
    public static CoroutineProvider Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject go = new GameObject("CoroutineProvider");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<CoroutineProvider>();
            }
            return instance;
        }
    }
}

