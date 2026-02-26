using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 调试日志记录器
/// </summary>
public class DebugLogger
{
    /// <summary>
    /// 运行时选项
    /// </summary>
    public enum RuntimeOption
    {
        /// <summary>
        /// 在任何运行时均不输出日志
        /// </summary>
        None = 0,

        /// <summary>
        /// 在热更模式下启用日志输出
        /// </summary>
        HotFix = 1 << 0,

        /// <summary>
        /// 在非热更模式下启用日志输出
        /// </summary>
        OverAll = 1 << 1,

        /// <summary>
        /// 在所有模式下启用日志输出
        /// </summary>
        All = HotFix | OverAll,
    }

    public enum PlatformOption
    {
        /// <summary>
        /// 在任何平台均不输出日志
        /// </summary>
        None = 0,

        /// <summary>
        /// 在Unity编辑器下启用日志输出
        /// </summary>
        Editor = 1 << 0,

        /// <summary>
        /// 在安卓平台下启用日志输出
        /// </summary>
        Android = 1 << 1,

        /// <summary>
        /// 在iOS平台下启用日志输出
        /// </summary>
        iOS = 1 << 2,

        /// <summary>
        /// 在正式Release包中启用日志输出
        /// </summary>
        ReleasePackage = 1 << 3,

        ///////////////////////////
        // 若有新平台需求请添加在此

        /// <summary>
        /// 在所有平台下启用日志输出
        /// </summary>
        All = Editor | Android | iOS | ReleasePackage,

        /// <summary>
        /// 在移动平台下启用日志输出
        /// </summary>
        MobileOnly = Android | iOS,

        /// <summary>
        /// 只在正式包中启用日志输出
        /// </summary>
        ReleaseOnly = ReleasePackage,
    }

    /// <summary>
    /// 格式选项
    /// </summary>
    public enum FormatOption
    {
        /// <summary>
        /// 无任何特殊格式（直接输出消息本体）
        /// </summary>
        None = 0,

        /// <summary>
        /// 添加本地时间
        /// </summary>
        LocalTime = 1 << 0,

        /// <summary>
        /// 添加调用方信息（建议格式为 类名.方法名）
        /// </summary>
        CallerInfo = 1 << 1,

        /// <summary>
        /// 完整输出
        /// </summary>
        Full = LocalTime | CallerInfo,
    }

    /// <summary>
    /// 默认运行时选项
    /// </summary>
    public const RuntimeOption DEFAULT_RUNTIME_OPTION = RuntimeOption.All;

    /// <summary>
    /// 默认平台选项
    /// </summary>
    public const PlatformOption DEFAULT_PLATFORM_OPTION = PlatformOption.Editor;

    /// <summary>
    /// 默认格式选项
    /// </summary>
    public const FormatOption DEFAULT_FORMAT_OPTION = FormatOption.Full;

    // 总览格式
    private const string FORMAT_OVERVIEW = "{0}{1}{2}";
    // 本地时间格式
    private const string FORMAT_LOCAL_TIME = "[{0:yyyy-MM-dd HH:mm:ss.fff}]";
    // 调用方信息格式
    private const string FORMAT_CALLER_INFO = "[{0}]: ";

    /// <summary>
    /// 是否启用日志输出，设为false将完全禁止日志输出
    /// </summary>
    public static bool Enabled = true;

    /// <summary>
    /// 运行时覆盖选项，设为None以外的值时所有日志输出将强制应用此选项
    /// </summary>
    public static RuntimeOption OverrideRuntimeOption = RuntimeOption.None;

    /// <summary>
    /// 平台覆盖选项，设为None以外的值时所有日志输出将强制应用此选项
    /// </summary>
    public static PlatformOption OverridePlatformOption = PlatformOption.None;

    /// <summary>
    /// 格式覆盖选项，设为None以外的值时所有日志输出将强制应用此选项
    /// </summary>
    public static FormatOption OverrideFormatOption = FormatOption.None;

    // 判断当前运行时是否可以输出
    private static bool CheckRuntime(RuntimeOption option)
    {
        if (!Enabled) return false;
        if (OverrideRuntimeOption != RuntimeOption.None) option = OverrideRuntimeOption;
        if (option == RuntimeOption.None) return false;
        if ((option & RuntimeOption.HotFix) == RuntimeOption.HotFix) return true;
        if ((option & RuntimeOption.OverAll) == RuntimeOption.OverAll) return true;
        return false;
    }

