using System;

public interface ILimitUpdator
{
    bool active {
        get;
        set;
    }
}

public interface IUpdateFacade
{
    ILimitUpdator AddTimeLimitUpdatorMethod(float interval, Action<object> updateHandler, object param = null);

	ILimitUpdator AddFrameLimitUpdatorMethod(int interval, Action<object> updateHandler, object param = null);

    ILimitUpdator CallOnceMethod(int delayFrames, Action<object> updateHandler, object param = null);

	ILimitUpdator CallOnceMethod(float delayTimes, Action<object> updateHandler, object param = null);

    void RemoveLimitUpdatorMethod(ILimitUpdator updator);
}