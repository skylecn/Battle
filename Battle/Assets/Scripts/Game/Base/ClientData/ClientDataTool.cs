using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ClientDataTool
{
    public static ClientDataTool Instance { get { return Nested.instance; } }
    private class Nested
    {
        static Nested()
        {
        }

        internal static readonly ClientDataTool instance = new ClientDataTool();
    }

    private ClientDataTool()
    {
    }

    public ClientData data { get; private set; }
    string fileName;

    public void LoadData()
    {
        fileName = $"{Application.persistentDataPath}/clientData.data";
        // 加载本地数据
        if (File.Exists(fileName))
        {
            try
            {
                StreamReader streamReader = File.OpenText(fileName);
                string file = streamReader.ReadToEnd();
                streamReader.Close();
                data = JsonMapper.ToObject<ClientData>(file);
            }
            catch (JsonException e)
            {
                // 解码失败，删除本地文件
                UnityEngine.Debug.LogError("本地存储数据解析失败，重置为默认");
                File.Delete(fileName);
                data = new ClientData();
                data.CreateDefault();
            }

        }
        else
        {
            data = new ClientData();
            data.CreateDefault();
        }
    }

    public void SaveData()
    {
        string txt = JsonMapper.ToJson(data);
        StreamWriter streamWriter = File.CreateText(fileName);
        streamWriter.Write(txt);
        streamWriter.Close();
    }
}

public class ClientData
{
    public void CreateDefault()
    {

    }
}