    // 判断当前平台是否可以输出
    private static bool CheckPlatform(PlatformOption option)
    {
        if (!Enabled) return false;
        if (OverridePlatformOption != PlatformOption.None) option = OverridePlatformOption;
        if (option == PlatformOption.None) return false;
        if ((option & PlatformOption.ReleasePackage) == PlatformOption.ReleasePackage) return true;
        if ((option & PlatformOption.Editor) == PlatformOption.Editor) return true;
        if ((option & PlatformOption.Android) == PlatformOption.Android) return true;
        if ((option & PlatformOption.iOS) == PlatformOption.iOS) return true;
        return false;
    }

    // 格式化消息
    private static string FormatMsg(string msg, string callerInfo, FormatOption option)
    {
        if (OverrideFormatOption != FormatOption.None) option = OverrideFormatOption;
        string localTimeStr = null;
        string callerInfoStr = null;
        if ((option & FormatOption.LocalTime) == FormatOption.LocalTime) localTimeStr = string.Format(FORMAT_LOCAL_TIME, DateTime.Now);
        if ((option & FormatOption.CallerInfo) == FormatOption.CallerInfo) callerInfoStr = string.Format(FORMAT_CALLER_INFO, callerInfo);
        return string.Format(FORMAT_OVERVIEW, localTimeStr, callerInfoStr, msg);
    }

    /// <summary>
    /// 输出带格式普通日志
    /// </summary>
    /// <param name="format">格式</param>
    /// <param name="args">参数</param>
    /// <param name="callerInfo">调用方信息，建议使用“类名.方法名”格式</param>
    /// <param name="runtimeOption">运行时选项</param>
    /// <param name="platformOption">平台选项</param>
    /// <param name="formatOption">格式选项</param>
    /// <param name="useOutputLogger">是否同步输出到OutputLogger</param>
    /// <param name="target">OutputLogger目标</param>
    /// <param name="logLevel">OutputLogger级别</param>
    public static void LogFormat(string format, object[] args, string callerInfo="",
        PlatformOption platformOption = DEFAULT_PLATFORM_OPTION,
        RuntimeOption runtimeOption = DEFAULT_RUNTIME_OPTION,
        FormatOption formatOption = DEFAULT_FORMAT_OPTION,
        bool useOutputLogger = false,
        OutputLogger.LoggingTarget target = OutputLogger.LoggingTarget.Runtime,
        OutputLogger.LogLevel logLevel = OutputLogger.LogLevel.Verbose)
    {
        if (!CheckRuntime(runtimeOption)) return;
        if (!CheckPlatform(platformOption)) return;
        string msg = string.Format(format, args);
        Debug.Log(FormatMsg(msg, callerInfo, formatOption));
        if (useOutputLogger) OutputLogger.Log(msg, target, logLevel);
    }

    /// <summary>
    /// 输出普通日志
    /// </summary>
    /// <param name="msg">消息内容</param>
    /// <param name="callerInfo">调用方信息，建议使用“类名.方法名”格式</param>
    /// <param name="runtimeOption">运行时选项</param>
    /// <param name="platformOption">平台选项</param>
    /// <param name="formatOption">格式选项</param>
    /// <param name="useOutputLogger">是否同步输出到OutputLogger</param>
    /// <param name="target">OutputLogger目标</param>
    /// <param name="logLevel">OutputLogger级别</param>
    public static void Log(string msg, string callerInfo="",
    PlatformOption platformOption = DEFAULT_PLATFORM_OPTION,
    RuntimeOption runtimeOption = DEFAULT_RUNTIME_OPTION,
    FormatOption formatOption = DEFAULT_FORMAT_OPTION,
    bool useOutputLogger = false,
    OutputLogger.LoggingTarget target = OutputLogger.LoggingTarget.Runtime,
    OutputLogger.LogLevel logLevel = OutputLogger.LogLevel.Verbose)
    {
        if (!CheckRuntime(runtimeOption)) return;
        if (!CheckPlatform(platformOption)) return;
        Debug.Log(FormatMsg(msg, callerInfo, formatOption));
        if (useOutputLogger) OutputLogger.Log(msg, target, logLevel);
    }

    /// <summary>
    /// 输出带格式警告日志
    /// </summary>
    /// <param name="format">格式</param>
    /// <param name="args">参数</param>
    /// <param name="callerInfo">调用方信息，建议使用“类名.方法名”格式</param>
    /// <param name="runtimeOption">运行时选项</param>
    /// <param name="platformOption">平台选项</param>
    /// <param name="formatOption">格式选项</param>
    /// <param name="useOutputLogger">是否同步输出到OutputLogger</param>
    /// <param name="target">OutputLogger目标</param>
    /// <param name="logLevel">OutputLogger级别</param>
    public static void LogWarningFormat(string format, object[] args, string callerInfo="",
        PlatformOption platformOption = DEFAULT_PLATFORM_OPTION,
        RuntimeOption runtimeOption = DEFAULT_RUNTIME_OPTION,
        FormatOption formatOption = DEFAULT_FORMAT_OPTION,
        bool useOutputLogger = false,
        OutputLogger.LoggingTarget target = OutputLogger.LoggingTarget.Runtime,
        OutputLogger.LogLevel logLevel = OutputLogger.LogLevel.Verbose)
    {
        if (!CheckRuntime(runtimeOption)) return;
        if (!CheckPlatform(platformOption)) return;
        string msg = string.Format(format, args);
        Debug.LogWarning(FormatMsg(msg, callerInfo, formatOption));
        if (useOutputLogger) OutputLogger.Log(msg, target, logLevel);
    }

