using UniRx;
using UnityEngine;

public class LogCallback
{
    public string Condition;
    public string StackTrace;
    public LogType LogType;
}

public static class LogHelper
{
    public static IObservable<LogCallback> LogCallbackAsObservable(){
    	return Observable.FromEvent<Application.LogCallback, LogCallback>(
    	    h => (condition, stackTrace, type) => h(new LogCallback { Condition = condition, StackTrace = stackTrace, LogType = type }),
    	    h => Application.logMessageReceived += h, h => Application.logMessageReceived -= h);
	}
}

