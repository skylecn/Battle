using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// This class allows statically (globally) configuring and using logging functionality.
/// </summary>
public class OutputLogger
{

    private static readonly object static_sync_lock = new object();

    public static bool Enabled = true;

    public static LogLevel Level = LogLevel.Verbose;

    public static string UserIdentifier = Environment.UserName;

    public static string GameIdentifier = @"CC";

    public static string VersionIdentifier = Application.version;

    private static readonly Dictionary<string, OutputLogger> static_loggers = new Dictionary<string, OutputLogger>();

    private static readonly HashSet<string> reserved_names = new HashSet<string>(Enum.GetNames(typeof(LoggingTarget)).Select(n => n.ToLower()));

    public LoggingTarget? Target { get; set; }

    public string Name { get; set; }

    public string Filename;

    private StreamWriter writer;

    Queue<string> logQueue = new Queue<string>();
    Task writeTask;

    private bool headerAdded;

    public static void Error(Exception e, string description, LoggingTarget target = LoggingTarget.Runtime, bool recursive = false)
    {
        error(e, description, target, null, recursive);
    }

    public static void Error(Exception e, string description, string name, bool recursive = false)
    {
        error(e, description, null, name, recursive);
    }

    private static void error(Exception e, string description, LoggingTarget? target, string name, bool recursive)
    {
        log(description, target, name, LogLevel.Error);
        log(e.ToString(), target, name, LogLevel.Important);

        if (recursive)
            for (Exception inner = e.InnerException; inner != null; inner = inner.InnerException)
                log(inner.ToString(), target, name, LogLevel.Important);
    }

    public static void Log(string message, string className, string functionName, LoggingTarget target = LoggingTarget.Runtime, LogLevel level = LogLevel.Verbose)
    {
        log($"{className}.{functionName}:{message}", target, null, level);
    }

    public static void Log(string message, LoggingTarget target = LoggingTarget.Runtime, LogLevel level = LogLevel.Verbose)
    {
        log(message, target, null, level);
    }

    public static void Error(string message, LoggingTarget target = LoggingTarget.Runtime, LogLevel level = LogLevel.Error)
    {
        log(message, target, null, level);
    }

    public static void Log(string message, string name, LogLevel level = LogLevel.Verbose)
    {
        log(message, null, name, level);
    }

    private static void log(string message, LoggingTarget? target, string loggerName, LogLevel level)
    {
        try
        {
            if (target.HasValue)
                GetLogger(target.Value).Add(message, level);
            else
                GetLogger(loggerName).Add(message, level);
        }
        catch
        {
        }
    }

    public static OutputLogger GetLogger(LoggingTarget target = LoggingTarget.Runtime)
    {
        // there can be no name conflicts between LoggingTarget-based Loggers and named loggers because
        // every name that would coincide with a LoggingTarget-value is reserved and cannot be used (see ctor).
        return GetLogger(target.ToString());
    }

    public static OutputLogger GetLogger(string name)
    {
        lock (static_sync_lock)
        {
            var nameLower = name.ToLower();
            OutputLogger l;
            if (!static_loggers.TryGetValue(nameLower, out l))
            {
                LoggingTarget target;
                try
                {
                    target = (LoggingTarget)Enum.Parse(typeof(LoggingTarget), name, true);
                    l = new OutputLogger(target);
                }
                catch (Exception e)
                {
                    l = new OutputLogger(name);
                }
                static_loggers[nameLower] = l;
            }

            return l;
        }
    }

    private OutputLogger(LoggingTarget target = LoggingTarget.Runtime)
    {
        Target = target;
        if (Target != null)
            Filename = String.Format("{0}/Log/{1}.txt", Application.persistentDataPath, Target.ToString().ToLower());

        Init();
    }

    private OutputLogger(string name)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(name.Trim()))
            throw new ArgumentException("The name of a logger must be non-null and may not contain only white space.", "name");

        if (reserved_names.Contains(name.ToLower()))
            throw new ArgumentException(String.Format("The name \"{0}\" is reserved. Please use the {1}-value corresponding to the name instead.", name, "LoggingTarget"));

        Name = name;
        Filename = String.Format("{0}/Log/{1}.txt", Application.persistentDataPath, Name.ToLower());
        
        Init();
    }

    void Init()
    {
        string path = Directory.GetParent(Filename).FullName;
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }
        writer = File.CreateText(Filename);

        ensureHeader();

    }

    [Conditional("DEBUG")]
    public void Debug(string message = @"")
    {
        Add(message, LogLevel.Debug);
    }

    public void Add(string message = @"", LogLevel level = LogLevel.Verbose)
    {
        add(message, level);
    }

    private async void add(string message = @"", LogLevel level = LogLevel.Verbose)
    {
        if (!Enabled || level < Level)
            return;

        //ensureHeader();

        //split each line up.
        string[] lines = message.Replace(@"\r\n", @"\n").Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string s = lines[i];
            lines[i] = string.Format("{0} : {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), s.Trim());
        }

        lock (logQueue) {
            foreach (var line in lines)
                logQueue.Enqueue(line);
        }

        if (writeTask == null || writeTask.IsCompleted)
        {
            writeTask = Task.Run(WriteTask);
        }
    }

    void WriteTask()
    {
        lock (logQueue)
        {
            while (logQueue.Count > 0)
            {
                writer.WriteLine(logQueue.Dequeue());
            }
        }
    }

    private void ensureHeader()
    {
        if (headerAdded) return;
        headerAdded = true;

        add("----------------------------------------------------------");
        add(String.Format("{0} Log for {1}", Target, UserIdentifier));
        add(String.Format("{0} version {1}", GameIdentifier, VersionIdentifier));
        add(String.Format("Running on {0}, {1} cores", Environment.OSVersion, Environment.ProcessorCount));
        add("----------------------------------------------------------");
    }

    public void Close()
    {
        writer.Flush();
        writer.Close();
    }

    public static void CloseAll()
    {
        lock (static_sync_lock)
        {
            foreach (var logger in static_loggers.Values)
                logger.Close();
        }
    }

    public enum LogLevel
    {
        /// <summary>
        /// Log-level for debugging-related log-messages. This is the lowest level (highest verbosity). Please note that this will log input events, including keypresses when entering a password.
        /// </summary>
        Debug,

        /// <summary>
        /// Log-level for most log-messages. This is the second-lowest level (second-highest verbosity).
        /// </summary>
        Verbose,

        /// <summary>
        /// Log-level for important log-messages. This is the second-highest level (second-lowest verbosity).
        /// </summary>
        Important,

        /// <summary>
        /// Log-level for error messages. This is the highest level (lowest verbosity).
        /// </summary>
        Error
    }

    public enum LoggingTarget
    {

        /// <summary>
        /// Logging target for information about the runtime.
        /// </summary>
        Runtime,

        /// <summary>
        /// Logging target for network-related events.
        /// </summary>
        Network,

        /// <summary>
        /// Logging target for performance-related information.
        /// </summary>
        Performance,

        /// <summary>
        /// Logging target for database-related events.
        /// </summary>
        Database,

        Resource,
    }
}
 
