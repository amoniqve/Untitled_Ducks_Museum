using UnityEngine;

/// <summary>
/// Suppresses known Unity-internal log messages that cannot be fixed without
/// external tooling (e.g. video file re-encoding).
/// The filter is installed before any scene object initialises so it catches
/// VideoPlayer warnings that fire during Awake().
/// </summary>
public class LogFilter : MonoBehaviour
{
    private static readonly string[] SuppressedSubstrings =
    {
        "Unexpected timestamp values detected",
        "Color primaries 0 is unknown or unsupported",
    };

    // Runs before any scene Awake() — catches VideoPlayer's first-frame warnings
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        Debug.unityLogger.logHandler = new FilteredLogHandler(Debug.unityLogger.logHandler);
    }

    private class FilteredLogHandler : ILogHandler
    {
        private readonly ILogHandler _inner;

        public FilteredLogHandler(ILogHandler inner) => _inner = inner;

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            string message = (args != null && args.Length > 0)
                ? string.Format(format, args)
                : format;

            if (!IsSuppressed(message))
                _inner.LogFormat(logType, context, format, args);
        }

        public void LogException(System.Exception exception, Object context)
            => _inner.LogException(exception, context);

        private static bool IsSuppressed(string msg)
        {
            foreach (string s in SuppressedSubstrings)
                if (msg.Contains(s)) return true;
            return false;
        }
    }
}
