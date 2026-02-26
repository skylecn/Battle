using System;
using System.Collections.Generic;
using UnityEngine;

public class ClassObjectPool
{

    public static ClassObjectPool instance { get { return Nested.instance; } }
    private class Nested
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Nested()
        {
        }

        internal static readonly ClassObjectPool instance = new ClassObjectPool();
    }

#if UNITY_EDITOR
    /// <summary>
    /// �ڼ��������ʾ����Ϣ
    /// </summary>
    public Dictionary<Type, int> InspectorDic = new Dictionary<Type, int>();
#endif

    /// <summary>
    /// �������ֵ�
    /// </summary>
    private Dictionary<int, Queue<object>> m_ClassObjectPoolDic;


    private ClassObjectPool()
    {
        m_ClassObjectPoolDic = new Dictionary<int, Queue<object>>();

    }

    #region Dequeue ȡ��һ������
    /// <summary>
    /// ȡ��һ������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Pop<T>() where T : class, new()
    {
        lock (m_ClassObjectPoolDic)
        {
            //���ҵ������Ĺ�ϣ
            int key = typeof(T).GetHashCode();

            Queue<object> queue = null;
            m_ClassObjectPoolDic.TryGetValue(key, out queue);

            if (queue == null)
            {
                queue = new Queue<object>();
                m_ClassObjectPoolDic[key] = queue;
            }

            //��ʼ��ȡ����
            if (queue.Count > 0)
            {
                object obj = queue.Dequeue();
#if UNITY_EDITOR
                Type t = obj.GetType();
                if (InspectorDic.ContainsKey(t))
                {
                    InspectorDic[t]--;
                }
                else
                {
                    InspectorDic[t] = 0;
                }
#endif

                return (T)obj;
            }
            else
            {
                return new T();
            }
        }
    }
    #endregion

    #region Enqueue ����س�
    /// <summary>
    /// ����س�
    /// </summary>
    /// <param name="obj">���յĶ��󣬻���ǰ�Լ�Clear</param>
    public void Recyle(object obj)
    {
        lock (m_ClassObjectPoolDic)
        {
            int key = obj.GetType().GetHashCode();
            //Debug.Log("���� " + key + "�س���");

            Queue<object> queue = null;
            m_ClassObjectPoolDic.TryGetValue(key, out queue);

#if UNITY_EDITOR
            Type t = obj.GetType();
            if (InspectorDic.ContainsKey(t))
            {
                InspectorDic[t]++;
            }
            else
            {
                InspectorDic[t] = 1;
            }
#endif

            if (queue != null)
            {
                queue.Enqueue(obj);
            }
        }
    }
    #endregion

    public void Release<T>() where T : class, new()
    {
        lock (m_ClassObjectPoolDic)
        {
            int key = typeof(T).GetHashCode();
            if (m_ClassObjectPoolDic.ContainsKey(key))
            {
                m_ClassObjectPoolDic.Remove(key);
            }

#if UNITY_EDITOR
            InspectorDic.Remove(typeof(T));
#endif
        }
    }

    /// <summary>
    /// �ͷ�������
    /// </summary>
    public void Release()
    {
        lock (m_ClassObjectPoolDic)
        {
            m_ClassObjectPoolDic.Clear();

#if UNITY_EDITOR
            InspectorDic.Clear();
#endif
        }
    }

}
