using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UpdateFacade : IUpdateFacade
{
	abstract class BaseLimitUpdator : ILimitUpdator
	{
		public bool active
		{
			get;
			set;
		}

		protected Action _updateProxyHandler;

		protected Action<object> _updateHandler;

		protected object _param;

		protected bool _once = false;
		protected BaseLimitUpdator()
		{
			_updateProxyHandler = new Action(UpdateProxyHandler);
		}

		protected void ResetVal(Action<object> target, object targetParam, bool once)
		{
			_updateHandler = target;
			_param = targetParam;
			_once = once;
		}

		protected abstract void UpdateProxyHandler();

		public virtual void Dispose()
		{
			_updateHandler = null;
			_param = null;
			_once = false;
			this.active = false;
		}
	}

	class TimeLimitUpdator : BaseLimitUpdator
	{
		private static Stack<TimeLimitUpdator> _timeLimitUpdatorStack = new Stack<TimeLimitUpdator>();

		private double _lastTime;
		private float _interval;
		private bool _inPool = false;

		TimeLimitUpdator()
		{

		}

		public static TimeLimitUpdator Allocate()
		{
			TimeLimitUpdator updator = null;
			if (_timeLimitUpdatorStack.Count > 0)
			{
				updator = _timeLimitUpdatorStack.Pop();
				updator._inPool = false;
			}
			else
			{
				updator = new TimeLimitUpdator();
			}
			return updator;
		}

		public void Start(Action<object> updateHandler, float interval, object param, bool once = false)
		{
			ResetVal(updateHandler, param, once);
			_interval = interval;
			_lastTime = RealTimer.elapsedSeconds;
			_param = param;
			active = true;
			UpdateFacade.instance._updateDelegate += this._updateProxyHandler;
		}


		protected override void UpdateProxyHandler()
		{
			if (active && RealTimer.elapsedSeconds - _lastTime >= _interval)
			{
				_lastTime = RealTimer.elapsedSeconds;
				_updateHandler(_param);
				if (_once)
				{
					Dispose();
				}
			}
		}

		public override void Dispose()
		{
			if (!_inPool)
			{
				//Debug.LogError(_updateHandler.Target+"."+ _updateHandler.Method);
				base.Dispose();
				UpdateFacade.instance._updateDelegate -= this._updateProxyHandler;
				_timeLimitUpdatorStack.Push(this);
				_inPool = true;
			}
		}
	}

	class FrameLimitUpdator : BaseLimitUpdator
	{
		private static Stack<FrameLimitUpdator> _frameLimitUpdatorStack = new Stack<FrameLimitUpdator>();

		private int _lastFrame;
		private int _interval;
		private bool _inPool = false;

		FrameLimitUpdator()
		{
		}

		public static FrameLimitUpdator Allocate()
		{
			FrameLimitUpdator updator = null;
			if (_frameLimitUpdatorStack.Count > 0)
			{
				updator = _frameLimitUpdatorStack.Pop();
				updator._inPool = false;
			}
			else
			{
				updator = new FrameLimitUpdator();
			}
			return updator;
		}

		public void Start(Action<object> updateHandler, int interval, object param, bool once = false)
		{
			this.ResetVal(updateHandler, param, once);
			_interval = interval;
			_lastFrame = Time.frameCount;
			active = true;
			instance.updateEvent += this._updateProxyHandler;
		}


		protected override void UpdateProxyHandler()
		{
			if (active && Time.frameCount - _lastFrame >= _interval)
			{
				_lastFrame = Time.frameCount;
				_updateHandler(_param);

				//            try
				//{
				//	_updateHandler(_param);
				//}
				//catch (Exception e)
				//{
				//	Debug.LogError("Exception :" + e.Message + " " + e.StackTrace);
				//	OutputLogger.Error("Exception: " + e.Message + " " + e.StackTrace);
				//	DataCountUtils.SetErrorEvent(DataCountUtils.TAG_GAME, e.Message + " " + e.StackTrace);
				//}
				if (_once)
				{
					Dispose();
				}
			}
		}

		public override void Dispose()
		{
			if (!_inPool)
			{
				base.Dispose();
				UpdateFacade.instance._updateDelegate -= this._updateProxyHandler;
				_frameLimitUpdatorStack.Push(this);
				_inPool = true;
			}
		}
	}

	protected static UpdateFacade cInstance = null;

    public static UpdateFacade instance
    {
        get {
            if (cInstance == null)
            {
                cInstance = new UpdateFacade();
            }
            return cInstance;
        }
    }

	private Action _updateDelegate;

	public event Action updateEvent
	{
		add { _updateDelegate += value; }

		remove { _updateDelegate -= value; }
	}

	public static ILimitUpdator AddTimeLimitUpdator(float interval, Action<object> updateHandler, object param = null)
	{
		return (instance as IUpdateFacade).AddTimeLimitUpdatorMethod(interval, updateHandler, param);
	}

    public static ILimitUpdator CallOnce(float delayTimes, Action<object> updateHandler, object param = null)
	{
		return (instance as IUpdateFacade).CallOnceMethod(delayTimes, updateHandler, param);
	}

	public static ILimitUpdator AddFrameLimitUpdator(int interval, Action<object> updateHandler, object param = null)
	{
		return (instance as IUpdateFacade).AddFrameLimitUpdatorMethod(interval, updateHandler, param);
	}

	public static ILimitUpdator CallOnce(int delayFrames, Action<object> updateHandler, object param = null)
	{
		return (instance as IUpdateFacade).CallOnceMethod(delayFrames, updateHandler, param);
	}

	public static void RemoveLimitUpdator(ILimitUpdator updator)
	{
		(instance as IUpdateFacade).RemoveLimitUpdatorMethod(updator);
	}

	ILimitUpdator IUpdateFacade.AddTimeLimitUpdatorMethod(float interval, Action<object> updateHandler, object param)
	{
		if (updateHandler == null)
		{
			Debug.LogError("Add Updator Error");
			return null;
		}
		TimeLimitUpdator updator = TimeLimitUpdator.Allocate();
		updator.Start(updateHandler, interval, param);
		return updator;
	}

	ILimitUpdator IUpdateFacade.CallOnceMethod(int delayFrames, Action<object> updateHandler, object param)
	{
		FrameLimitUpdator updator = FrameLimitUpdator.Allocate();
		updator.Start(updateHandler, delayFrames, param, true);
		return updator;
	}

	ILimitUpdator IUpdateFacade.AddFrameLimitUpdatorMethod(int interval, Action<object> updateHandler, object param)
	{
		FrameLimitUpdator updator = FrameLimitUpdator.Allocate();
		updator.Start(updateHandler, interval, param);
		return updator;
	}

	ILimitUpdator IUpdateFacade.CallOnceMethod(float delayTimes, Action<object> updateHandler, object param)
	{
		if (updateHandler == null)
		{
			Debug.LogError("Add Updator Error");
			return null;
		}
		TimeLimitUpdator updator = TimeLimitUpdator.Allocate();
		updator.Start(updateHandler, delayTimes, param, true);
		return updator;
	}

	void IUpdateFacade.RemoveLimitUpdatorMethod(ILimitUpdator updator)
	{
		BaseLimitUpdator limitUpdator = updator as BaseLimitUpdator;
		if (limitUpdator != null)
		{
			limitUpdator.Dispose();
		}
	}

	public void Update()
	{
		_updateDelegate?.Invoke();
	}

	public void Clean()
	{
		_updateDelegate = null;
	}
}
