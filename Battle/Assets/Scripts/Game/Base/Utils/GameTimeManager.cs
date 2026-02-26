using System;
using UnityEngine;
/// <summary>
/// count game time 
/// </summary>
public class GameTimeManager
{
    public static GameTimeManager instance { get { return Singleton.instance;  } }
    private class Singleton
    {
        static Singleton() { }
        internal static readonly GameTimeManager instance = new GameTimeManager();
    }
    private readonly DateTime mOriTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    /// <summary>
    ///  call time
    /// </summary>
    public long callTime;

    private long _startTime;
    private long _runTime;
    private long _endTime;

    private bool logStart = false;
    public void Start()
    {
        DateTime time = DateTime.Now;
        TimeSpan ts = time - mOriTime;
        _startTime = (long)ts.TotalMilliseconds;     
    }
    private void LogStartTime()
    {
        if (!logStart)
        {
            OutputLogger.Log("[Count Game Time] Start Time:" + _startTime + "ms", OutputLogger.LoggingTarget.Performance);
            logStart = true;
        }
    }
    public void Record(string flag = "")
    {
        LogStartTime();
        DateTime time = DateTime.Now;
        TimeSpan ts = time - mOriTime;
        long currentTime = (long)ts.TotalMilliseconds;
        _runTime = currentTime - _startTime;
        if (string.IsNullOrEmpty(flag))
        {
            OutputLogger.Log("[Count Game Time] Current Run Time:" + _runTime + "ms", OutputLogger.LoggingTarget.Performance);
        }
        else
        {
            OutputLogger.Log("[Count Game Time] Current Run Time:" + _runTime + "ms -- " + flag, OutputLogger.LoggingTarget.Performance);
        }
    }

    public void End()
    {
        LogStartTime();
        DateTime time = DateTime.Now;
        TimeSpan ts = time - mOriTime;
        _endTime = (long)ts.TotalMilliseconds;
        _runTime = _endTime - _startTime;
        OutputLogger.Log("[Count Game Time] Run Total Time:" + _runTime + "ms", OutputLogger.LoggingTarget.Performance);
        OutputLogger.Log("[Count Game Time] End Time:" + _endTime + "ms", OutputLogger.LoggingTarget.Performance);
    }

    /// <summary>
    /// call start - count time
    /// </summary>
    public void CallStart()
    {
        DateTime time = DateTime.Now;
        TimeSpan ts = time - mOriTime;
        long currentTime = (long)ts.TotalMilliseconds;
        _runTime = currentTime - _startTime;
    }

    /// <summary>
    /// call end -- count time ,result : callTime
    /// </summary>
    public void CallEnd()
    {
        DateTime time = DateTime.Now;
        TimeSpan ts = time - mOriTime;
        long currentTime = (long)ts.TotalMilliseconds;
        long lastTime = _runTime;
        _runTime = currentTime - _startTime;
        callTime = _runTime - lastTime;
    }

    private long[] minTime = new long[2] { 1000,1000};
    private long[] maxTime = new long[2] { 0, 0 };
    private long[] totalTime = new long[2] { 0, 0 };
    private long[] averageTime = new long[2] { 0, 0 };
    private long[] recordCounts = new long[2] { 0, 0 };
    public void RecordUpdate(int index)
    {
        if(callTime == 0)
        {
            return;
        }
        if(minTime[index] > callTime)
        {
            minTime[index] = callTime;
        }

        if (maxTime[index] < callTime)
        {
            maxTime[index] = callTime;
        }

        totalTime[index] += callTime;
        recordCounts[index]++;
        averageTime[index] = totalTime[index] / recordCounts[index];
        if (index == 1)
        {
            if (recordCounts[index] % 1000 == 0)
            {
                LogUpdate();
            }
        }
    }

    public void LogUpdate()
    {
        for (int i = 0; i < recordCounts.Length; i++)
        {
            if (i == 0)
            {
                OutputLogger.Log("Record GameMain Update Time! minTime:" + minTime[i] + ", maxTime:" + maxTime[i] + ", averageTime:" + averageTime[i], OutputLogger.LoggingTarget.Performance);
            }
            else if (i == 1)
            {
                OutputLogger.Log("Record UpdateFacade Update Time! minTime:" + minTime[i] + ", maxTime:" + maxTime[i] + ", averageTime:" + averageTime[i], OutputLogger.LoggingTarget.Performance);
            }
        }
    }
}