    /// <summary>
    /// 输出警告日志
    /// </summary>
    /// <param name="msg">消息内容</param>
    /// <param name="callerInfo">调用方信息，建议使用“类名.方法名”格式</param>
    /// <param name="runtimeOption">运行时选项</param>
    /// <param name="platformOption">平台选项</param>
    /// <param name="formatOption">格式选项</param>
    /// <param name="useOutputLogger">是否同步输出到OutputLogger</param>
    /// <param name="target">OutputLogger目标</param>
    /// <param name="logLevel">OutputLogger级别</param>
    public static void LogWarning(string msg, string callerInfo="",
        PlatformOption platformOption = DEFAULT_PLATFORM_OPTION,
        RuntimeOption runtimeOption = DEFAULT_RUNTIME_OPTION,
        FormatOption formatOption = DEFAULT_FORMAT_OPTION,
        bool useOutputLogger = false,
        OutputLogger.LoggingTarget target = OutputLogger.LoggingTarget.Runtime,
        OutputLogger.LogLevel logLevel = OutputLogger.LogLevel.Verbose)
    {
        if (!CheckRuntime(runtimeOption)) return;
        if (!CheckPlatform(platformOption)) return;
        Debug.LogWarning(FormatMsg(msg, callerInfo, formatOption));
        if (useOutputLogger) OutputLogger.Log(msg, target, logLevel);
    }

    /// <summary>
    /// 输出带格式错误日志
    /// </summary>
    /// <param name="format">格式</param>
    /// <param name="args">参数</param>
    /// <param name="callerInfo">调用方信息，建议使用“类名.方法名”格式</param>
    /// <param name="runtimeOption">运行时选项</param>
    /// <param name="platformOption">平台选项</param>
    /// <param name="formatOption">格式选项</param>
    /// <param name="useOutputLogger">是否同步输出到OutputLogger</param>
    /// <param name="target">OutputLogger目标</param>
    /// <param name="logLevel">OutputLogger级别</param>
    public static void LogErrorFormat(string format, object[] args, string callerInfo="",
        PlatformOption platformOption = DEFAULT_PLATFORM_OPTION,
        RuntimeOption runtimeOption = DEFAULT_RUNTIME_OPTION,
        FormatOption formatOption = DEFAULT_FORMAT_OPTION,
        bool useOutputLogger = false,
        OutputLogger.LoggingTarget target = OutputLogger.LoggingTarget.Runtime,
        OutputLogger.LogLevel logLevel = OutputLogger.LogLevel.Error)
    {
        if (!CheckRuntime(runtimeOption)) return;
        if (!CheckPlatform(platformOption)) return;
        string msg = string.Format(format, args);
        Debug.LogError(FormatMsg(msg, callerInfo, formatOption));
        if (useOutputLogger) OutputLogger.Log(msg, target, logLevel);
    }

    /// <summary>
    /// 输出错误日志
    /// </summary>
    /// <param name="msg">消息内容</param>
    /// <param name="callerInfo">调用方信息，建议使用“类名.方法名”格式</param>
    /// <param name="runtimeOption">运行时选项</param>
    /// <param name="platformOption">平台选项</param>
    /// <param name="formatOption">格式选项</param>
    /// <param name="useOutputLogger">是否同步输出到OutputLogger</param>
    /// <param name="target">OutputLogger目标</param>
    /// <param name="logLevel">OutputLogger级别</param>
    public static void LogError(string msg, string callerInfo="",
        PlatformOption platformOption = DEFAULT_PLATFORM_OPTION,
        RuntimeOption runtimeOption = DEFAULT_RUNTIME_OPTION,
        FormatOption formatOption = DEFAULT_FORMAT_OPTION,
        bool useOutputLogger = false,
        OutputLogger.LoggingTarget target = OutputLogger.LoggingTarget.Runtime,
        OutputLogger.LogLevel logLevel = OutputLogger.LogLevel.Error)
    {
        if (!CheckRuntime(runtimeOption)) return;
        if (!CheckPlatform(platformOption)) return;
        Debug.LogError(FormatMsg(msg, callerInfo, formatOption));
        if (useOutputLogger) OutputLogger.Log(msg, target, logLevel);
    }

}
