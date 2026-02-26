using System.Diagnostics;

public class RealTimer
{
	private static Stopwatch _watcher;

	public static void Reset()
	{
		if (_watcher == null)
		{
			_watcher = Stopwatch.StartNew();
		}
		else
		{
			_watcher.Reset();
			_watcher.Start();
		}
	}

	public static long elapsedMilliseconds
	{
		get {
            if (_watcher == null)
            {
                _watcher = Stopwatch.StartNew();
            }
            return _watcher.ElapsedMilliseconds;
        }
	}

	public static double elapsedSeconds
	{
		get {
            if(_watcher == null)
            {
                _watcher = Stopwatch.StartNew();
            }
            return _watcher.Elapsed.TotalSeconds;
        }
	}
}
